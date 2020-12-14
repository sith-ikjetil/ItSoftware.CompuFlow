using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ItSoftware.CompuFlow.Common.IO;
using ItSoftware.CompuFlow.Common.Status;
using ItSoftware.CompuFlow.Manifest;
namespace ItSoftware.CompuFlow.Common 
{
    public class TransparentFlow<TRealFlow, TSettings, THostRuntimeSettings>
        where TRealFlow : RealFlow<THostRuntimeSettings>
        where TSettings : IFlowSettings<THostRuntimeSettings>
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public TransparentFlow( )
        {
        }
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="flow"></param>
        public TransparentFlow( Flow flow, TSettings settings )
        {
            this.Flow = flow;
            this.Settings = settings;
        }
        #endregion

        #region Protected Properties
        private string m_directory;
        protected string Directory
        {
            get
            {
                return m_directory;
            }
            set
            {
                m_directory = value;
            }
        }
        private string m_guid;
        protected string GUID
        {
            get
            {
                return m_guid;
            }
            set
            {
                m_guid = value;
            }
        }
        private bool m_initialized;
        protected bool Initialized
        {
            get
            {
                return m_initialized;
            }
            set
            {
                m_initialized = value;
            }
        }   
        /// <summary>
        /// 
        /// </summary>
        protected TRealFlow RealFlow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected AppDomain AppDomain { get; set; }
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
        /// Adds an execution time to the a list. The list only maintains the last 10 items.
        /// </summary>
        /// <param name="ts"></param>
        private void AddExecutionTime( TimeSpan ts )
        {
            lock ( this ) {
                this.ExecutionTimes.Add( ts );

                while ( this.ExecutionTimes.Count > 10 ) {
                    this.ExecutionTimes.RemoveAt( 0 );
                }
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan LastExecutionTime
        {
            get
            {
                lock ( this ) {
                    if ( this.ExecutionTimes.Count > 0 ) {
                        return this.ExecutionTimes[this.ExecutionTimes.Count - 1];
                    }
                    return TimeSpan.Zero;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan MinExecutionTime
        {
            get
            {
                lock ( this ) {
                    TimeSpan tsCheck = TimeSpan.MaxValue;
                    foreach ( TimeSpan ts in this.ExecutionTimes ) {
                        if ( ts < tsCheck ) {
                            tsCheck = ts;
                        }
                    }
                    if ( tsCheck == TimeSpan.MaxValue ) {
                        return TimeSpan.Zero;
                    }

                    return tsCheck;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan MaxExecutionTime
        {
            get
            {
                lock ( this ) {
                    TimeSpan tsCheck = TimeSpan.MinValue;
                    foreach ( TimeSpan ts in this.ExecutionTimes ) {
                        if ( ts > tsCheck ) {
                            tsCheck = ts;
                        }
                    }
                    if ( tsCheck == TimeSpan.MinValue ) {
                        return TimeSpan.Zero;
                    }

                    return tsCheck;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan AvgExecutionTime
        {
            get
            {
                lock ( this ) {
                    long lTicks = 0;
                    long lCount = 0;
                    foreach ( TimeSpan ts in this.ExecutionTimes ) {
                        lTicks += ts.Ticks;
                        lCount++;
                    }

                    if ( lCount == 0 ) {
                        return TimeSpan.Zero;
                    }

                    long lAvgTicks = lTicks / lCount;

                    return new TimeSpan( lAvgTicks );
                }
            }
        }
        /// <summary>
        /// Settings backing field.
        /// </summary>
        private TSettings m_settings;
        /// <summary>
        /// Settings.
        /// </summary>
        public TSettings Settings
        {
            get
            {
                return m_settings;
            }
            set
            {
                m_settings = value;
            }
        }
        /// <summary>
        /// Flow.
        /// </summary>
        public Flow Flow { get; set; }
        /// <summary>
        /// Markes the flow for termination.
        /// </summary>
        public bool MarkedForTermination { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Terminated { get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Executes a flow. Only executes if it is not marked for termination and/or is terminated.
        /// </summary>
        /// <param name="flowManifest"></param>
        public virtual void Execute( FlowManifest flowManifest )
        {
            if ( this.MarkedForTermination || this.Terminated ) {
                return;
            }

            if ( !this.Initialized ) {
                this.Initialize( );
            }

            //
            // Execute flow.
            //            
            DateTime dtBefore = DateTime.Now;
            this.RealFlow.Execute( flowManifest.ToXmlDocument().OuterXml, Settings.ToHostRuntimeSettings() );
            DateTime dtAfter = DateTime.Now;
            this.AddExecutionTime( dtAfter - dtBefore );
            
        }
        /// <summary>
        /// Terminates the flow by unloading the appdomain.
        /// </summary>
        public virtual void Terminate( )
        {            
            if ( this.Terminated ) {
                return;
            }

            if ( this.Initialized ) {
                Uninitialize( );
            }

            this.Terminated = true;        
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StatusFlow GatherStatusInformation( )
        {
            StatusFlow sr = new StatusFlow( );
            sr.Filename = this.Flow.FilenameFullPath;
            sr.FlowID = this.Flow.FlowID;            
            sr.AvgExecutionTime = this.AvgExecutionTime;
            sr.MinExecutionTime = this.MinExecutionTime;
            sr.MaxExecutionTime = this.MaxExecutionTime;
            sr.LastExecutionTime = this.LastExecutionTime;            
            if ( !this.Terminated && this.RealFlow != null ) {
                sr.ProgressItems = this.RealFlow.GatherStatusInformation( );
            }
            return sr;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Initializes a flow.
        /// </summary>
        protected virtual void Initialize( )
        {
        
        }
        /// <summary>
        /// 
        /// </summary>
        protected virtual void Uninitialize( )
        {        
            try {
                AppDomain.Unload( this.AppDomain );
                this.AppDomain = null;
                this.RealFlow = null;
                this.Initialized = false;
            }
            catch ( StackOverflowException ) {
                throw;
            }
            catch ( OutOfMemoryException ) {
                throw;
            }
            catch ( Exception x ) {
                string msg = "Could not unload an appdomain.";
                if ( this.AppDomain != null ) {
                    msg = string.Format( "Could not unload appdomain {0}",this.AppDomain.FriendlyName );
                }                
                CompuFlowException rfe = new CompuFlowException( msg, x );
                throw rfe;
            }            
        }       
        #endregion
    }// class
}// namespace
