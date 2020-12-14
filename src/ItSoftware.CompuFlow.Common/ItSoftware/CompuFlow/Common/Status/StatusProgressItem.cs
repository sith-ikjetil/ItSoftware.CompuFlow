using System;
using System.Collections.Generic;
using ItSoftware.CompuFlow.Manifest;
using System.Text;
using System.Xml;
namespace ItSoftware.CompuFlow.Common.Status
{
    /// <summary>
    /// RealFlow's progress item information.
    /// </summary>
    [Serializable]
    public class StatusProgressItem
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public StatusProgressItem( )
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xn"></param>
        public StatusProgressItem( XmlNode xn )
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

            XmlNode xnStatus = xn.SelectSingleNode( "status" );
            if ( xnStatus != null ) {
                this.Status = xnStatus.InnerText;
            }

            XmlNode xnErrorInfo = xn.SelectSingleNode( "errorInfo" );
            if ( xnErrorInfo != null ) {
                this.ErrorInfo = xnErrorInfo.InnerText;
            }

            XmlNode xnStartTime = xn.SelectSingleNode( "startTime" );
            if ( xnStartTime != null ) {
                this.StartTime = DateTime.Parse(xnStartTime.InnerText);
            }

            XmlNode xnDescription = xn.SelectSingleNode( "description" );
            if ( xnDescription != null ) {
                this.Description = xnDescription.InnerText;
            }

            XmlNode xnExecTime = xn.SelectSingleNode( "execTime" );
            if ( xnExecTime != null ) {
                this.ExecutionTime = TimeSpan.Parse(xnExecTime.InnerText);
            }

            XmlNode xnMinExecTime = xn.SelectSingleNode( "minExecTime" );
            if ( xnMinExecTime != null ) {
                this.MinExecutionTime = TimeSpan.Parse( xnMinExecTime.InnerText );
            }

            XmlNode xnAvgExecTime = xn.SelectSingleNode( "avgExecTime" );
            if ( xnAvgExecTime != null ) {
                this.AvgExecutionTime = TimeSpan.Parse( xnAvgExecTime.InnerText );
            }

            XmlNode xnMaxExecTime = xn.SelectSingleNode( "maxExecTime" );
            if ( xnMaxExecTime != null ) {
                this.MaxExecutionTime = TimeSpan.Parse( xnMaxExecTime.InnerText );
            }            
        }
        #endregion

        #region Public Properties
        private string m_status;
        public string Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
            }
        }
        private string m_errorInfo;
        public string ErrorInfo
        {
            get
            {
                return m_errorInfo;
            }
            set
            {
                m_errorInfo = value;
            }
        }
        private DateTime m_startTime = DateTime.MinValue;
        public DateTime StartTime
        {
            get
            {
                return m_startTime;
            }
            set
            {
                m_startTime = value;
            }
        }
        private TimeSpan m_executionTime = TimeSpan.Zero;
        public TimeSpan ExecutionTime
        {
            get
            {
                return m_executionTime;
            }
            set
            {
                m_executionTime = value;
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
        private string m_description;
        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
            }
        }        
        #endregion

        #region Public Methods
        /// <summary>
        /// Serialize object to XML.
        /// </summary>
        /// <param name="xd"></param>
        /// <returns></returns>
        public XmlNode SerializeToXml( XmlDocument xd )
        {
            XmlElement xeProgressItem = xd.CreateElement( "progressItem" );

            XmlElement xeStatus = xd.CreateElement( "status" );
            if ( !string.IsNullOrEmpty( this.Status ) ) {
                xeStatus.InnerText = this.Status;
            }
            xeProgressItem.AppendChild( xeStatus );

            XmlElement xeErrorInfo = xd.CreateElement( "errorInfo" );
            if ( !string.IsNullOrEmpty( this.ErrorInfo ) ) {
                xeErrorInfo.InnerText = this.ErrorInfo;
            }
            xeProgressItem.AppendChild( xeErrorInfo );

            XmlElement xeStartTime = xd.CreateElement( "startTime" );
            xeStartTime.InnerText = this.StartTime.ToString("s");
            xeProgressItem.AppendChild( xeStartTime );

            XmlElement xeDescription = xd.CreateElement( "description" );
            if ( !string.IsNullOrEmpty( this.Description ) ) {
                xeDescription.InnerText = this.Description;
            }
            xeProgressItem.AppendChild( xeDescription );

            XmlElement xeExecTime = xd.CreateElement( "execTime" );
            xeExecTime.InnerText  = this.ExecutionTime.ToString();
            xeProgressItem.AppendChild( xeExecTime );

            XmlElement xeMinExecTime = xd.CreateElement( "minExecTime" );
            xeMinExecTime.InnerText = this.MinExecutionTime.ToString( );
            xeProgressItem.AppendChild( xeMinExecTime );

            XmlElement xeAvgExecTime = xd.CreateElement( "avgExecTime" );
            xeAvgExecTime.InnerText = this.AvgExecutionTime.ToString( );
            xeProgressItem.AppendChild( xeAvgExecTime );

            XmlElement xeMaxExecTime = xd.CreateElement( "maxExecTime" );
            xeMaxExecTime.InnerText = this.MaxExecutionTime.ToString( );
            xeProgressItem.AppendChild( xeMaxExecTime );            
            
            return xeProgressItem;
        }
        #endregion
    }// class
}// namespace
