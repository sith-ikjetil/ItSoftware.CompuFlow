<%@ WebHandler Language="C#" Class="ResponseInBrowserHandler" %>

using System;
using System.Web;
using ItSoftware.CompuFlow.Common.ResponseInBrowser.Hosts;
using System.Threading;

public class ResponseInBrowserHandler : IHttpAsyncHandler
{
    /// <summary>
    /// You will need to configure this handler in the web.config file of your 
    /// web and register it with IIS before being able to use it. For more information
    /// see the following link: http://go.microsoft.com/?linkid=8101007
    /// </summary>
    #region IHttpHandler Members

    public bool IsReusable
    {
        // Return false in case your Managed Handler cannot be reused for another request.
        // Usually this would be false in case you have some state information preserved per request.
        get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
    }
       

    #endregion

    #region IHttpAsyncHandler Members

    public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
    {
        AsyncResponseInBrowserRequestState ars = new AsyncResponseInBrowserRequestState( context, cb, extraData );
        ResponseInBrowserRequest ribr = new ResponseInBrowserRequest( ars );
        ThreadStart ts = new ThreadStart( ribr.ProcessRequest );
        Thread thread = new Thread( ts );
        thread.Start( );
        return ars;
    }

    public void EndProcessRequest(IAsyncResult result)
    {
           
    }

    #endregion
}
