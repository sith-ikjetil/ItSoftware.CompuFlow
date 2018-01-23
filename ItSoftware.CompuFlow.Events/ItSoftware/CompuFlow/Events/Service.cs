using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Messaging;
using ItSoftware.CompuFlow.Common;
using ItSoftware.CompuFlow.Manifest;
using ItSoftware.CompuFlow.Events.HostRuntime;
using ItSoftware.ExceptionHandler;
using ItSoftware.CompuFlow.Util;
using ItSoftware.CompuFlow.Common.Status.Hosts;
namespace ItSoftware.CompuFlow.Events
{
    public partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
        }

        #region Private Properties
        /// <summary>
        /// Main worker thread.
        /// </summary>
        private Thread WorkerThread { get; set; }
        #endregion

        #region Public Properties
        /// <summary>
        /// Is the service paused.
        /// </summary>
        public bool Paused { get; private set; }
        /// <summary>
        /// Is this service stopped.
        /// </summary>
        public bool Stopped { get; private set; }
        #endregion

        #region Protected Override Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            this.WorkerThread = new Thread(new ThreadStart(WorkerMethod));
            this.WorkerThread.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnStop()
        {
            this.Stopped = true;
            base.OnStop();

            //
            // Give the thread 10 seconds to clean up.
            //
            long timeout = 1000 * 10;
            while (this.WorkerThread.IsAlive && timeout > 0)
            {
                Thread.Sleep(100);
                timeout -= 100;
            }

            if (this.WorkerThread.IsAlive)
            {
                this.WorkerThread.Abort();
                while (this.WorkerThread.IsAlive)
                {
                    Thread.Sleep(100);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPause()
        {
            this.Paused = true;
            base.OnPause();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnContinue()
        {
            this.Paused = false;
            base.OnContinue();
        }
        #endregion

        #region Private Helper Methods
        /// <summary>
        /// Puts thread to sleep for timeMs milliseconds. This method gets called only in debug mode.
        /// </summary>
        /// <param name="timeMs"></param>
        [Conditional("DEBUG")]
        private void ThreadSleep(int timeMs)
        {
            Thread.Sleep(timeMs);
        }
        #endregion

        #region Worker Method
        private void WorkerMethod()
        {
            //
            // Thread sleep for debugging purposes only. Se conditional attribute.
            //
            ThreadSleep(15000);

            try
            {
                //
                // Get/Load Settings
                //
                Settings settings = new Settings();

                //
                // Delete temporary flow files, temp files and folders
                //
                DeleteTemporaryFiles(settings);

                //
                // Create controller object.
                //
                Controller<Settings, TransparentHandler, RealHandler, HostRuntimeSettings> controller = new Controller<Settings, TransparentHandler, RealHandler, HostRuntimeSettings>(settings, false);

                //
                // Status Information Host
                //
                StatusInformationHost<Settings, TransparentHandler, RealHandler, HostRuntimeSettings> statusInformationHost = new StatusInformationHost<Settings, TransparentHandler, RealHandler, HostRuntimeSettings>(controller, "net.pipe://localhost/ItSoftware.CompuFlow.Events");

                //
                // Worker loop.
                //
                MessageQueue mq = new MessageQueue(settings.SourceMsmqPath);
                mq.Formatter = new BinaryMessageFormatter();
                System.Messaging.Message sourceMsg = null;
                do
                {
                    try
                    {
                        if (this.Paused)
                        {
                            Thread.Sleep(new TimeSpan(0, 0, 5));
                        }
                        else
                        {
                            try
                            {
                                sourceMsg = null;
                                sourceMsg = mq.Receive(new TimeSpan(0, 0, 5));
                            }
                            catch (MessageQueueException mqe)
                            {
                                if (mqe.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                                {
                                    HandlerException ge = new HandlerException("MessageQueue.Receive threw an unexpected exception.", mqe);
                                    ExceptionManager.PublishException(ge, "Log Warning Policy");

                                    Thread.Sleep(new TimeSpan(0, 0, 5));
                                }
                            }
                            if (sourceMsg != null)
                            {
                                FlowManifest flowManifest = new FlowManifest(sourceMsg.Body as string);
                                controller.ExecuteFlow(flowManifest);
                            }
                        }
                    }
                    catch (OutOfMemoryException)
                    {
                        throw;
                    }
                    catch (StackOverflowException)
                    {
                        throw;
                    }
                    catch (ThreadAbortException)
                    {
                        controller.Stop();
                        throw;
                    }
                    catch (Exception x)
                    {
                        ExceptionManager.PublishException(x, "Error");
                    }
                } while (!this.Stopped);
                controller.Stop();
            }
            catch (ThreadAbortException tae)
            {
                if (!this.Stopped)
                {
                    ExceptionManager.PublishException(tae, "Error");
                    System.ServiceProcess.ServiceController sc = new System.ServiceProcess.ServiceController(base.ServiceName);
                    sc.Stop();
                }
            }
            catch (SettingsException se)
            {
                HandlerException ge = new HandlerException("Handler could not continue execution.", se);
                ExceptionManager.PublishException(ge, "Error");

                System.ServiceProcess.ServiceController sc = new System.ServiceProcess.ServiceController(base.ServiceName);
                sc.Stop();
            }
            catch (HandlerException ge)
            {
                ExceptionManager.PublishException(ge, "Error");

                System.ServiceProcess.ServiceController sc = new System.ServiceProcess.ServiceController(base.ServiceName);
                sc.Stop();
            }
            catch (RuntimeWrappedException rwe)
            {
                HandlerException ge = new HandlerException("An unhandled RuntimeWrappedException exception was thrown.\r\nHandler could not continue execution.", rwe);
                ExceptionManager.PublishException(ge, "Error");

                System.ServiceProcess.ServiceController sc = new System.ServiceProcess.ServiceController(base.ServiceName);
                sc.Stop();
            }
            catch (Exception x)
            {
                HandlerException ge = new HandlerException("An unhandled CLS complient exception was thrown.\r\nHandler could not continue execution.", x);
                ExceptionManager.PublishException(ge, "Error");

                System.ServiceProcess.ServiceController sc = new System.ServiceProcess.ServiceController(base.ServiceName);
                sc.Stop();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Delete all temporary files.
        /// </summary>
        /// <param name="settings"></param>
        private void DeleteTemporaryFiles(Settings settings)
        {
            string[] directories = Directory.GetDirectories(settings.TemporaryHandlerFilesDirectory);
            DeleteDirectories(directories);
        }

        /// <summary>
        /// Deletes directories.
        /// </summary>
        /// <param name="directories"></param>
        private void DeleteDirectories(string[] directories)
        {
            foreach (string directory in directories)
            {
                FileSystem.DeleteDirectoryStructure(directory);
            }
        }
        #endregion
    }
}
