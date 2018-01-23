using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace ItSoftware.CompuFlow.Common
{
    [Serializable]
    public class SettingsException : Exception
    {
        public SettingsException( )
        {
        }
        public SettingsException( string msg )
            : base( msg )
        {
        }
        public SettingsException( string msg, Exception inner )
            : base( msg, inner )
        {
        }
        protected SettingsException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
    }// class
}// namespace
