using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace ItSoftware.CompuFlow.Publisher
{
    [Serializable]
    public class PublisherException : Exception
    {
        public PublisherException( )
        {
        }
        public PublisherException( string msg )
            : base( msg )
        {
        }
        public PublisherException( string msg, Exception inner )
            : base( msg, inner )
        {
        }
        protected PublisherException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
    }// class
}// namespace
