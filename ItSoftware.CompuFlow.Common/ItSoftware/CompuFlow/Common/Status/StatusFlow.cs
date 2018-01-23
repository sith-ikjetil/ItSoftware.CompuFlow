using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace ItSoftware.CompuFlow.Common.Status
{
    /// <summary>
    /// 
    /// </summary>
    public class StatusFlow
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public StatusFlow( )
        {
            this.ProgressItems = new StatusProgressItemCollection( );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xn"></param>
        public StatusFlow( XmlNode xn )
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

            XmlAttribute xaFilename = xn.Attributes["filename"];
            if ( xaFilename != null ) {
                this.Filename = xaFilename.InnerText;
            }

            XmlAttribute xaFlowID = xn.Attributes["flowID"];
            if ( xaFlowID != null ) {
                this.FlowID = xaFlowID.InnerText;
            }

            XmlNode xnLastExecTime = xn.SelectSingleNode( "lastExecTime" );
            this.LastExecutionTime = TimeSpan.Parse( xnLastExecTime.InnerText );

            XmlNode xnMinExecTime = xn.SelectSingleNode( "minExecTime" );
            this.MinExecutionTime = TimeSpan.Parse( xnMinExecTime.InnerText );

            XmlNode xnMaxExecTime = xn.SelectSingleNode( "maxExecTime" );
            this.MaxExecutionTime = TimeSpan.Parse( xnMaxExecTime.InnerText );

            XmlNode xnAvgExecTime = xn.SelectSingleNode( "avgExecTime" );
            this.AvgExecutionTime = TimeSpan.Parse( xnAvgExecTime.InnerText );

            XmlNode xnProgress = xn.SelectSingleNode( "progressItems" );
            if ( xnProgress != null ) {
                this.ProgressItems = new StatusProgressItemCollection( xnProgress );
            }            
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 
        /// </summary>
        public string FlowID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private string m_filename;
        public string Filename
        {
            get
            {
                return m_filename;
            }
            set
            {
                m_filename = value;
            }
        }
        private StatusProgressItemCollection m_progressItems;
        public StatusProgressItemCollection ProgressItems
        {
            get
            {
                return m_progressItems;
            }
            set
            {
                m_progressItems = value;
            }
        }
        private TimeSpan m_lastExecutionTime = TimeSpan.Zero;
        public TimeSpan LastExecutionTime
        {
            get
            {
                return m_lastExecutionTime;
            }
            set
            {
                m_lastExecutionTime = value;
            }
        }
        private TimeSpan m_minExecutionTime = TimeSpan.Zero;
        public TimeSpan MinExecutionTime
        {
            get
            {
                return m_minExecutionTime;
            }
            set
            {
                m_minExecutionTime = value;
            }
        }
        private TimeSpan m_maxExecutionTime = TimeSpan.Zero;
        public TimeSpan MaxExecutionTime
        {
            get
            {
                return m_maxExecutionTime;
            }
            set
            {
                m_maxExecutionTime = value;
            }
        }
        private TimeSpan m_avgExecutionTime = TimeSpan.Zero;
        public TimeSpan AvgExecutionTime
        {
            get
            {
                return m_avgExecutionTime;
            }
            set
            {
                m_avgExecutionTime = value;
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
            XmlElement xeFlow = xd.CreateElement( "flow" );

            XmlAttribute xaFilename = xd.CreateAttribute( "filename" );
            if ( this.Filename != null ) {
                xaFilename.InnerText = this.Filename;
            }
            xeFlow.Attributes.Append( xaFilename );

            XmlAttribute xaFlowID = xd.CreateAttribute( "flowID" );
            if ( this.FlowID != null ) {
                xaFlowID.InnerText = this.FlowID;
            }
            xeFlow.Attributes.Append( xaFlowID );

            XmlElement xeLastExecTime = xd.CreateElement( "lastExecTime" );
            xeLastExecTime.InnerText = this.LastExecutionTime.ToString( );
            xeFlow.AppendChild( xeLastExecTime );

            XmlElement xeMinExecTime = xd.CreateElement( "minExecTime" );
            xeMinExecTime.InnerText = this.MinExecutionTime.ToString( );
            xeFlow.AppendChild( xeMinExecTime );

            XmlElement xeMaxExecTime = xd.CreateElement( "maxExecTime" );
            xeMaxExecTime.InnerText = this.MaxExecutionTime.ToString( );
            xeFlow.AppendChild( xeMaxExecTime );

            XmlElement xeAvgExecTime = xd.CreateElement( "avgExecTime" );
            xeAvgExecTime.InnerText = this.AvgExecutionTime.ToString( );
            xeFlow.AppendChild( xeAvgExecTime );

            if ( this.ProgressItems != null ) {
                xeFlow.AppendChild( this.ProgressItems.SerializeToXml( xd ) );                
            }
            return xeFlow;
        }
        #endregion
    }// class
}// namespace
