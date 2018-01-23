using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace ItSoftware.CompuFlow.Gateway.SectionHandlers
{
    public class InputParametersCollection
    {
        public InputParametersCollection(XmlNode section)
        {
            this.DefaultParameters = new ParametersContainer(section.SelectSingleNode("defaultParameters"));
            this.OverrideParameters = new ParametersContainer(section.SelectSingleNode("overrideParameters"));
        }

        public ParametersContainer DefaultParameters { get; private set; }
        public ParametersContainer OverrideParameters { get; private set; }
    }
}
