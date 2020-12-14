using System;
using ItSoftware.CompuFlow.Manifest;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace ItSoftware.CompuFlow.Common.Status
{
    /// <summary>
    /// 
    /// </summary>
    public class StatusChannel
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public StatusChannel( )
        {
            this.Flows = new StatusFlowCollection( );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xn"></param>
        public StatusChannel( XmlNode xn )
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
            
            XmlAttribute xaName = xn.Attributes["name"];
            if ( xaName != null ) {
                this.Name = xaName.InnerText;
            }

            XmlAttribute xaQueueCount = xn.Attributes["queueCount"];
            if ( xaQueueCount != null ) {
                this.QueueCount = Convert.ToInt32(xaQueueCount.InnerText);
            }

            XmlNode xnFlows = xn.SelectSingleNode( "flow" );
            if ( xnFlows != null ) {
                this.Flows = new StatusFlowCollection( xnFlows );
            }

            XmlNode xnCurrentFlowManifest = xn.SelectSingleNode( "currentFlowManifest" );
            if ( xnCurrentFlowManifest != null ) {
                XmlDocument xdManifest = new XmlDocument();
                xdManifest.LoadXml(xnCurrentFlowManifest.InnerText);
                this.CurrentFlowManifest = new FlowManifest( xdManifest );
            }

            XmlNode xnQueue = xn.SelectSingleNode( "queue" );
            if ( xnQueue != null ) {
                XmlNodeList xnl = xnQueue.SelectNodes( "item" );
                this.Queue = new FlowManifest[xnl.Count];
                for ( int i = 0; i < this.Queue.Length; i++ ) {
                    XmlDocument xdManifest = new XmlDocument();
                    xdManifest.LoadXml(xnl[i].InnerText);
                    this.Queue[i] = new FlowManifest( xdManifest );
                }
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int QueueCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FlowManifest CurrentFlowManifest { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FlowManifest[] Queue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public StatusFlowCollection Flows { get; set; }
        #endregion

        #region Public Methods
        public XmlNode SerializeToXml( XmlDocument xd )
        {
            XmlElement xeChannel = xd.CreateElement( "channel" );

            XmlAttribute xaName = xd.CreateAttribute( "name" );
            if ( this.Name != null ) {
                xaName.InnerText = this.Name;
            }
            xeChannel.Attributes.Append( xaName );

            XmlAttribute xaQueueCount = xd.CreateAttribute( "queueCount" );
            xaQueueCount.InnerText = this.QueueCount.ToString( );
            xeChannel.Attributes.Append( xaQueueCount );

            if ( this.CurrentFlowManifest != null ) {
                XmlElement xeCurrentFlowManifest = xd.CreateElement( "currentFlowManifest" );
                xeCurrentFlowManifest.InnerText = this.CurrentFlowManifest.ToXmlDocument().OuterXml;
                xeChannel.AppendChild( xeCurrentFlowManifest );
            }

            if ( this.Queue != null ) {
                XmlElement xeQueue = xd.CreateElement( "queue" );
                xeChannel.AppendChild( xeQueue );

                foreach ( FlowManifest flowManifest in this.Queue ) {
                    XmlElement xeItem = xd.CreateElement( "item" );
                    xeItem.InnerText = flowManifest.ToXmlDocument().OuterXml;
                    xeQueue.AppendChild( xeItem );
                }
            }

            if ( this.Flows != null ) {
                xeChannel.AppendChild( this.Flows.SerializeToXml( xd ) );                
            }

            return xeChannel;
        }
        #endregion
    }// class
}// namespace
