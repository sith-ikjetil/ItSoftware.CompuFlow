using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace ItSoftware.CompuFlow.Generator
{
    [Serializable]
    public class GeneratorException : Exception
    {
        public GeneratorException( )
        {
        }
        public GeneratorException( string msg )
            : base( msg )
        {
        }
        public GeneratorException( string msg, Exception inner )
            : base( msg, inner )
        {
        }
        protected GeneratorException(SerializationInfo info, StreamingContext context)
            : base( info, context )
        {
        }
    }// class
}// namespace
