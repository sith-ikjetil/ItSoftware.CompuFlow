using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using ItSoftware.ExceptionHandler;
namespace ItSoftware.CompuFlow.Common.IO
{
    public class FlowWatcher
    {
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootDirectory"></param>
        public FlowWatcher( string rootDirectory )
        {
            this.RootDirectory = rootDirectory;

            this.FileSystemWatcher = new FileSystemWatcher( rootDirectory );
            this.FileSystemWatcher.IncludeSubdirectories = true;
            this.FileSystemWatcher.Filter = "*.zip";
            this.FileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime;
            this.FileSystemWatcher.Renamed += new RenamedEventHandler( FileSystemWatcher_Renamed );
            this.FileSystemWatcher.Created += new FileSystemEventHandler( FileSystemWatcher_Created );
            this.FileSystemWatcher.Deleted += new FileSystemEventHandler( FileSystemWatcher_Deleted );
            this.FileSystemWatcher.Changed += new FileSystemEventHandler( FileSystemWatcher_Changed );
            this.FileSystemWatcher.EnableRaisingEvents = true;

            this.DirectorySystemWatcher = new FileSystemWatcher( rootDirectory );
            this.DirectorySystemWatcher.IncludeSubdirectories = true;
            this.DirectorySystemWatcher.NotifyFilter = NotifyFilters.DirectoryName;
            this.DirectorySystemWatcher.Deleted += new FileSystemEventHandler( DirectorySystemWatcher_Deleted );
            this.DirectorySystemWatcher.Renamed += new RenamedEventHandler(DirectorySystemWatcher_Renamed);
            this.DirectorySystemWatcher.EnableRaisingEvents = true;
        }
        #endregion

        #region Events
        public event EventHandler<FlowWatcherEventArgs> Add;
        public event EventHandler<FlowWatcherEventArgs> Remove;
        public event EventHandler<FlowWatcherRemoveChannelEventArgs> RemoveChannel;
        public event EventHandler<FlowWatcherRenameChannelEventArgs> RenameChannel;
        public event EventHandler<FlowWatcherRemoveFlowIDEventArgs> RemoveFlowID;
        public event EventHandler<FlowWatcherRenameFlowIDEventArgs> RenameFlowID;
        #endregion

        #region Private Properties
        /// <summary>
        /// FileSystemWatcher class. File notifications.
        /// </summary>
        private FileSystemWatcher FileSystemWatcher { get; set; }
        /// <summary>
        /// FileSystemWatcher class. Directory notifications.
        /// </summary>
        private FileSystemWatcher DirectorySystemWatcher { get; set; }
        #endregion

        #region Public Properties
        /// <summary>
        /// 
        /// </summary>
        public int RootDirectoryCount { get; private set; }
        /// <summary>
        /// RootDirectory backing field.
        /// </summary>
        private string m_rootDirectory;
        /// <summary>
        /// 
        /// </summary>
        public string RootDirectory
        {
            get
            {
                return m_rootDirectory;
            }
            private set
            {
                m_rootDirectory = value;
                string[] parts = value.Split( Path.DirectorySeparatorChar );
                if ( parts[parts.Length - 1] == string.Empty ) {
                    this.RootDirectoryCount = parts.Length - 1;
                }
                else {
                    this.RootDirectoryCount = parts.Length;
                }
            }
        }
        #endregion

