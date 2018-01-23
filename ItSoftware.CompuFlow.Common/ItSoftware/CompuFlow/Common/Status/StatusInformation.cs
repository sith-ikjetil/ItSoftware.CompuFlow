using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace ItSoftware.CompuFlow.Common.Status
{
    /// <summary>
    /// 
    /// </summary>
    public class StatusInformation
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public StatusInformation( )
        {
            this.Channels = new StatusChannelCollection( );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xn"></param>
        public StatusInformation( XmlDocument xd )
        {
            if ( xd == null ) {
                throw new ArgumentNullException( "xd" );
            }
            this.DeserializeFromXml( xd );
        }
        #endregion
        
        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xd"></param>
        private void DeserializeFromXml( XmlDocument xd )
        {
            XmlNode xnChannels = xd.SelectSingleNode( "statusInformation/channels" );
            if ( xnChannels != null ) {
                this.Channels = new StatusChannelCollection( xnChannels );
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 
        /// </summary>
        private StatusChannelCollection m_channels;
        /// <summary>
        /// 
        /// </summary>
        public StatusChannelCollection Channels
        {
            get
            {
                return m_channels;
            }
            set
            {
                m_channels = value;
            }
        }
        #endregion

        #region Public Methods
        public XmlDocument SerializeToXml( )
        {
            XmlDocument xd = new XmlDocument();

            XmlDeclaration xdDecl = xd.CreateXmlDeclaration("1.0","utf-8",null);
            xd.AppendChild(xdDecl);

            XmlElement xeStatusInformation = xd.CreateElement( "statusInformation" );
            xd.AppendChild( xeStatusInformation );

            if ( this.Channels != null ) {
                xeStatusInformation.AppendChild( this.Channels.SerializeToXml( xd ) );
            }
            
            return xd;
        }
        #endregion
    }// class
}// namespace
