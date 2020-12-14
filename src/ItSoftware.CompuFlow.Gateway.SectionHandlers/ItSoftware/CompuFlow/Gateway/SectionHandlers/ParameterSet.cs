using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.Specialized;
namespace ItSoftware.CompuFlow.Gateway.SectionHandlers
{
    public class ParameterSet : NameValueCollection
    {
        public ParameterSet(XmlNode section)
        {
            this.FlowID = section.Attributes["flowID"].InnerText;

            XmlNodeList xnl = section.SelectNodes("add");
            foreach (XmlNode xn in xnl)
            {
                this.Add(xn.Attributes["key"].InnerText, xn.Attributes["value"].InnerText);
            }
        }

        public string FlowID { get; private set; }
    }
}
