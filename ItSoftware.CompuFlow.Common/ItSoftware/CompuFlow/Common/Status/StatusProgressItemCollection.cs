using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace ItSoftware.CompuFlow.Common.Status
{
    [Serializable]
    public class StatusProgressItemCollection : List<StatusProgressItem>
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public StatusProgressItemCollection() {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xn"></param>
        public StatusProgressItemCollection(XmlNode xn) {
            if ( xn == null ) {
                throw new ArgumentNullException( "xn" );
            }
            this.DeserializeFromXml( xn );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pIList"></param>
        public StatusProgressItemCollection(IList<StatusProgressItem> pIList)
        {
            foreach (StatusProgressItem pi in pIList)
            {
                this.Add(pi);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xn"></param>
        private void DeserializeFromXml( XmlNode xn )
        {
            XmlNodeList xnlItems = xn.SelectNodes( "progressItem" );
            if ( xnlItems != null ) {
                foreach ( XmlNode node in xnlItems ){
                    this.Add( new StatusProgressItem(node) );
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
            if ( xd == null ) {
                throw new ArgumentNullException( "xd" );
            }

            XmlElement xeProgress = xd.CreateElement( "progressItems" );

            foreach ( StatusProgressItem item in this ) {
                xeProgress.AppendChild( item.SerializeToXml( xd ) );
            }

            return xeProgress;
        }
        #endregion
    }// class
}// namespace
