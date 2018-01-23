using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using ItSoftware.CompuFlow.Util;
using ItSoftware.CompuFlow.Common;
using ItSoftware.CompuFlow.Retrival.HostRuntime;
namespace ItSoftware.CompuFlow.Retrival
{
    [Serializable]
    public class Settings : HostRuntimeSettings, IFlowSettings<HostRuntimeSettings>
    {
        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        internal Settings( )
        {
            object data;
            string msg = "Missing setting '{0}' in app.config file.";            

            data = ConfigurationManager.AppSettings["SourceMsmqPath"];
            if ( data == null ) {
                throw new SettingsException( string.Format( msg, "SourceMsmqPath" ) );
            }
            this.SourceMsmqPath = Convert.ToString( data );

            data = ConfigurationManager.AppSettings["DestinationMsmqPath"];
            if ( data == null ) {
                throw new SettingsException( string.Format( msg, "DestinationMsmqPath" ) );
            }
            this.DestinationMsmqPath = Convert.ToString( data );

            data = ConfigurationManager.AppSettings["EventsMsmqPath"];
            if ( data == null ) {
                throw new SettingsException( string.Format( msg, "EventsMsmqPath" ) );
            }
            this.EventsMsmqPath = Convert.ToString( data );

            data = ConfigurationManager.AppSettings["RetrivalsDirectory"];
            if ( data == null ) {
                throw new SettingsException( string.Format( msg, "RetrivalsDirectory" ) );
            }
            this.RetrivalsDirectory = FileSystem.NormalizeDirectoryPath( Convert.ToString( data ) );

            data = ConfigurationManager.AppSettings["TempDirectory"];
            if (data == null)
            {
                throw new SettingsException(string.Format(msg, "TempDirectory"));
            }
            this.TempDirectory = FileSystem.NormalizeDirectoryPath(Convert.ToString(data));

            data = ConfigurationManager.AppSettings["TemporaryRetrivalFilesDirectory"];
            if ( data == null ) {
                throw new SettingsException(string.Format(msg, "TemporaryRetrivalFilesDirectory"));
            }
            this.TemporaryRetrivalFilesDirectory = FileSystem.NormalizeDirectoryPath( Convert.ToString( data ) );            

            data = ConfigurationManager.AppSettings["OutputDirectory"];
            if ( data == null ) {
                throw new SettingsException( string.Format( msg, "OutputDirectory" ) );
            }
            this.OutputDirectory = FileSystem.NormalizeDirectoryPath( Convert.ToString( data ) );

            data = ConfigurationManager.AppSettings["FailureDirectory"];
            if ( data == null ) {
                throw new SettingsException( string.Format( msg, "FailureDirectory" ) );
            }
            this.FailureDirectory = FileSystem.NormalizeDirectoryPath( Convert.ToString( data ) );

            
            data = ConfigurationManager.AppSettings["Log"];
            if ( data == null ) {
                throw new SettingsException( string.Format( msg, "Log" ) );
            }
            this.Log = Convert.ToBoolean( data );

            ValidateMsmqSettings( );
            ValidateDirectorySettings( );
        }
        #endregion

        #region Properites
        /// <summary>
        /// 
        /// </summary>
        public string SourceMsmqPath { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string TemporaryRetrivalFilesDirectory { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string RetrivalsDirectory { get; protected set; }
        #endregion

        #region Private Methods
        private void ValidateMsmqSettings( )
        {
            string msg = "Msmq path '{0}' does not exist.";
            if ( !System.Messaging.MessageQueue.Exists( this.SourceMsmqPath ) ) {
                throw new SettingsException( string.Format( msg, "SourceMsmqPath" ) );
            }
            if ( !System.Messaging.MessageQueue.Exists( this.DestinationMsmqPath ) ) {
                throw new SettingsException( string.Format( msg, "DestinationMsmqPath" ) );
            }
            if ( !System.Messaging.MessageQueue.Exists( this.EventsMsmqPath ) ) {
                throw new SettingsException( string.Format( msg, "EventsMsmqPath" ) );
            }
        }
        private void ValidateDirectorySettings( )
        {
            string msg = "Directory '{0}' does not exist.";
            if ( !System.IO.Directory.Exists( this.FailureDirectory ) ) {
                throw new SettingsException( string.Format( msg, "FailureDirectory" ) );
            }
            if ( !System.IO.Directory.Exists( this.OutputDirectory ) ) {
                throw new SettingsException( string.Format( msg, "OutputDirectory" ) );
            }
            if ( !System.IO.Directory.Exists( this.RetrivalsDirectory ) ) {
                throw new SettingsException( string.Format( msg, "RetrivalDirectory" ) );
            }            
            if ( !System.IO.Directory.Exists( this.TemporaryRetrivalFilesDirectory ) ) {
                throw new SettingsException( string.Format( msg, "TemporaryRetrivalFilesDirectory" ) );
            }
            if (!System.IO.Directory.Exists(this.TempDirectory))
            {
                throw new SettingsException(string.Format(msg, "TempDirectory"));
            }
        }
        #endregion

        #region IFlowSettings<HostRuntimeSettings> Members
        /// <summary>
        /// Create and return a HostRuntimeSettings object.
        /// </summary>
        /// <returns></returns>
        public HostRuntimeSettings ToHostRuntimeSettings( )
        {
            HostRuntimeSettings settings = new HostRuntimeSettings( );
            settings.DestinationMsmqPath = this.DestinationMsmqPath;
            settings.FailureDirectory = this.FailureDirectory;
            settings.Log = this.Log;
            settings.EventsMsmqPath = this.EventsMsmqPath;
            settings.OutputDirectory = this.OutputDirectory;
            settings.TempDirectory = this.TempDirectory;
            return settings;
        }        
        /// <summary>
        /// Parent directory of channels.
        /// </summary>
        public string FlowDirectory
        {
            get {
                return this.RetrivalsDirectory;
            }
        }
        #endregion
    }// class
}// namespace
