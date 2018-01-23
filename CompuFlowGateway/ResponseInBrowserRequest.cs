using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;
using System.Net;
using Microsoft.Win32.SafeHandles;
using ItSoftware.ExceptionHandler;
using ItSoftware.CompuFlow.Common.ResponseInBrowser.Hosts;
using ItSoftware.CompuFlow.Util;
/// <summary>
/// ResponseInBrowserRequest. Handles the ResponseInBrowser request.
/// </summary>
public class ResponseInBrowserRequest
{
    #region Constructors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="asyncRequestState"></param>
    public ResponseInBrowserRequest( AsyncResponseInBrowserRequestState asyncRequestState )
    {
        this.RequestState = asyncRequestState;
    }
    #endregion

    #region Public Properties
    private AsyncResponseInBrowserRequestState m_asyncRequestState;
    public AsyncResponseInBrowserRequestState RequestState
    {
        get
        {
            return m_asyncRequestState;
        }
        private set
        {
            m_asyncRequestState = value;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 
    /// </summary>
    public void ProcessRequest( )
    {
        try {
            //
            // Create and read from named pipe.
            //

            if (this.RequestState.HttpContext.Request.QueryString["guid"] == null)
            {
                this.RequestState.HttpContext.Response.Write("Missing required parameter: guid");
                return;
            }

            string guidString = this.RequestState.HttpContext.Request.QueryString["guid"];

            if (string.IsNullOrEmpty(guidString) || string.IsNullOrWhiteSpace(guidString))
            {
                this.RequestState.HttpContext.Response.Write("Invalid parameter: guid");
                return;
            }

            Guid guid;
            try
            {
                guid = Guid.Parse(guidString);
            }
            catch (FormatException)
            {
                this.RequestState.HttpContext.Response.Write("Invalid format parameter: guid");
                return;
            }

            ResponseInBrowserHost host = new ResponseInBrowserHost(this.RequestState.HttpContext, "net.pipe://localhost/CompuFlow.Gateway." + guid.ToString());
            host.Start(guid);
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
            ExceptionManager.PublishException( new Exception( "An error occured in ResponseInBrowser handler.", x ), "Error" );

            this.RequestState.HttpContext.Response.ClearHeaders( );
            this.RequestState.HttpContext.Response.AddHeader( "Content-Type", "text/html" );
            this.RequestState.HttpContext.Response.Clear( );
            this.RequestState.HttpContext.Response.Write( "<html>" );
            this.RequestState.HttpContext.Response.Write( "<head><title>ItSoftware - CompuFlow - Exception Information</title></head>");
            this.RequestState.HttpContext.Response.Write( "<body style=\"font-family:tahoma, verdana;font-size:9pt\">" );
            this.RequestState.HttpContext.Response.Write("<h3>An error occured in ResponseInBrowser handler.</h3><br><br>Exception information:<br>");
            this.RequestState.HttpContext.Response.Write( "<textarea cols=80 rows=30 style=\"font-family:tahoma,verdana;font-size:9pt\">" );
            this.RequestState.HttpContext.Response.Write( Formatter.FormatException(x) );
            this.RequestState.HttpContext.Response.Write( "</textarea>" );
            this.RequestState.HttpContext.Response.Write( "</body>" );
            this.RequestState.HttpContext.Response.Write( "</html>" );
            this.RequestState.HttpContext.Response.End( );
        }

        //
        // Complete the request.
        //
        m_asyncRequestState.CompleteRequest( );
    }
    #endregion
}// class
