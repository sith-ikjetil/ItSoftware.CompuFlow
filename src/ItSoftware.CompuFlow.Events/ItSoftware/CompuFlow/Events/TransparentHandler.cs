using System;
using System.Threading;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ItSoftware.CompuFlow.Common;
using ItSoftware.CompuFlow.Util;
using ItSoftware.CompuFlow.Events.HostRuntime;

namespace ItSoftware.CompuFlow.Events
{
    public class TransparentHandler : TransparentFlow<RealHandler, Settings, HostRuntimeSettings>
    {
        //
        // Initialize the generator.
        //
        protected override void Initialize( )
        {
            this.GUID = Guid.NewGuid( ).ToString( );
            this.Directory = Path.Combine( Settings.TemporaryHandlerFilesDirectory, this.GUID );

            //
            // Unpack
            //
            try {
                System.IO.Directory.CreateDirectory( this.Directory );
                Zip.Unpack( this.Flow.FilenameFullPath, this.Directory );

                //
                // Verify Bin and Config directories.
                //
                if ( !System.IO.Directory.Exists( Path.Combine( this.Directory, "Bin" ) ) ) {
                    string msg = string.Format("The flow file {0} does not contain a Bin directory.", this.Flow.FilenameFullPath);
                    throw new HandlerException(msg);
                }
                if ( !System.IO.Directory.Exists( Path.Combine( this.Directory, "Config" ) ) ) {
                    string msg = string.Format("The flow file {0} does not contain a Config directory.", this.Flow.FilenameFullPath);
                    throw new HandlerException(msg);
                }
                /* Do not check for data directory, it does not have to bee there. data directory argument in interface then becomes null.
                if ( !System.IO.Directory.Exists( Path.Combine( this.Directory, "Data" ) ) ) {
                    string msg = string.Format( "The flow file {0} does not contain a Data directory.", this.Flow.FilenameFullPath );
                    throw new GeneratorException( msg );
                }
                 * */
                /* Do not check for config.xml file, it does not have to be there.
                if ( !File.Exists( Path.Combine( Path.Combine( this.Directory, "Config" ), "Config.xml" ) ) ) {
                    string msg = string.Format("The flow file {0} does not contain the required Config\\Config.xml file.", this.Flow.FilenameFullPath);
                    throw new RetrivalException( msg );
                }
                */
                if ( !File.Exists( Path.Combine( Path.Combine( this.Directory, "Config" ), "Binding.xml" ) ) ) {
                    string msg = string.Format("The flow file {0} does not contain the required Config\\Binding.xml file.", this.Flow.FilenameFullPath);
                    throw new HandlerException(msg);
                }
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
            catch ( Exception x ) {
                HandlerException ge = new HandlerException("Catastophics failure. Error while unpacking flow.", x);
                throw ge;
            }

            //
            // Copy needed assemblies.
            //            
            try {
                string[] files = new string[] { "ItSoftware.CompuFlow.Events.HostRuntime.dll", 
                                                "ItSoftware.ExceptionHandler.dll",
                                                "ItSoftware.CompuFlow.Manifest.dll",
                                                "ItSoftware.CompuFlow.Events.Interfaces.dll",
                                                "ItSoftware.CompuFlow.Util.dll",
                                                "ItSoftware.CompuFlow.Common.dll" };

                foreach ( string file in files ) {
                    string fromFile = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, file );
                    string toFile = Path.Combine( this.Directory, string.Format( "Bin\\{0}", file ) );
                    File.Copy( fromFile, toFile, true );
                }
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
            catch ( Exception x ) {
                HandlerException ae = new HandlerException("Catastrophic failure. Could not copy required host assemblies because one or more files was not found.", x);
                throw ae;
            }

            //
            // Fix Flow.config
            //
            try {
                string sourceFlowConfigFilename = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Flow.config" );
                string outputFlowConfigFilename = Path.Combine( this.Directory, "Flow.config" );
                if ( File.Exists( outputFlowConfigFilename ) ) {
                    //
                    // Modify existing Flow.config file.
                    //
                    XmlDocument xdSource = new XmlDocument( );
                    xdSource.Load( sourceFlowConfigFilename );

                    XmlDocument xdOutput = new XmlDocument( );
                    xdOutput.Load( outputFlowConfigFilename );

                    XmlDocument xdResult = FlowConfiguration.Merge( xdSource, xdOutput );
                    xdResult.Save( outputFlowConfigFilename );
                }
                else {
                    //
                    // Create new Flow.config file.
                    //
                    File.Copy( sourceFlowConfigFilename, outputFlowConfigFilename );
                }
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
            catch ( Exception x ) {
                HandlerException ae = new HandlerException("Catastrophic failure. Could not configure Flow.config.", x);
                throw ae;
            }

            //
            // Create app domain
            //
            try {
                AppDomainSetup ads = new AppDomainSetup( );
                ads.ApplicationBase = this.Directory;
                ads.ConfigurationFile = "Flow.config";
                ads.PrivateBinPath = "Bin";
                ads.PrivateBinPathProbe = "*";
                this.AppDomain = AppDomain.CreateDomain( this.Flow.FilenameFullPath, null, ads );
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
            catch ( Exception x ) {
                HandlerException ae = new HandlerException("Catastrophic failure. Could not create application domain.", x);
                throw ae;
            }

            //
            // Create type
            //
            try {
                this.RealFlow = (RealHandler)this.AppDomain.CreateInstanceAndUnwrap( "ItSoftware.CompuFlow.Events.HostRuntime", "ItSoftware.CompuFlow.Events.HostRuntime.RealHandler" );
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
            catch ( Exception x ) {
                AppDomain.Unload( this.AppDomain );
                this.AppDomain = null;
                HandlerException ae = new HandlerException("Could not create RealRetrival type in appdomain.", x);
                throw ae;
            }

            //
            // Mark initialization complete.
            //
            this.Initialized = true;
        }
    }// class
}// namespace
