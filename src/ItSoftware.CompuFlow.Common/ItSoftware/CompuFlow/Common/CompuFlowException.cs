using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace ItSoftware.CompuFlow.Common
{
    [Serializable]
    public class CompuFlowException : Exception
    {
        public CompuFlowException( )
        {
        }
        public CompuFlowException( string msg )
            : base( msg )
        {
        }
        public CompuFlowException( string msg, Exception inner )
            : base( msg, inner )
        {
        }
        protected CompuFlowException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
    }// class
}// namespace