        #region Private FileSystemWatcher Event Handlers
        /// <summary>
        /// Converts a renamed event to a remove and new event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileSystemWatcher_Renamed( object sender, RenamedEventArgs e )
        {
            if ( IsValidFile( e.OldFullPath ) ) {
                try {
                    bool bOldIsZip = ( Path.GetExtension( e.OldFullPath ).ToLower( ) == ".zip" );
                    bool bNewIsZip = ( Path.GetExtension( e.FullPath ).ToLower( ) == ".zip" );
                    if ( bOldIsZip && !bNewIsZip ) {
                        Flow flow = new Flow(e.OldFullPath);
                        if ( this.Remove != null ) {
                            this.Remove(this, new FlowWatcherEventArgs(flow));
                        }
                    }
                    else if ( !bOldIsZip && bNewIsZip ) {
                        Flow flow = new Flow( e.FullPath );
                        if ( this.Add != null ) {
                            this.Add( this, new FlowWatcherEventArgs( flow ) );
                        }
                    }
                    else if ( bOldIsZip && bNewIsZip ) {
                        Flow flow = new Flow(e.OldFullPath);
                        if ( this.Remove != null ) {
                            this.Remove(this, new FlowWatcherEventArgs(flow));
                        }
                        Flow flowNew = new Flow(e.FullPath);
                        if ( this.Add != null ) {
                            this.Add(this, new FlowWatcherEventArgs(flowNew));
                        }
                    }
                }
                catch ( OutOfMemoryException ) {
                    throw;
                }
                catch ( StackOverflowException ) {
                    throw;
                }
                catch ( ThreadAbortException ) {
                    throw;
                }
                catch ( Exception x ) {
                    string msg = string.Format( "Error while renaming the file {0} to {1}.", e.OldFullPath, e.FullPath );
                    ExceptionManager.PublishException( new FlowWatcherException( msg, x ), "Error" );
                }
            }
        }
        /// <summary>
        /// Converts a created event to an add event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileSystemWatcher_Created( object sender, FileSystemEventArgs e )
        {
            if ( IsValidFile( e.FullPath ) ) {
                try {
                    Flow flow = new Flow( e.FullPath );
                    if ( this.Add != null ) {
                        this.Add( this, new FlowWatcherEventArgs( flow ) );
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
                    FlowWatcherException rwe = new FlowWatcherException( "Error adding flow after filesystem created event", x );
                    ExceptionManager.PublishException(rwe, "Error");
                }
            }
        }
        /// <summary>
        /// Converts a deleted event to a remove event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileSystemWatcher_Deleted( object sender, FileSystemEventArgs e )
        {
            if ( IsValidFile( e.FullPath ) ) {
                try {
                    Flow flow = new Flow( e.FullPath );
                    if ( this.Remove != null ) {
                        this.Remove( this, new FlowWatcherEventArgs( flow ) );
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
                    FlowWatcherException rwe = new FlowWatcherException( "Error adding removing flow after filesystem deleted event", x );
                    ExceptionManager.PublishException(rwe, "Error");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileSystemWatcher_Changed( object sender, FileSystemEventArgs e )
        {
            if ( IsValidFile( e.FullPath ) ) {
                try {
                    Flow flow = new Flow(e.FullPath);
                    if ( this.Remove != null ) {
                        this.Remove(this, new FlowWatcherEventArgs(flow));
                    }
                    if ( this.Add != null ) {
                        this.Add(this, new FlowWatcherEventArgs(flow));
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
                    FlowWatcherException rwe = new FlowWatcherException( "Error removing/adding flow after filesystem changed event", x );
                    ExceptionManager.PublishException(rwe, "Error");
                }
            }
        }

        #endregion

        #region Private DirectorySystemWatcher Event Handlers
        /// <summary>
        /// If reporid directory remove flow. If channel directory remove channel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DirectorySystemWatcher_Deleted( object sender, FileSystemEventArgs e )
        {
            if ( IsValidDirectory( e.FullPath ) ) {
                try {
                    Flow flow = new Flow(Path.Combine(e.FullPath, "dummy.zip"));
                    FlowWatcherRemoveFlowIDEventArgs ea = new FlowWatcherRemoveFlowIDEventArgs(flow.FlowID, flow.Channel);
                    if ( this.RemoveFlowID != null ) {
                        this.RemoveFlowID( this, ea );
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
                    FlowWatcherException rwe = new FlowWatcherException( "Error removing flow after directory filesystem deleted event", x );
                    ExceptionManager.PublishException(rwe, "Error");
                }
            }
            else if ( IsValidChannelDirectory( e.FullPath ) ) {
                try {
                    string channel = Path.GetFileName( e.FullPath );
                    if ( this.RemoveChannel != null ) {
                        this.RemoveChannel( this, new FlowWatcherRemoveChannelEventArgs( channel ) );
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
                    FlowWatcherException rwe = new FlowWatcherException( "Error removing channel after directory filesystem deleted event", x );
                    ExceptionManager.PublishException(rwe, "Error");
                }
            }
        }
        /// <summary>
        /// If directory is renamed, delete old flow and add first zip file found under directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DirectorySystemWatcher_Renamed( object sender, RenamedEventArgs e )
        {
            if ( this.IsValidDirectory( e.FullPath ) ) {
                try
                {
                    Flow oldFlow = new Flow(Path.Combine(e.OldFullPath, "dummy.zip"));
                    Flow newFlow = new Flow(Path.Combine(e.FullPath, "dummy.zip"));
                    FlowWatcherRenameFlowIDEventArgs ea = new FlowWatcherRenameFlowIDEventArgs(oldFlow.FlowID, newFlow.FlowID, newFlow.Channel);
                    if (this.RenameFlowID != null)
                    {
                        this.RenameFlowID(this, ea);
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
                catch (Exception x)
                {
                    FlowWatcherException rwe = new FlowWatcherException("Error removing/adding flow after directory filesystem renamed event", x);
                    ExceptionManager.PublishException(rwe, "Error");
                }
            }
            else if ( this.IsValidChannelDirectory( e.FullPath ) ) {
                try {
                    FlowWatcherRenameChannelEventArgs rcea = new FlowWatcherRenameChannelEventArgs( Path.GetFileName( e.OldFullPath ), Path.GetFileName( e.FullPath ) );
                    if ( this.RenameChannel != null ) {
                        this.RenameChannel( this, rcea );
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
                    FlowWatcherException rwe = new FlowWatcherException( "Error renameing channel.", x );
                    ExceptionManager.PublishException(rwe, "Error");
                }
            }
        }
        #endregion

        #region Private Helper Methods
        /// <summary>
        /// Check if directory is 1 directory under root directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private bool IsValidDirectory( string directory )
        {
            string[] parts = directory.Split( Path.DirectorySeparatorChar );
            if ( parts.Length == this.RootDirectoryCount + 2 ) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Check if directory is 1 directory under root directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private bool IsValidChannelDirectory( string directory )
        {
            string[] parts = directory.Split( Path.DirectorySeparatorChar );
            if ( parts.Length == this.RootDirectoryCount + 1 ) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Check if file lies in a directory 1 directory under root directory.
        /// </summary>
        /// <param name="filnameFullPath"></param>
        /// <returns></returns>
        private bool IsValidFile( string filnameFullPath )
        {
            return IsValidDirectory( Path.GetDirectoryName( filnameFullPath ) );
        }
        #endregion
    }// class
}// namespace
