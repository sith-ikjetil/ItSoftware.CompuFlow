using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ItSoftware.CompuFlow.Common.IO;
using ItSoftware.CompuFlow.Manifest;
using ItSoftware.CompuFlow.Common.Status;
using ItSoftware.ExceptionHandler;
namespace ItSoftware.CompuFlow.Common
{
    public class Controller<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings>
        where TSettings : class, IFlowSettings<THostRuntimeSettings>
        where TTransparentFlow : TransparentFlow<TRealFlow, TSettings, THostRuntimeSettings>, new()
        where TRealFlow : RealFlow<THostRuntimeSettings>
    {        
        #region Constructor
        /// <summary>
        /// Overloaded constructor.
        /// </summary>
        /// <param name="settings"></param>
        public Controller( TSettings settings, bool allowMultipleInstances )
        {
            this.AllowMultipleInstances = allowMultipleInstances;
            this.Settings = settings;
            this.Channels = new Dictionary<string, Channel<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings>>( );

            ReadPreInstalledFlows( );

            this.FlowWatcher = new FlowWatcher( settings.FlowDirectory );
            this.FlowWatcher.Add += new EventHandler<FlowWatcherEventArgs>(FlowWatcher_Add);
            this.FlowWatcher.Remove += new EventHandler<FlowWatcherEventArgs>(FlowWatcher_Remove);
            this.FlowWatcher.RemoveChannel += new EventHandler<FlowWatcherRemoveChannelEventArgs>(FlowWatcher_RemoveChannel);
            this.FlowWatcher.RenameChannel += new EventHandler<FlowWatcherRenameChannelEventArgs>(FlowWatcher_RenameChannel);
            this.FlowWatcher.RemoveFlowID += new EventHandler<FlowWatcherRemoveFlowIDEventArgs>(FlowWatcher_RemoveFlowID);
            this.FlowWatcher.RenameFlowID += new EventHandler<FlowWatcherRenameFlowIDEventArgs>(FlowWatcher_RenameFlowID);
        }        
        #endregion
        
        #region Private Methods
        /// <summary>
        /// Get preinstalled flows.
        /// </summary>
        private void ReadPreInstalledFlows( )
        {            
            string[] channels = Directory.GetDirectories( this.Settings.FlowDirectory );
            foreach ( string channel in channels ) {
                string[] flowIDs = Directory.GetDirectories( channel );
                foreach ( string flowID in flowIDs ) {
                    string[] files = Directory.GetFiles( flowID, "*.zip" );
                    foreach( string file in files ) {
                        Flow flow = new Flow( file );
                        this.AddFlow(flow);
                        if ( !this.AllowMultipleInstances ) {
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Add a flow.
        /// </summary>
        /// <param name="flow"></param>
        private void AddFlow( Flow flow )
        {
            lock ( this ) {
                try {
                    Channel<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings> channel = this.Channels[ToChannelKey( flow.Channel )];
                    channel.AddFlow( flow );
                }
                catch ( KeyNotFoundException ) {
                    Channel<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings> channel = new Channel<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings>( flow.Channel, this.Settings, this.AllowMultipleInstances );
                    channel.AddFlow( flow );
                    this.Channels.Add( ToChannelKey( flow.Channel ), channel );
                }
            }
        }
        /// <summary>
        /// Remove a flow.
        /// </summary>
        /// <param name="flow"></param>
        private void RemoveFlow( Flow flow )
        {
            lock ( this ) {
                try {
                    Channel<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings> channel = this.Channels[ToChannelKey(flow.Channel)];
                    channel.RemoveFlow(flow);
                }
                catch ( KeyNotFoundException ) {
                }
            }
        }
        /// <summary>
        /// Remove an entire channel.
        /// </summary>
        /// <param name="name"></param>
        private void RemoveChannel( string name )
        {
            lock ( this ) {
                try {
                    string key = ToChannelKey( name );
                    Channel<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings> channel = this.Channels[key];
                    channel.Terminate( );
                    this.Channels.Remove( key );                    
                }
                catch ( KeyNotFoundException ) {
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="oldFlowID"></param>
        /// <param name="newFlowID"></param>
        private void RenameFlowID( string channel, string oldFlowID, string newFlowID ) 
        {
            lock ( this ) {
                try {
                    string key = ToChannelKey( channel );
                    this.Channels[key].RenameFlowID(oldFlowID,newFlowID);                                        
                }
                catch ( KeyNotFoundException ) {
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="flowID"></param>
        private void RemoveFlowID( string channel, string flowID )
        {
            lock ( this ) {
                try {
                    string key = ToChannelKey( channel );
                    this.Channels[key].RemoveFlowID(flowID);                    
                }
                catch ( KeyNotFoundException ) {
                }
            }
        }
        /// <summary>
        /// Rename a channel.
        /// </summary>
        /// <param name="oldChannel"></param>
        /// <param name="newChannel"></param>
        private void RenameChannel( string oldChannel, string newChannel )
        {
            lock ( this ) {
                try {
                    string oldKey = ToChannelKey( oldChannel );
                    Channel<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings> channel = this.Channels[oldKey];
                    channel.Name = newChannel;                    
                    this.Channels.Remove( oldKey );
                    this.Channels.Add( ToChannelKey( newChannel ), channel );
                }
                catch ( KeyNotFoundException ) {
                }
            }
        }
        /// <summary>
        /// Convert a channel name to a channel key.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        private string ToChannelKey( string channel )
        {
            return string.Format( "{0}_{1}", "CHANNEL", channel.ToUpper() );
        }
        #endregion

        #region Private FlowWatcher Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlowWatcher_RenameFlowID( object sender, FlowWatcherRenameFlowIDEventArgs e )
        {
            this.RenameFlowID( e.Channel, e.OldFlowID, e.NewFlowID );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlowWatcher_RemoveFlowID( object sender, FlowWatcherRemoveFlowIDEventArgs e )
        {
            this.RemoveFlowID( e.Channel, e.FlowID );
        }        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlowWatcher_RenameChannel( object sender, FlowWatcherRenameChannelEventArgs e )
        {
            this.RenameChannel( e.OldChannel, e.NewChannel );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlowWatcher_RemoveChannel( object sender, FlowWatcherRemoveChannelEventArgs e )
        {
            this.RemoveChannel( e.Channel );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlowWatcher_Remove( object sender, FlowWatcherEventArgs e )
        {
            this.RemoveFlow( e.Flow );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlowWatcher_Add( object sender, FlowWatcherEventArgs e )
        {
            this.AddFlow( e.Flow );
        }
        #endregion

        #region Private Properties
        private Dictionary<string, Channel<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings>> m_channels;
        private Dictionary<string, Channel<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings>> Channels
        {
            get
            {
                return m_channels;
            }
            set
            {
                m_channels = value;
            }
        }
        private FlowWatcher FlowWatcher { get; set; }
        #endregion

        #region Public Properties
        /// <summary>
        /// 
        /// </summary>
        public bool AllowMultipleInstances { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public TSettings Settings { get; private set; }        
        #endregion        

        #region Public Methods
        /// <summary>
        /// Executes a flow.
        /// </summary>
        /// <param name="flowManifest"></param>
        public void ExecuteFlow( FlowManifest flowManifest )
        {
            lock ( this ) {
                bool bExecuted = false;
                foreach ( string key in this.Channels.Keys ) {                    
                    Channel<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings> channel = this.Channels[key];
                    if ( channel.IsInChannel( flowManifest.FlowID ) ) {
                        channel.Execute( flowManifest );
                        bExecuted = true;   
                    }                   
                }
                if ( !bExecuted ) {
                    string msg = string.Format( "No TransparentFlow to handle FlowID = '{0}'.", flowManifest.FlowID );
                    ExceptionManager.PublishException(new CompuFlowException(msg), "Warning");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Stop( )
        {
            lock ( this ) {
                foreach ( string key in this.Channels.Keys ) {
                    Channel<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings> channel = this.Channels[key];
                    channel.Terminate( );
                }
                this.Channels.Clear( );
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StatusInformation GatherStatusInformation( )
        {
            StatusInformation si = new StatusInformation( );
            
            lock ( this ) {
                foreach ( string key in this.Channels.Keys ) {
                    Channel<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings> channel = this.Channels[key];
                    if ( !channel.Terminated ) {
                        si.Channels.Add( channel.GatherStatusInformation( ) );
                    }
                }
            }

            return si;
        }
        #endregion
    }// class
}// namespace
