using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItSoftware.CompuFlow.Common.Status.Services;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Description;
using ItSoftware.ExceptionHandler;
using ItSoftware.CompuFlow.Common.Status.Contracts;
namespace ItSoftware.CompuFlow.Common.Status.Hosts
{
    public class StatusInformationHost<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings>
        where TSettings : class, IFlowSettings<THostRuntimeSettings>
        where TTransparentFlow : TransparentFlow<TRealFlow, TSettings, THostRuntimeSettings>, new()
        where TRealFlow : RealFlow<THostRuntimeSettings>
    {
        private Thread HostThread { get; set; }
        private string EndpointAddress { get; set; }
        public StatusInformationHost(Controller<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings> controller, string endpointAddress)
        {
            if (string.IsNullOrEmpty(endpointAddress) || string.IsNullOrWhiteSpace(endpointAddress))
            {
                throw new ArgumentNullException("endpointAddress");
            }

            this.EndpointAddress = endpointAddress;
            StatusInformationService<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings>.Controller = controller;
            this.LockObject = new object();
            
            this.HostThread = new Thread(new ThreadStart(HostThreadWorkerMethod));
            this.HostThread.Start();
        }

        public void Stop()
        {
            if (this.HostThread.ThreadState == ThreadState.WaitSleepJoin)
            {
                //this.HostThread.Abort();
                Monitor.Enter(this.LockObject);
                Monitor.Pulse(this.LockObject);
                Monitor.Exit(this.LockObject);
            }
        }

        private object LockObject { get; set; }
        private void HostThreadWorkerMethod()
        {
            ServiceHost sh = null;
            bool bEntered = false;
            try
            {
                sh = new ServiceHost(typeof(StatusInformationService<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings>));
                {
                    sh.AddServiceEndpoint(typeof(IStatusInformation), new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), this.EndpointAddress);
                    sh.Open();

                    Monitor.Enter(this.LockObject);
                    bEntered = true;
                    Monitor.Wait(this.LockObject);
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception x)
            {
                ExceptionManager.PublishException(x, "Error");
            }
            finally
            {
                if ( sh != null ) {
                    sh.Close();
                }
                if (bEntered)
                {
                    Monitor.Exit(this.LockObject);
                }
            }
        }
    }
}
