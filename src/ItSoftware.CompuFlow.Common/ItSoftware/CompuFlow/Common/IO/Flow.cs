using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace ItSoftware.CompuFlow.Common.IO
{
    /// <summary>
    /// Flow. The central part of CompuFlow is the Flow. It is executed
    /// in the Retrival/Generator/Publisher and Events.
    /// </summary>
    public class Flow
    {
        #region Constructors
        public Flow( string filenameFullPath )
        {
            if ( filenameFullPath == null ) {
                throw new ArgumentNullException( "filenameFullPath" );
            }

            this.FilenameFullPath = filenameFullPath;
            this.Filename = Path.GetFileName( filenameFullPath );

            string directory = Path.GetDirectoryName( filenameFullPath );
            string[] parts = directory.Split( Path.DirectorySeparatorChar );
            if ( parts.Length >= 2 ) {
                this.FlowID = parts[parts.Length - 1];
                this.Channel = parts[parts.Length - 2];
            }
            else {
                throw new ArgumentException( "Invalid flow directory structure.", "filenameFullPath");
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Full path and filename of the flow file.
        /// </summary>
        public string FilenameFullPath { get; set; }
        /// <summary>
        /// Filename of flow file.
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// Flow ID.
        /// </summary>
        public string FlowID { get; set; }
        /// <summary>
        /// Channel name.
        /// </summary>
        public string Channel { get; set; }
        #endregion
    }// class
}// namespace
