using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItSoftware.CompuFlow.Common.Status.Services;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Description;
using ItSoftware.ExceptionHandler;
using ItSoftware.CompuFlow.Common.ResponseInBrowser.Contracts;
using ItSoftware.CompuFlow.Common.ResponseInBrowser.Services;
using System.Web;
using System.Xml;
using System.Configuration;
using ItSoftware.CompuFlow.Gateway.SectionHandlers;
namespace ItSoftware.CompuFlow.Common.ResponseInBrowser.Hosts
{
    public class ResponseInBrowserHost
    {
        private string EndpointAddress { get; set; }
        private HttpContext HttpContext { get; set; }
        public ResponseInBrowserHost(HttpContext context, string endpointAddress)
        {
            if (string.IsNullOrEmpty(endpointAddress) || string.IsNullOrWhiteSpace(endpointAddress))
            {
                throw new ArgumentNullException("endpointAddress");
            }

            this.EndpointAddress = endpointAddress;
            this.HttpContext = context;   
        }

        public bool NoHttpContextWrite { get; set; }

        private byte[] m_responseInBrowserData;
        public byte[] ResponseInBrowserData
        {
            get
            {
                return m_responseInBrowserData;
            }
            private set
            {
                m_responseInBrowserData = value;
            }
        }

        public void Start(Guid guid)
        {
            ServiceHost sh = null;
            bool bFinished = false;
            ResponseInBrowserService.Contexts.Add(guid, this.HttpContext);
            int countdown = 600; // 10 minutes before auto close.
            try
            {
                sh = new ServiceHost(typeof(ResponseInBrowserService));
                NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                binding.MaxReceivedMessageSize = 1024 * 1024 * 1024;
                binding.ReaderQuotas.MaxStringContentLength = 1024 * 1024 * 1024;
                binding.ReaderQuotas.MaxBytesPerRead = 1024 * 1024 * 1024;
                sh.AddServiceEndpoint(typeof(ItSoftware.CompuFlow.Common.ResponseInBrowser.Contracts.IResponseInBrowser), binding, this.EndpointAddress);
                sh.Open();

                ResponseInBrowserContext ribContext = null;
                do
                {
                    try
                    {
                        ribContext = ResponseInBrowserService.FinishedContexts[guid];
                    }
                    catch ( System.Collections.Generic.KeyNotFoundException)
                    {
                    }

                    if (ribContext != null)
                    {
                        ResponseInBrowserService.FinishedContexts.Remove(guid);

                        if (this.NoHttpContextWrite)
                        {
                            this.ResponseInBrowserData = ribContext.Content;
                            bFinished = true;
                        }
                        else
                        {
                            //
                            // Return content in browser
                            //
                            if (ribContext.FlowID == null)
                            {
                                if (ribContext.Content != null)
                                {
                                    if (ribContext.ContentType != null && !string.IsNullOrEmpty(ribContext.ContentType) && !string.IsNullOrWhiteSpace(ribContext.ContentType))
                                    {
                                        this.HttpContext.Response.ContentType = ribContext.ContentType;
                                    }

                                    if (ribContext.Charset != null && !string.IsNullOrEmpty(ribContext.Charset) && !string.IsNullOrWhiteSpace(ribContext.Charset))
                                    {
                                        this.HttpContext.Response.Charset = ribContext.Charset;
                                    }

                                    if (ribContext.Filename != null && !string.IsNullOrEmpty(ribContext.Filename) && !string.IsNullOrWhiteSpace(ribContext.Filename))
                                    {
                                        this.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + ribContext.Filename + "\"");
                                    }

                                    this.HttpContext.Response.BinaryWrite(ribContext.Content);
                                    this.HttpContext.ApplicationInstance.CompleteRequest();
                                }
                            }
                            else
                            {
                                if (ribContext.Content != null)
                                {
                                    ResponseInBrowserCollection ribc = ConfigurationManager.GetSection("responseInBrowser") as ResponseInBrowserCollection;
                                    ResponseInBrowserItem item = ribc[ribContext.FlowID];
                                    if (item != null)
                                    {
                                        if (item.ContentType != null && !string.IsNullOrEmpty(item.ContentType) && !string.IsNullOrWhiteSpace(item.ContentType))
                                        {
                                            this.HttpContext.Response.ContentType = item.ContentType;
                                        }

                                        if (item.Charset != null && !string.IsNullOrEmpty(item.Charset) && !string.IsNullOrWhiteSpace(item.Charset))
                                        {
                                            this.HttpContext.Response.Charset = item.Charset;
                                        }

                                        if (item.Filename != null && !string.IsNullOrEmpty(item.Filename) && !string.IsNullOrWhiteSpace(item.Filename))
                                        {
                                            this.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + item.Filename + "\"");
                                        }

                                        this.HttpContext.Response.BinaryWrite(ribContext.Content);
                                        this.HttpContext.ApplicationInstance.CompleteRequest();
                                    }
                                    else
                                    {
                                        this.HttpContext.Response.Write("FlowID=" + ribContext.FlowID + " has no ResponseInBrowser configuration in web config of CompuFlowGateway");
                                    }
                                }
                            }


                            //
                            // Indicate that we are finished.
                            //
                            bFinished = true;
                        }
                    }

                    if (!bFinished)
                    {
                        Thread.Sleep(1000);
                        if (--countdown <= 0)
                        {
                            bFinished = true;
                        }
                    }
                } while (!bFinished);
                
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception x)
            {
                ExceptionManager.PublishException(x, "Error");
            }
            finally
            {
                if ( sh != null && sh.State == CommunicationState.Opened) {
                    sh.Close();
                }
            }
        }
    }
}
