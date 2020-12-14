using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;

   
    /// <summary>
    /// AsyncRequestState. State information used during async processing.
    /// </summary>
    public class AsyncResponseInBrowserRequestState : IAsyncResult
    {
        #region Public Properties
        private HttpContext m_httpContext;
        public HttpContext HttpContext
        {
            get
            {
                return m_httpContext;
            }
            private set
            {
                m_httpContext = value;
            }
        }
        #endregion

        #region Private Fields
        private AsyncCallback m_asyncCallback;
        private object m_state;
        private bool m_isCompleted;
        private ManualResetEvent m_callCompleteEvent;
        #endregion

        #region Constructors
        public AsyncResponseInBrowserRequestState(HttpContext httpContext, AsyncCallback asyncCallback, object state)
        {
            this.HttpContext = httpContext;
            this.m_asyncCallback = asyncCallback;
            this.m_state = state;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        internal void CompleteRequest()
        {
            this.m_isCompleted = true;
            lock (this)
            {
                if (m_callCompleteEvent != null)
                {
                    m_callCompleteEvent.Set();
                }
            }
            if (m_asyncCallback != null)
            {
                m_asyncCallback(this);
            }
        }
        #endregion

        #region IAsyncResult Members
        /// <summary>
        /// 
        /// </summary>
        public object AsyncState
        {
            get
            {
                return m_state;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool CompletedSynchronously
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return this.m_isCompleted;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                lock (this)
                {
                    if (m_callCompleteEvent == null)
                    {
                        m_callCompleteEvent = new ManualResetEvent(false);
                    }
                    return m_callCompleteEvent;
                }
            }
        }
        #endregion
    }// class

