using System;
using System.Collections.Generic;
using System.Text;

namespace ItSoftware.CompuFlow.Common.IO
{
    public class FlowWatcherEventArgs : EventArgs
    {
        #region Constructor
        public FlowWatcherEventArgs( Flow flow )
        {
            this.Flow = flow;
        }
        #endregion

        #region Properites
        /// <summary>
        /// Flow.
        /// </summary>
        public Flow Flow { get; private set; }
                        
        #endregion
    }// class
}// namespace
