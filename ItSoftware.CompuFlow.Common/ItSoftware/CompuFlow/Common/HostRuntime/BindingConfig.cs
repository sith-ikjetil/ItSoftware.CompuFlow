using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace ItSoftware.CompuFlow.Common.HostRuntime
{
    /// <summary>
    /// Binding configuration file.
    /// </summary>
    public class BindingConfig
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="xd"></param>
        public BindingConfig( XmlDocument xd )
        {
            if ( xd == null ) {
                throw new ArgumentNullException( "xd" );
            }
            try {
                XmlNode xnBinding = xd.SelectSingleNode( "/binding" );
                this.DisplayName = xnBinding.Attributes["displayName"].InnerText;
                this.Type = xnBinding.Attributes["type"].InnerText;
                this.Pooled = bool.Parse(xnBinding.Attributes["pooled"].InnerText);

                string[] typeParts = this.Type.Split( new char[] { '.' } );
                this.Description = string.Format( "{0}, {1}", this.DisplayName.Split( new char[] { ',' } )[0].Trim( ), typeParts[typeParts.Length - 1] );
            }
            catch ( OutOfMemoryException ) {
                throw;
            }
            catch ( StackOverflowException ) {
                throw;
            }
            catch ( ThreadAbortException ) {
                throw;
            }
            catch ( Exception x ) {
                throw new HostRuntimeException( "Invalid binding file.", x );
            }
        }
        /// <summary>
        /// Should the retrival/generator/publisher/handler be pooled.
        /// </summary>
        public bool Pooled { get; private set; }
        /// <summary>
        /// Display name of assembly.
        /// </summary>
        public string DisplayName { get; private set; }
        /// <summary>
        /// Type full name.
        /// </summary>
        public string Type { get; private set; }
        /// <summary>
        /// Description of binding.
        /// </summary>
        public string Description { get; private set; }
     }// class
}// namespace
