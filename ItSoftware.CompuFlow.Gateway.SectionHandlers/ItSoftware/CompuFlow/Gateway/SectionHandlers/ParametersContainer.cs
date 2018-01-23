using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace ItSoftware.CompuFlow.Gateway.SectionHandlers
{
    public class ParametersContainer : List<ParameterSet>
    {
        public ParametersContainer(XmlNode section)
        {
            XmlNodeList xnl = section.SelectNodes("parameterSet");
            foreach (XmlNode xn in xnl)
            {
                this.Add(new ParameterSet(xn));
            }
        }

        public ParameterSet this[string flowID] 
        {
            get
            {
                foreach (ParameterSet ps in this)
                {
                    if (ps.FlowID == flowID)
                    {
                        return ps;
                    }
                }
                return null;
            }
        }
    }
}
