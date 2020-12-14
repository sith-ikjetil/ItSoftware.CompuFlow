using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace ItSoftware.CompuFlow.Common.IO
{
    [Serializable]
    public class FlowWatcherException : Exception
    {
        public FlowWatcherException( )
        {
        }
        public FlowWatcherException( string msg )
            : base( msg )
        {
        }
        public FlowWatcherException( string msg, Exception inner )
            : base( msg, inner )
        {
        }
        protected FlowWatcherException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
    }// class
}// namespace
