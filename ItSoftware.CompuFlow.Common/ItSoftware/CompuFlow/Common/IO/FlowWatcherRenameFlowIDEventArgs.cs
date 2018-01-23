using System;
using System.Collections.Generic;
using System.Text;

namespace ItSoftware.CompuFlow.Common.IO
{    
    /// <summary>
    /// 
    /// </summary>
    public class FlowWatcherRenameFlowIDEventArgs : EventArgs
    {
        #region Constructor
        public FlowWatcherRenameFlowIDEventArgs( string oldFlowID, string newFlowID, string channel )
        {
            this.OldFlowID = oldFlowID;
            this.NewFlowID = newFlowID;
            this.Channel = channel;
        }
        #endregion

        #region Properites
        /// <summary>
        /// Old flow id.
        /// </summary>
        public string OldFlowID { get; private set; }
        /// <summary>
        /// New flow id.
        /// </summary>
        public string NewFlowID { get; private set; }
        /// <summary>
        /// Channel name.
        /// </summary>
        public string Channel { get; private set; }
        #endregion
    }// class
}// namespace
