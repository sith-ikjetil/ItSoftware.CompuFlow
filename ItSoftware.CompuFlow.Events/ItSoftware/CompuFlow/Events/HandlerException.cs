using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace ItSoftware.CompuFlow.Events
{
    [Serializable]
    public class HandlerException : Exception
    {
        public HandlerException( )
        {
        }
        public HandlerException( string msg )
            : base( msg )
        {
        }
        public HandlerException( string msg, Exception inner )
            : base( msg, inner )
        {
        }
        protected HandlerException(SerializationInfo info, StreamingContext context)
            : base( info, context )
        {
        }
    }// class
}// namespace
