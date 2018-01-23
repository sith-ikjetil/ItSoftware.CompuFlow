using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace ItSoftware.CompuFlow.Gateway.SectionHandlers
{
    public class ResponseInBrowserCollection : List<ResponseInBrowserItem>
    {
        public ResponseInBrowserCollection(XmlNode section)
        {
            if ( section == null ) {
               throw new ArgumentNullException("section");    
            }

            XmlNodeList xnlAdd = section.SelectNodes("add");
            foreach (XmlNode xnAdd in xnlAdd)
            {
                this.Add(new ResponseInBrowserItem(xnAdd));
            }
        }

        public ResponseInBrowserItem this[string flowID]
        {
            get
            {
                foreach (ResponseInBrowserItem item in this)
                {
                    if (item.FlowID == flowID)
                    {
                        return item;
                    }
                }
                return null;
            }
        }
    }// class
}// namespace 
