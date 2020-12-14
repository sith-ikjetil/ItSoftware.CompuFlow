using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItSoftware.CompuFlow.Common.ResponseInBrowser
{
    public class ResponseInBrowserContext
    {
        public ResponseInBrowserContext(byte[] content, string flowID, string filename, string contentType, string charset)
        {
            this.Content = content;
            this.FlowID = flowID;
            this.Filename = filename;
            this.ContentType = contentType;
            this.Charset = charset;
        }

        public byte[] Content { get; private set; }
        public string FlowID { get; private set; }
        public string Filename { get; private set; }
        public string ContentType { get; private set; }
        public string Charset { get; private set; }
    }
}
