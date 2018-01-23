using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ItSoftware.CompuFlow.Common.IO;
using ItSoftware.CompuFlow.Manifest;
using ItSoftware.CompuFlow.Common.Status;
using ItSoftware.ExceptionHandler;
namespace ItSoftware.CompuFlow.Common
{
    public class Channel<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings> 
        where TSettings : IFlowSettings<THostRuntimeSettings>
        where TTransparentFlow : TransparentFlow<TRealFlow,TSettings,THostRuntimeSettings>, new()
        where TRealFlow : RealFlow<THostRuntimeSettings>
    {
        #region Constructors
        /// <summary>
        /// Overloaded constructor.
        /// </summary>
        /// <param name="name"></param>
        public Channel( string name, TSettings settings, bool allowMultipleInstances )
        {
            if ( name == null ) {
                throw new ArgumentNullException( "name" );
            }

            this.AllowMultipleInstances = allowMultipleInstances;
            this.Name = name;
            this.Settings = settings;
            this.Flows = new List<TTransparentFlow>( );
            this.Queue = new Queue<FlowManifest>();

            this.WaitHandle = new AutoResetEvent( false );

            this.WorkerThread = new Thread( new ThreadStart( this.WorkerMethod ) );
            this.WorkerThread.Start( );

            this.Renamed += new EventHandler<FlowWatcherRenameChannelEventArgs>( Channel_Renamed );
        }        
        #endregion        

        #region Private Properties
        /// <summary>
        /// Auto reset event object.
        /// </summary>
        private AutoResetEvent WaitHandle { get; set; }
        /// <summary>
        /// Worker thread thread object.
        /// </summary>
        private Thread WorkerThread { get; set; }
        /// <summary>
        /// TransparentFlow list.
        /// </summary>
        private List<TTransparentFlow> Flows { get; set; }
        /// <summary>
        /// FlowManifest queue.
        /// </summary>
        private Queue<FlowManifest> Queue { get; set; }
        #endregion

        #region Public Properties
        /// <summary>
        /// 
        /// </summary>
        public bool AllowMultipleInstances { get; private set; }
        /// <summary>
        /// Settings.
        /// </summary>
        public TSettings Settings { get; private set; }
        /// <summary>
        /// Marked for termination flag.
        /// </summary>
        public bool MarkedForTermination { get; private set; }
        /// <summary>
        /// Current execution flow manifest.
        /// </summary>
        public FlowManifest CurrentFlowManifest { get; private set; }
        /// <summary>
        /// Terminated means the object has been killed.
        /// </summary>
        public bool Terminated { get; private set; }
        /// <summary>
        /// Name backing field.
        /// </summary>
        private string m_name;
        /// <summary>
        /// Channel name.
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                FlowWatcherRenameChannelEventArgs ea = new FlowWatcherRenameChannelEventArgs(this.Name, value);
                m_name = value;
                if ( this.Renamed != null ) {
                    this.Renamed( this, ea );
                }
            }
        }        
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a flow to the channel.
        /// </summary>
        /// <param name="flow"></param>
        public void AddFlow( Flow flow )
        {
            if ( this.MarkedForTermination || this.Terminated ) {
                return;
            }

            if ( flow == null ) {
                throw new ArgumentNullException( "flow" );
            }

            if ( flow.Channel != this.Name ) {
                string msg = string.Format("Flow added to channel '{0}' but flow belongs to channel '{1}'",this.Name,flow.Channel);
                throw new ArgumentException( msg, "flow" );
            }

            if ( !this.AllowMultipleInstances ) {
                this.RemoveFlowID( flow.FlowID );
            }

            lock ( this ) {                
                TTransparentFlow transparentFlow = new TTransparentFlow();
                transparentFlow.Flow = flow;
                transparentFlow.Settings = Settings;
                this.Flows.Add(transparentFlow);
            }
        }
        /// <summary>
        /// Finds the flow id.
        /// </summary>
        /// <param name="flowID"></param>
        /// <returns></returns>
        public bool IsInChannel( string flowID )
        {
            if ( this.MarkedForTermination || this.Terminated ) {
                return false;
            }

            lock ( this ) {
                foreach ( TTransparentFlow rr in this.Flows ) {
                    if ( rr.Flow.FlowID == flowID ) {
                        return true;
                    }
                }
            }
            return false;
        }        
        /// <summary>
        /// Mark all flows for termination.
        /// </summary>
        public void Terminate( )
        {
            if ( this.Terminated ) {
                return;
            }

            lock ( this ) {
                this.MarkedForTermination = true;
                foreach ( TTransparentFlow realFlow in this.Flows ) {
                    realFlow.MarkedForTermination = true;
                }
            }
            this.WaitHandle.Set( );            
        } 
        /// <summary>
        /// Mark flow for termination.
        /// </summary>
        /// <param name="flow"></param>
        public void RemoveFlow( Flow flow )
        {
            if ( this.MarkedForTermination || this.Terminated ) {
                return;
            }

            if (flow == null)
            {
                throw new ArgumentNullException( "flow" );
            }

            if (flow.Channel != this.Name)
            {
                string msg = string.Format( "Flow removed from channel '{0}' but flow belongs to channel '{1}'.", this.Name, flow.Channel );
                throw new ArgumentException( msg, "flow" );
            }

            lock ( this ) {
                foreach ( TTransparentFlow transparentFlow in this.Flows ) {
                    if (transparentFlow.Flow.FilenameFullPath == flow.FilenameFullPath)
                    {
                        transparentFlow.MarkedForTermination = true;
                    }
                    /*if ( transparentFlow.Flow.FlowID == flow.FlowID ) {
                        transparentFlow.MarkedForTermination = true;
                    }
                     * */
                }
            }
            this.WaitHandle.Set( );            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldFlowID"></param>
        /// <param name="newFlowID"></param>
        public void RenameFlowID(string oldFlowID, string newFlowID) {
            lock ( this ) {
                foreach ( TTransparentFlow transparentFlow in this.Flows ) {
                    if (transparentFlow.Flow.FlowID == oldFlowID)
                    {
                        transparentFlow.Flow.FlowID = newFlowID;
                        transparentFlow.Flow.FilenameFullPath = UpdateFlowIDFilePath( transparentFlow.Flow.FilenameFullPath, newFlowID );
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowID"></param>
        public void RemoveFlowID( string flowID )
        {            
            lock ( this ) {
                List<TTransparentFlow> removeList = new List<TTransparentFlow>( );
                foreach ( TTransparentFlow transparentFlow in this.Flows ) {
                    if ( transparentFlow.Flow.FlowID == flowID ) {
                        transparentFlow.MarkedForTermination = true;                        
                    }
                }                
            }// lock
            this.WaitHandle.Set( );
        }
        /// <summary>
        /// Appends a flow manifest to the threads work queue.
        /// </summary>
        /// <param name="flowManifest"></param>
        public void Execute( FlowManifest flowManifest )
        {
            if ( this.MarkedForTermination || this.Terminated ) {
                return;
            }

            this.Queue.Enqueue( flowManifest );
            this.WaitHandle.Set( );            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StatusChannel GatherStatusInformation( )
        {
            StatusChannel sc = new StatusChannel( );
            sc.Name = this.Name;
            lock (this) {
                sc.QueueCount = this.Queue.Count;            
                sc.Queue = this.Queue.ToArray( );
            }
            sc.CurrentFlowManifest = this.CurrentFlowManifest;
            lock ( this ) {
                foreach ( TTransparentFlow transparentFlow in this.Flows ) {
                    sc.Flows.Add( transparentFlow.GatherStatusInformation( ) );                    
                }                
            }

            return sc;
        }
        #endregion

        #region Private Events
        private event EventHandler<FlowWatcherRenameChannelEventArgs> Renamed; 
        #endregion

        #region Private Event Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="part"></param>
        /// <param name="indexFromRight"></param>
        /// <returns></returns>
        private string UpdateChannelFilePath( string path, string channel )
        {
            string[] pathParts = path.Split( System.IO.Path.DirectorySeparatorChar );
            pathParts[pathParts.Length - 3] = channel;
            StringBuilder newPath = new StringBuilder( );
            newPath.Append( pathParts[0] );
            for ( int i = 1; i < pathParts.Length; i++ ) {
                newPath.Append( System.IO.Path.DirectorySeparatorChar );
                newPath.Append( pathParts[i] );
            }         
            return newPath.ToString( );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        private string UpdateFlowIDFilePath( string path, string flowID )
        {
            string[] pathParts = path.Split( System.IO.Path.DirectorySeparatorChar );
            pathParts[pathParts.Length - 2] = flowID;
            StringBuilder newPath = new StringBuilder( );
            newPath.Append( pathParts[0] );
            for ( int i = 1; i < pathParts.Length; i++ ) {
                newPath.Append( System.IO.Path.DirectorySeparatorChar );
                newPath.Append( pathParts[i] );
            }
            return newPath.ToString( );
        }
        /// <summary>
        /// Update all paths.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Channel_Renamed( object sender, FlowWatcherRenameChannelEventArgs e )
        {
            foreach ( TTransparentFlow transparentFlow in this.Flows ) {
                transparentFlow.Flow.Channel = e.NewChannel;
                transparentFlow.Flow.FilenameFullPath = UpdateChannelFilePath( transparentFlow.Flow.FilenameFullPath, e.NewChannel );                
            }
        }
        #endregion

        #region Worker Method
        /// <summary>
        /// Worker method.
        /// </summary>
        private void WorkerMethod( )
        {
            while ( !this.MarkedForTermination ) {
                try {
                    this.WaitHandle.WaitOne( );
                    //
                    // Terminate flow marked and remove references to them.
                    //
                    lock ( this ) {
                        List<TTransparentFlow> removeList = new List<TTransparentFlow>( );
                        foreach ( TTransparentFlow transparentFlow in this.Flows ) {
                            if ( transparentFlow.MarkedForTermination && !transparentFlow.Terminated ) {
                                try {
                                    transparentFlow.Terminate( );
                                    //
                                    // Add the flow to list of flows to be removed.
                                    //
                                    removeList.Add( transparentFlow );
                                }
                                catch ( StackOverflowException ) {
                                    throw;
                                }
                                catch ( OutOfMemoryException ) {
                                    throw;
                                }
                                catch ( ThreadAbortException ) {
                                    throw;
                                }
                                catch ( Exception e ) {
                                    ExceptionManager.PublishException(e, "Error");
                                }
                            }
                        }
                        //
                        // Remove the flows.
                        //
                        foreach ( TTransparentFlow transparentFlow in removeList ) {
                            this.Flows.Remove( transparentFlow );
                        }
                    }// lock

                    //
                    // Execute flow.
                    //                
                    while ( this.Queue.Count > 0 ) {
                        FlowManifest flowManifest = this.Queue.Dequeue( );
                        bool bExecuted = false;
                        foreach ( TTransparentFlow transparentFlow in this.Flows ) {
                            if ( transparentFlow.Flow.FlowID == flowManifest.FlowID && !transparentFlow.MarkedForTermination ) {
                                try {
                                    this.CurrentFlowManifest = flowManifest;
                                    transparentFlow.Execute( flowManifest );
                                }
                                catch ( StackOverflowException ) {
                                    throw;
                                }
                                catch ( OutOfMemoryException ) {
                                    throw;
                                }
                                catch ( ThreadAbortException ) {
                                    throw;
                                }
                                catch ( Exception e ) {
                                    string msg = string.Format( "TransparentFlow threw an exception while executing flow with FlowID='{0}'.", flowManifest.FlowID );
                                    ExceptionManager.PublishException(e, "Error");
                                }
                                finally {
                                    bExecuted = true;
                                }
                            }// if
                        }// foreach
                        if ( !bExecuted ) {
                            string msg = string.Format( "No TransparentFlow to handle FlowID = '{0}'.", flowManifest.FlowID );
                            ExceptionManager.PublishException(new CompuFlowException(msg), "Warning");
                        }
                    }
                }
                catch ( StackOverflowException ) {
                    throw;
                }
                catch ( OutOfMemoryException ) {
                    throw;
                }
                catch ( ThreadAbortException tae ) {
                    CompuFlowException rfe = new CompuFlowException( "Could not continue execution.", tae );
                    ExceptionManager.PublishException(rfe, "Error");
                    this.Terminate( );
                    
                    //System.ServiceProcess.ServiceController sc = new System.ServiceProcess.ServiceController( "Psr3Engine" );
                    //sc.Stop( );                    
                }
                catch ( Exception x ) {
                    ExceptionManager.PublishException(x, "Error");
                }
            }// while

            //
            // Terminate all flows.
            //
            foreach ( TTransparentFlow transparentFlow in this.Flows ) {
                transparentFlow.Terminate( );
            }
            lock ( this ) {
                this.Flows.Clear( );
            }
            this.Terminated = true;
        }
        #endregion
        
    }// class
}// namespace
