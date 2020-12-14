using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ItSoftware.CompuFlow.Common.ResponseInBrowser.Contracts;
namespace ItSoftware.CompuFlow.Common.ResponseInBrowser.Proxies
{
    public class ResponseInBrowserProxy : ClientBase<ItSoftware.CompuFlow.Common.ResponseInBrowser.Contracts.IResponseInBrowser>, ItSoftware.CompuFlow.Common.ResponseInBrowser.Contracts.IResponseInBrowser
    {
        public ResponseInBrowserProxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress endpointAddress)
            : base(binding, endpointAddress)
        { }



        #region IResponseInBrowser Members

        public void WriteText(Guid guid, string content)
        {
            Channel.WriteText(guid, content); 
        }

        public void WriteHtml(Guid guid, string html)
        {
            Channel.WriteHtml(guid, html);
        }

        public void WriteXml(Guid guid, string xml)
        {
            Channel.WriteHtml(guid, xml);
        }

        public void WriteFile1(Guid guid, string flowID, byte[] content)
        {
            Channel.WriteFile1(guid, flowID, content);
        }

        public void WriteFile2(Guid guid, byte[] content, string filename, string contentType, string charset)
        {
            Channel.WriteFile2(guid, content, filename, contentType, charset);
        }

        #endregion
    }
}
