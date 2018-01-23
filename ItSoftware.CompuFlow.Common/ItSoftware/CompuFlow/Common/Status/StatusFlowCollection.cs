using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace ItSoftware.CompuFlow.Common.Status
{
    /// <summary>
    /// 
    /// </summary>
    public class StatusFlowCollection : List<StatusFlow>
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public StatusFlowCollection( )
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xn"></param>
        public StatusFlowCollection( XmlNode xn )
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

            XmlNodeList xnl = xn.SelectNodes( "flow" );
            foreach ( XmlNode node in xnl ) {
                this.Add( new StatusFlow( node ) );
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
            if ( xd == null ) {
                throw new ArgumentNullException( "xd" );
            }

            XmlElement xeFlows = xd.CreateElement( "flows" );
            foreach ( StatusFlow sr in this ) {
                xeFlows.AppendChild( sr.SerializeToXml( xd ) );
            }
            return xeFlows;
        }
        #endregion
    }// class
}// namespace
