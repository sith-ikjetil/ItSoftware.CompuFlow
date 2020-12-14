using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace ItSoftware.CompuFlow.Gateway.SectionHandlers
{
    public class ResponseInBrowserItem
    {
        public ResponseInBrowserItem(XmlNode section)
        {
            if ( section == null ) {
                throw new ArgumentNullException("section");
            }
            this.FlowID = section.Attributes["flowID"].InnerText;
            this.ContentType = section.Attributes["contentType"].InnerText;
            this.Filename = section.Attributes["filename"].InnerText;
            this.Charset = section.Attributes["charset"].InnerText;
        }

        public string FlowID { get; private set; }
        public string ContentType { get; private set; }
        public string Filename { get; private set; }
        public string Charset { get; private set; }
    }
}
