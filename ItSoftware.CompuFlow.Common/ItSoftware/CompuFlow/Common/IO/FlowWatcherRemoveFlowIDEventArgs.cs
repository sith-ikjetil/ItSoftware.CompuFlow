using System;
using System.Collections.Generic;
using System.Text;

namespace ItSoftware.CompuFlow.Common.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class FlowWatcherRemoveFlowIDEventArgs : EventArgs
    {
        #region Constructor
        public FlowWatcherRemoveFlowIDEventArgs(string flowID, string channel)
        {
            this.FlowID = flowID;
            this.Channel = channel;
        }
        #endregion

        #region Properites
        /// <summary>
        /// 
        /// </summary>
        public string FlowID { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Channel { get; private set; }
        #endregion
    }// class
}// namespace
