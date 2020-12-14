using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace ItSoftware.CompuFlow.Common.Status
{
    /// <summary>
    /// 
    /// </summary>
    public class StatusChannelCollection : List<StatusChannel>
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public StatusChannelCollection( )
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xn"></param>
        public StatusChannelCollection( XmlNode xn )
        {
            if ( xn == null ) {
                throw new ArgumentNullException( "xn" );
            }
            this.DeserializeFromXml( xn );
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xn"></param>
        private void DeserializeFromXml( XmlNode xn )
        {
            if ( xn == null ) {
                throw new ArgumentNullException( "xn" );
            }

            XmlNodeList xnlChannels = xn.SelectNodes( "channel" );
            if ( xnlChannels != null ) {
                foreach ( XmlNode node in xnlChannels ) {
                    this.Add( new StatusChannel( node ) );
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xd"></param>
        /// <returns></returns>
        public XmlNode SerializeToXml( XmlDocument xd )
        {
            XmlElement xe = xd.CreateElement( "channels" );

            foreach ( StatusChannel sc in this ) {
                xe.AppendChild( sc.SerializeToXml( xd ) );
            }

            return xe;
        }
        #endregion
    }// class
}// namespace
