using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace ItSoftware.CompuFlow.Retrival
{
    [Serializable]
    public class RetrivalException : Exception
    {
        public RetrivalException( )
        {
        }
        public RetrivalException( string msg )
            : base( msg )
        {
        }
        public RetrivalException( string msg, Exception inner )
            : base( msg, inner )
        {
        }
        protected RetrivalException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
    }// class
}// namespace
