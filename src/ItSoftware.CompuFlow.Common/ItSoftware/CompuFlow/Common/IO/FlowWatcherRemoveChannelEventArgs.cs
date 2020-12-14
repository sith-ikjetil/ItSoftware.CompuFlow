using System;
using System.Collections.Generic;
using System.Text;

namespace ItSoftware.CompuFlow.Common.IO
{
    public class FlowWatcherRemoveChannelEventArgs : EventArgs
    {
        #region Constructor
        public FlowWatcherRemoveChannelEventArgs( string channel )
        {
            this.Channel = channel;
        }
        #endregion

        #region Properites
        private string m_channel;
        public string Channel
        {
            get
            {
                return m_channel;
            }
            private set
            {
                m_channel = value;
            }
        }
        #endregion
    }// class
}// namespace
