using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItSoftware.CompuFlow.Publisher.Interfaces;
using ItSoftware.CompuFlow.Common.Status;
namespace ItSoftware.CompuFlow.Publisher.TestPublisher
{
    public class PublishSiteHtml : IPublisher   
    {
        #region IPublisher Members
        public void Publish(System.Collections.Specialized.NameValueCollection inputParameters, System.Collections.Hashtable applicationCache, string tempDirectory, string configDirectory, string dataDirectory, string retrivalOutputDirectory, string generatorOutputDirectory, ItSoftware.CompuFlow.Common.IExecutionEngine pIExecutionEngine)
        {
            StatusProgressItem spi = new StatusProgressItem { Description = "Copying", Status = "Running" };
            pIExecutionEngine.AddStatusInformation(spi);
            System.IO.File.Copy(System.IO.Path.Combine(retrivalOutputDirectory, "Site.html"), System.IO.Path.Combine("C:\\Temp", "Site.html"),true);
            spi.Status = "OK";
        }
        #endregion
    }
}
