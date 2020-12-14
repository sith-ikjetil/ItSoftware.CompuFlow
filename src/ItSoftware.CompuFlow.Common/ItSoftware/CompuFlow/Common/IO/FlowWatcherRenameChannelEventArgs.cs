using System;
using System.Collections.Generic;
using System.Text;

namespace ItSoftware.CompuFlow.Common.IO
{
    /// <summary>
    /// Rename channel event args.
    /// </summary>
    public class FlowWatcherRenameChannelEventArgs : EventArgs
    {
        #region Constructor
        public FlowWatcherRenameChannelEventArgs( string oldChannel, string newChannel )
        {
            this.OldChannel = oldChannel;
            this.NewChannel = newChannel;
        }
        #endregion

        #region Properites
        /// <summary>
        /// OldChannel name.
        /// </summary>
        public string OldChannel { get; private set; }
        /// <summary>
        /// NewChannel name.
        /// </summary>
        public string NewChannel { get; private set; }
        #endregion
    }// class
}// namespace
