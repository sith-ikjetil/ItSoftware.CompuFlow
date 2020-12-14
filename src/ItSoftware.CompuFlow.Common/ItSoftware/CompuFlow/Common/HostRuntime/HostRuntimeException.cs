using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace ItSoftware.CompuFlow.Common.HostRuntime
{
    /// <summary>
    /// HostRuntimeException class.
    /// </summary>
    [Serializable]
    public class HostRuntimeException : Exception
    {
        public HostRuntimeException( )
        {
        }
        public HostRuntimeException( string msg )
            : base( msg )
        {
        }
        public HostRuntimeException( string msg, Exception inner )
            : base( msg, inner )
        {
        }
        protected HostRuntimeException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
    }// class
}// namespace
