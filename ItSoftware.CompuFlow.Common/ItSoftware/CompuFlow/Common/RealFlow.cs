using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Lifetime;
using ItSoftware.CompuFlow.Common.Status;
using ItSoftware.CompuFlow.Common;
using ItSoftware.CompuFlow.Common.ResponseInBrowser.Contracts;
using ItSoftware.CompuFlow.Common.ResponseInBrowser.Proxies;
using ItSoftware.CompuFlow.Util;
using System.ServiceModel;
using ItSoftware.CompuFlow.Manifest;
namespace ItSoftware.CompuFlow.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="THostRuntimeSettings"></typeparam>
    public class RealFlow<THostRuntimeSettings> : MarshalByRefObject, IExecutionEngine, IResponseInBrowser
    {
        #region Constructors
        public RealFlow()
        {
            this.m_bLogToEvents = true;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 
        /// </summary>
        private StatusProgressItem m_progressItem = new StatusProgressItem( );
        /// <summary>
        /// 
        /// </summary>
        public StatusProgressItem ProgressItem
        {
            get
            {
                return m_progressItem;
            }
        }
        public FlowManifest CurrentExecutingFlowManifest { get; set; }

        #endregion

        #region Private Properties
        /// <summary>
        /// 
        /// </summary>
        private List<TimeSpan> m_executionTimes = new List<TimeSpan>( );
        /// <summary>
        /// 
        /// </summary>
        private List<TimeSpan> ExecutionTimes
        {
            get
            {
                return m_executionTimes;
            }
            set
            {
                m_executionTimes = value;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        private void CalculateAvgExecutionTime( )
        {
            long lTotalTicks = 0;
            long lCount = 0;
            for ( int i = 0; i < this.ExecutionTimes.Count; i++ ) {
                lTotalTicks += this.ExecutionTimes[i].Ticks;
                lCount++;
            }
            ProgressItem.AvgExecutionTime = new TimeSpan( lTotalTicks / lCount );
        }
        /// <summary>
        /// 
        /// </summary>
        private void CalculateMinExecutionTime( )
        {
            long lMinTicks = this.ExecutionTimes[0].Ticks;
            for ( int i = 1; i < this.ExecutionTimes.Count; i++ ) {
                if ( this.ExecutionTimes[i].Ticks < lMinTicks ) {
                    lMinTicks = this.ExecutionTimes[i].Ticks;
                }
            }
            ProgressItem.MinExecutionTime = new TimeSpan( lMinTicks );
        }
        /// <summary>
        /// 
        /// </summary>
        private void CalculateMaxExecutionTime( )
        {
            long lMaxTicks = this.ExecutionTimes[0].Ticks;
            for ( int i = 1; i < this.ExecutionTimes.Count; i++ ) {
                if ( this.ExecutionTimes[i].Ticks > lMaxTicks ) {
                    lMaxTicks = this.ExecutionTimes[i].Ticks;
                }
            }
            ProgressItem.MaxExecutionTime = new TimeSpan( lMaxTicks );
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        public void AddExecutionTime( TimeSpan ts )
        {
            lock ( this ) {
                this.ExecutionTimes.Add( ts );
                this.ProgressItem.ExecutionTime = ts;

                while ( this.ExecutionTimes.Count > 10 ) {
                    this.ExecutionTimes.RemoveAt( 0 );
                }

                CalculateAvgExecutionTime( );
                CalculateMinExecutionTime( );
                CalculateMaxExecutionTime( );
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StatusProgressItem GetProgressItem( )
        {
            StatusProgressItem progressItem = new StatusProgressItem( );
            progressItem.Description = this.ProgressItem.Description;
            progressItem.ErrorInfo = this.ProgressItem.ErrorInfo;
            progressItem.ExecutionTime = this.ProgressItem.ExecutionTime;
            progressItem.StartTime = this.ProgressItem.StartTime;
            progressItem.Status = this.ProgressItem.Status;
            progressItem.MinExecutionTime = this.ProgressItem.MinExecutionTime;
            progressItem.MaxExecutionTime = this.ProgressItem.MaxExecutionTime;
            progressItem.AvgExecutionTime = this.ProgressItem.AvgExecutionTime;
            return progressItem;
        }
        #endregion

        #region Public Virtual Methods
        /// <summary>
        /// Initialize. Typically used to setup resolve routine on appdomain.
        /// </summary>
        public virtual void Initialize( )
        {
        }
        /// <summary>
        /// Execute flow.
        /// </summary>
        /// <param name="flowManifest"></param>
        /// <param name="settings"></param>
        public virtual void Execute( string flowManifest, THostRuntimeSettings settings )
        {
            this.CurrentExecutingFlowManifest = new FlowManifest(flowManifest);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual StatusProgressItemCollection GatherStatusInformation( )
        {
            return new StatusProgressItemCollection( this.m_statusInformation );
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService( )
        {
            //
            // Infinite lease time.
            //
            return null;
        }
        #endregion

        #region IExecutionEngine Members

        public virtual bool EndFlow
        {
            get; 
            set;
        }

        private bool m_bLogToEvents;
        public virtual bool LogToEvents
        {
            get
            {
                return m_bLogToEvents;
            }
            set
            {
                m_bLogToEvents = value;
            }
        }

        protected List<StatusProgressItem> m_statusInformation = new List<StatusProgressItem>();
        public void AddStatusInformation(StatusProgressItem pi)
        {
            if (!m_statusInformation.Contains(pi))
            {
                m_statusInformation.Add(pi);
            }
        }

        public void Zip(string directory, string[] filenames, string zipFilename)
        {
            ItSoftware.CompuFlow.Util.Zip.PackFilesIntoOne(directory, filenames, zipFilename);
        }


        public virtual IResponseInBrowser ResponseInBrowser
        {
            get 
            {
                return this as IResponseInBrowser;
            }
        }

        public string FormatException(Exception exception)
        {
            return Util.Formatter.FormatException(exception);
        }

        #endregion

        #region IResponseInBrowser Members

        public void WriteText(string content)
        {
            if (this.CurrentExecutingFlowManifest.HasReturnedResponseInBrowser)
            {
                return;
            }

            int retry = 10;
            RetryStage:
            try
            {
                ResponseInBrowserProxy proxy = new ResponseInBrowserProxy(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), new EndpointAddress("net.pipe://localhost/CompuFlow.Gateway." + this.CurrentExecutingFlowManifest.GUID.ToString()));
                proxy.WriteText(this.CurrentExecutingFlowManifest.GUID, content);
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                if (--retry >= 0)
                {
                    System.Threading.Thread.Sleep(1000);
                    goto RetryStage;
                }
                else
                {
                    throw;
                }
            }
            this.CurrentExecutingFlowManifest.HasReturnedResponseInBrowser = true;
        }

        public void WriteHtml(string html)
        {
            if (this.CurrentExecutingFlowManifest.HasReturnedResponseInBrowser)
            {
                return;
            }

            int retry = 10;
            RetryStage:
            try
            {
                ResponseInBrowserProxy proxy = new ResponseInBrowserProxy(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), new EndpointAddress("net.pipe://localhost/CompuFlow.Gateway." + this.CurrentExecutingFlowManifest.GUID.ToString()));  
                proxy.WriteHtml(this.CurrentExecutingFlowManifest.GUID, html);
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                if (--retry >= 0)
                {
                    System.Threading.Thread.Sleep(1000);
                    goto RetryStage;
                }
                else
                {
                    throw;
                }
            }
            this.CurrentExecutingFlowManifest.HasReturnedResponseInBrowser = true;
        }

        public void WriteXml(string xml)
        {
            if (this.CurrentExecutingFlowManifest.HasReturnedResponseInBrowser)
            {
                return;
            }

            int retry = 10;
        RetryStage:
            try
            {
                ResponseInBrowserProxy proxy = new ResponseInBrowserProxy(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), new EndpointAddress("net.pipe://localhost/CompuFlow.Gateway." + this.CurrentExecutingFlowManifest.GUID.ToString()));
                proxy.WriteXml(this.CurrentExecutingFlowManifest.GUID, xml);
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                if (--retry >= 0)
                {
                    System.Threading.Thread.Sleep(1000);
                    goto RetryStage;
                }
                else
                {
                    throw;
                }
            }
            this.CurrentExecutingFlowManifest.HasReturnedResponseInBrowser = true;
        }

        public void WriteFile(string flowID, byte[] content)
        {
            if (this.CurrentExecutingFlowManifest.HasReturnedResponseInBrowser)
            {
                return;
            }

            int retry = 10;
            RetryStage:
            try
            {
                ResponseInBrowserProxy proxy = new ResponseInBrowserProxy(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), new EndpointAddress("net.pipe://localhost/CompuFlow.Gateway." + this.CurrentExecutingFlowManifest.GUID.ToString()));
                proxy.WriteFile1(this.CurrentExecutingFlowManifest.GUID, this.CurrentExecutingFlowManifest.FlowID, content);
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                if (--retry >= 0)
                {
                    System.Threading.Thread.Sleep(1000);
                    goto RetryStage;
                }
                else
                {
                    throw;
                }
            }
            this.CurrentExecutingFlowManifest.HasReturnedResponseInBrowser = true;
        }

        public void WriteFile(byte[] content, string filename, string contentType, string charset)
        {
            if (this.CurrentExecutingFlowManifest.HasReturnedResponseInBrowser)
            {
                return;
            }

            int retry = 10;
            RetryStage:
            try
            {
                ResponseInBrowserProxy proxy = new ResponseInBrowserProxy(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), new EndpointAddress("net.pipe://localhost/CompuFlow.Gateway." + this.CurrentExecutingFlowManifest.GUID.ToString()));
                proxy.WriteFile2(this.CurrentExecutingFlowManifest.GUID, content, filename, contentType, charset);
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                if (--retry >= 0)
                {
                    System.Threading.Thread.Sleep(1000);
                    goto RetryStage;
                }
                else
                {
                    throw;
                }
            }
            this.CurrentExecutingFlowManifest.HasReturnedResponseInBrowser = true;
        }

        #endregion
    }// class
}// namespace
