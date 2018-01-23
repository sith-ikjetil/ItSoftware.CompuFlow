using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Messaging;
using System.Xml;
using System.IO;
using ItSoftware.CompuFlow.Common;
using ItSoftware.CompuFlow.Util;
using ItSoftware.CompuFlow.Manifest;
using ItSoftware.CompuFlow.Common.HostRuntime;
using ItSoftware.ExceptionHandler;
using System.Reflection;
using ItSoftware.CompuFlow.Common.Status;
using ItSoftware.CompuFlow.Publisher.Interfaces;
namespace ItSoftware.CompuFlow.Publisher.HostRuntime
{
    /// <summary>
    /// 
    /// </summary>
    public class RealPublisher : RealFlow<HostRuntimeSettings>
    {
        #region Public Override Methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override StatusProgressItemCollection GatherStatusInformation()
        {
            StatusProgressItemCollection progressItems = base.GatherStatusInformation();

            progressItems.Add(this.GetProgressItem());

            return progressItems;
        }
        /// <summary>
        /// Execute a flow.
        /// </summary>
        /// <param name="flowManifest"></param>
        /// <param name="hostRuntimeSettings"></param>
        public override void Execute(string flowManifest, HostRuntimeSettings settings)
        {
            // This base call sets up the this.CurrentExecutingFlowManifest.
            base.Execute(flowManifest, settings);

            //
            // First of all clear previous status information
            //
            this.m_statusInformation.Clear();


            FlowManifest manifest = this.CurrentExecutingFlowManifest; //= null;
            System.Globalization.CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            try
            {
                //XmlDocument xdManifest = new XmlDocument();
                //xdManifest.LoadXml(flowManifest);
                //manifest = new FlowManifest(xdManifest);
                if (this.BindingConfig == null)
                {
                    //
                    // Initialize the application cache.
                    //
                    this.ApplicationCache = new Hashtable();

                    //
                    // Set data directory
                    //
                    this.DataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                    if (!Directory.Exists(this.DataDirectory))
                    {
                        Directory.CreateDirectory(this.DataDirectory);
                    }

                    //
                    // Set config directory
                    //
                    this.ConfigDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
                    if (!Directory.Exists(this.ConfigDirectory))
                    {
                        //Directory.CreateDirectory(this.ConfigDirectory);
                        throw new HostRuntimeException("Missing Config directory in package file.");
                    }

                    //
                    // Load configuration data.
                    //
                    XmlDocument xdRetrival = new XmlDocument();
                    xdRetrival.Load(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config"), "Binding.xml"));
                    this.BindingConfig = new PublisherBindingConfig(xdRetrival);
                    this.ProgressItem.Description = this.BindingConfig.Description;
                }

                //
                // Reset progress item.
                //                
                this.ProgressItem.ErrorInfo = "";
                this.ProgressItem.ExecutionTime = TimeSpan.Zero;
                this.ProgressItem.StartTime = DateTime.Now;
                this.ProgressItem.Status = "Running";


                // !NB! NO OUTPUT DIRECTORY IN PUBLISHER
                // Create output directory and session object.
                //                                        
                //string outputDirectory = FileSystem.NormalizeDirectoryPath(Path.Combine(settings.OutputDirectory, manifest.GUID.ToString()));
                //if (!Directory.Exists(outputDirectory))
                //{
                //    Directory.CreateDirectory(outputDirectory);
                //}

                //
                // Create temp directory.
                //
                string tempDirectory = FileSystem.NormalizeDirectoryPath(Path.Combine(settings.TempDirectory, manifest.GUID.ToString()));
                if (!Directory.Exists(tempDirectory))
                {
                    Directory.CreateDirectory(tempDirectory);
                }
                
                //
                // Execute generator.
                //
                DateTime dtBefore = DateTime.Now;
                this.BindingConfig.IPublisherRef.Publish(manifest.InputParameters, this.ApplicationCache, tempDirectory, this.ConfigDirectory, this.DataDirectory, manifest.RetrivalOutputDirectory, manifest.GeneratorOutputDirectory, this);
                DateTime dtAfter = DateTime.Now;
                this.AddExecutionTime(dtAfter - dtBefore);
                
                this.ProgressItem.Status = "OK";

                //
                // Delete temp directory.
                //
                try
                {
                    FileSystem.DeleteDirectoryStructure(tempDirectory);
                }
                catch (Exception) { }

                //
                // Always perform a garbage collection after generation. Some generators, like powerpoint etc., hold 
                // on to objects that make processes like powerpoint to keep running. We have to run the finalizers so 
                // that they are released.
                //
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                

                 
                // !NB! NOT DESTINATION AFTER PUBLISHER. FINAL STAGE.
                // Providers have executed. Pass message to destination and log.
                //
                //string msmqPath = settings.DestinationMsmqPath;
                //try
                //{
                //    manifest.FlowStatus = FlowStatus.CompletedPublisher;

                //    //
                //    // Send to generator.
                //    //
                //    using (MessageQueue mqPublisher = new MessageQueue(settings.DestinationMsmqPath))
                //    {
                //        mqPublisher.Formatter = new BinaryMessageFormatter();

                //        System.Messaging.Message msgPublisher = new System.Messaging.Message();
                //        msgPublisher.Formatter = new BinaryMessageFormatter();
                //        msgPublisher.Body = manifest.ToXmlDocument().OuterXml;
                //        msgPublisher.Label = string.Format("{0} - {1}", manifest.FlowID, manifest.FlowStatus.ToString());

                //        mqPublisher.Send(msgPublisher);
                //    }
                //
                // Send message to log.
                //
                if (settings.Log && this.LogToEvents)
                {
                    manifest.FlowStatus = FlowStatus.CompletedPublisher; 
                    string msmqPath = settings.EventsMsmqPath;
                    using (MessageQueue mqLog = new MessageQueue(settings.EventsMsmqPath))
                    {
                        mqLog.Formatter = new BinaryMessageFormatter();

                        System.Messaging.Message msgLog = new System.Messaging.Message();
                        msgLog.Formatter = new BinaryMessageFormatter();
                        msgLog.Body = manifest.ToXmlDocument().OuterXml;
                        msgLog.Label = string.Format("{0} - {1}", manifest.FlowID, manifest.FlowStatus.ToString());

                        mqLog.Send(msgLog);
                    }
                }

                // 
                // If Retrival has ended the execution flow return now.
                //
                if (this.EndFlow)
                {
                    return;
                }

                //}
                //catch (StackOverflowException)
                //{
                //    throw;
                //}
                //catch (OutOfMemoryException)
                //{
                //    throw;
                //}
                //catch (ThreadAbortException)
                //{
                //    throw;
                //}
                //catch (Exception x)
                //{
                //    string msg = string.Format("Failure to send message to MSMQ Path: {0}\r\nFlow: {0}.", msmqPath, manifest.GUID.ToString());
                //    throw new HostRuntimeException(msg, x);
                //}
            }
            catch (StackOverflowException)
            {
                throw;
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception x)
            {
                this.ProgressItem.Status = "Failure";
                this.ProgressItem.ErrorInfo = string.Format("Type:\r\n{0}\r\n\r\nSource:\r\n{1}\r\n\r\nMessage:\r\n{2}\r\n\r\nStack Trace:\r\n{3}", x.GetType().FullName, (x.Source != null) ? x.Source : "<NULL>", x.Message, x.StackTrace);
                HostRuntimeException hre = new HostRuntimeException("Retrival.HostRuntime's RealRetrival.Execute method failed.", x);
                ExceptionManager.PublishException(hre, "Error");
                
                //
                // Uphold failure policy. Save packet to failure directory.
                //
                if (manifest != null)
                {
                    //
                    // Save FlowManifest to failure directory.
                    //
                    string failureDirectory = Path.Combine(settings.FailureDirectory, manifest.GUID.ToString());
                    try
                    {
                        if (Directory.Exists(failureDirectory))
                        {
                            FileSystem.DeleteDirectoryStructure(failureDirectory);
                        }
                        Directory.CreateDirectory(failureDirectory);
                        manifest.ToXmlDocument().Save(Path.Combine(failureDirectory, "FlowManifest.xml"));
                    }
                    catch (StackOverflowException)
                    {
                        throw;
                    }
                    catch (OutOfMemoryException)
                    {
                        throw;
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception x2)
                    {
                        ExceptionManager.PublishException(new HostRuntimeException("Failed to uphold failure policy.", x2),"Error");
                    }
                    //
                    // If log, then log the failed execution.
                    //
                    if (settings.Log && this.LogToEvents)
                    {
                        try
                        {
                            manifest.FlowStatus = FlowStatus.ErrorPublisher;

                            using (MessageQueue mqLog = new MessageQueue(settings.EventsMsmqPath))
                            {
                                mqLog.Formatter = new BinaryMessageFormatter();

                                System.Messaging.Message msgLog = new System.Messaging.Message();
                                msgLog.Formatter = new BinaryMessageFormatter();
                                msgLog.Body = manifest.ToXmlDocument().OuterXml;
                                msgLog.Label = manifest.FlowID;

                                mqLog.Send(msgLog);
                            }
                        }
                        catch (StackOverflowException)
                        {
                            throw;
                        }
                        catch (OutOfMemoryException)
                        {
                            throw;
                        }
                        catch (ThreadAbortException)
                        {
                            throw;
                        }
                        catch (Exception x3)
                        {
                            ExceptionManager.PublishException(new HostRuntimeException("Failure logging error to log.", x3), "Error");
                        }
                    }// if ( settings.Log ...
                }// if ( packet != null ...
            }// catch ( Exception x ...
            finally
            {
                Thread.CurrentThread.CurrentCulture = cultureInfo;
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 
        /// </summary>
        public string DataDirectory { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string ConfigDirectory { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public PublisherBindingConfig BindingConfig { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public Hashtable ApplicationCache { get; private set; }
        #endregion

       
    }// class
}// namespace
