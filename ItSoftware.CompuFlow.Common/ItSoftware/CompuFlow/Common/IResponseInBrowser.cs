using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItSoftware.CompuFlow.Common
{
    public interface IResponseInBrowser
    {
        void WriteText(string content);

        void WriteHtml(string html);

        void WriteXml(string xml);

        void WriteFile(string flowID, byte[] content);

        void WriteFile(byte[] content, string filename, string contentType, string charset);
    }
}
