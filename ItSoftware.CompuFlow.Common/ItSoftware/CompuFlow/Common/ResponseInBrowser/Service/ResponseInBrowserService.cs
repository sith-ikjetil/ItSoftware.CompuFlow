using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ItSoftware.CompuFlow.Common.ResponseInBrowser.Contracts;
using System.Web;
using ItSoftware.ExceptionHandler;
using ItSoftware.CompuFlow.Common.ResponseInBrowser;
namespace ItSoftware.CompuFlow.Common.ResponseInBrowser.Services
{
    public class ResponseInBrowserService : ItSoftware.CompuFlow.Common.ResponseInBrowser.Contracts.IResponseInBrowser
    {
        public static Dictionary<Guid, HttpContext> Contexts { get; set; }
        public static Dictionary<Guid, ResponseInBrowserContext> FinishedContexts { get; set; }
        
        static ResponseInBrowserService()
        {
            ResponseInBrowserService.Contexts = new Dictionary<Guid, HttpContext>();
            ResponseInBrowserService.FinishedContexts = new Dictionary<Guid, ResponseInBrowserContext>();
        }

        
        #region IResponseInBrowser Members

        public void WriteText(Guid guid, string content)
        {
            bool guidOK = false;
            HttpContext ctx = null;
            try
            {
                ctx = ResponseInBrowserService.Contexts[guid];
                if (ctx == null)
                {
                    throw new ArgumentOutOfRangeException("guid");
                }
                guidOK = true;
                ResponseInBrowserContext ribContext = new ResponseInBrowserContext(Encoding.UTF8.GetBytes(content),null, null,"text/plain", "utf-8");
                ResponseInBrowserService.FinishedContexts.Add(guid, ribContext );
            }
            catch (Exception x)
            {
                ExceptionManager.PublishException(x, "Error");
            }
            finally
            {
                if (guidOK)
                {
                    ResponseInBrowserService.Contexts.Remove(guid);
                }
            }
        }

        public void WriteHtml(Guid guid, string html)
        {
            bool guidOK = false;
            HttpContext ctx = null;
            try
            {
                ctx = ResponseInBrowserService.Contexts[guid];
                if (ctx == null)
                {
                    throw new ArgumentOutOfRangeException("guid");
                }
                guidOK = true;
                ResponseInBrowserContext ribContext = new ResponseInBrowserContext(Encoding.UTF8.GetBytes(html), null, null, "text/html", "utf-8");
                ResponseInBrowserService.FinishedContexts.Add(guid, ribContext);
            }
            catch (Exception x)
            {
                ExceptionManager.PublishException(x, "Error");
            }
            finally
            {
                if (guidOK)
                {
                    ResponseInBrowserService.Contexts.Remove(guid);
                }
            }
        }

        public void WriteXml(Guid guid, string xml)
        {
            bool guidOK = false;
            HttpContext ctx = null;
            try
            {
                ctx = ResponseInBrowserService.Contexts[guid];
                if (ctx == null)
                {
                    throw new ArgumentOutOfRangeException("guid");
                }
                guidOK = true;
                ResponseInBrowserContext ribContext = new ResponseInBrowserContext(Encoding.UTF8.GetBytes(xml), null, null, "text/xml", "utf-8");
                ResponseInBrowserService.FinishedContexts.Add(guid, ribContext);
            }
            catch (Exception x)
            {
                ExceptionManager.PublishException(x, "Error");
            }
            finally
            {
                if (guidOK)
                {
                    ResponseInBrowserService.Contexts.Remove(guid);
                }
            }
        }

        public void WriteFile1(Guid guid, string flowID, byte[] content)
        {
            bool guidOK = false;
            HttpContext ctx = null;
            try
            {
                ctx = ResponseInBrowserService.Contexts[guid];
                if (ctx == null)
                {
                    throw new ArgumentOutOfRangeException("guid");
                }
                guidOK = true;
                ResponseInBrowserContext ribContext = new ResponseInBrowserContext(content, flowID, null, null, null);
                ResponseInBrowserService.FinishedContexts.Add(guid, ribContext);
            }
            catch (Exception x)
            {
                ExceptionManager.PublishException(x, "Error");
            }
            finally
            {
                if (guidOK)
                {
                    ResponseInBrowserService.Contexts.Remove(guid);
                }
            }
        }

        public void WriteFile2(Guid guid, byte[] content, string filename, string contentType, string charset)
        {
            bool guidOK = false;
            HttpContext ctx = null;
            try
            {
                ctx = ResponseInBrowserService.Contexts[guid];
                if (ctx == null)
                {
                    throw new ArgumentOutOfRangeException("guid");
                }
                guidOK = true;
                ResponseInBrowserContext ribContext = new ResponseInBrowserContext(content, null, filename, contentType, charset);
                ResponseInBrowserService.FinishedContexts.Add(guid, ribContext);
            }
            catch (Exception x)
            {
                ExceptionManager.PublishException(x, "Error");
            }
            finally
            {
                if (guidOK)
                {
                    ResponseInBrowserService.Contexts.Remove(guid);
                }
            }
        }

        #endregion
    }
}
