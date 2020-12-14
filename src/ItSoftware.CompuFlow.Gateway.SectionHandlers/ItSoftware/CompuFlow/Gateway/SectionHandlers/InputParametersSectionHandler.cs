using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
namespace ItSoftware.CompuFlow.Gateway.SectionHandlers
{
    public class InputParametersSectionHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            return new InputParametersCollection(section);
        }

        #endregion
    }
}
