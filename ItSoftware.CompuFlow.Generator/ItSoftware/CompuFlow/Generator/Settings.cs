using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using ItSoftware.CompuFlow.Util;
using ItSoftware.CompuFlow.Common;
using ItSoftware.CompuFlow.Generator.HostRuntime;
namespace ItSoftware.CompuFlow.Generator
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

            data = ConfigurationManager.AppSettings["GeneratorsDirectory"];
            if ( data == null ) {
                throw new SettingsException(string.Format(msg, "GeneratorsDirectory"));
            }
            this.GeneratorsDirectory = FileSystem.NormalizeDirectoryPath( Convert.ToString( data ) );

            data = ConfigurationManager.AppSettings["TempDirectory"];
            if (data == null)
            {
                throw new SettingsException(string.Format(msg, "TempDirectory"));
            }
            this.TempDirectory = FileSystem.NormalizeDirectoryPath(Convert.ToString(data));

            data = ConfigurationManager.AppSettings["TemporaryGeneratorFilesDirectory"];
            if ( data == null ) {
                throw new SettingsException(string.Format(msg, "TemporaryGeneratorFilesDirectory"));
            }
            this.TemporaryGeneratorFilesDirectory = FileSystem.NormalizeDirectoryPath( Convert.ToString( data ) );            

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
        public string TemporaryGeneratorFilesDirectory { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string GeneratorsDirectory { get; protected set; }
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
            if ( !System.IO.Directory.Exists( this.GeneratorsDirectory ) ) {
                throw new SettingsException( string.Format( msg, "RetrivalDirectory" ) );
            }            
            if ( !System.IO.Directory.Exists( this.TemporaryGeneratorFilesDirectory ) ) {
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
                return this.GeneratorsDirectory;
            }
        }
        #endregion
    }// class
}// namespace
