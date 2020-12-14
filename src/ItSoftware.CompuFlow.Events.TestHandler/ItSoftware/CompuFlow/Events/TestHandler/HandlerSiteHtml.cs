using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItSoftware.CompuFlow.Events.Interfaces;
using ItSoftware.CompuFlow.Common.Status;
using ItSoftware.CompuFlow.Common;
using ItSoftware.CompuFlow.Manifest;
namespace ItSoftware.CompuFlow.Events.TestHandler
{
    public class HandlerSiteHtml : IEventHandler  
    {
        #region IPublisher Members
        public void HandleEvent(FlowManifest manifest, IExecutionEngine pIExecutionEngine)
        {
            StatusProgressItem spi0 = new StatusProgressItem { Description = "FlowStatus", Status = manifest.FlowStatus.ToString() };
            pIExecutionEngine.AddStatusInformation(spi0);

            StatusProgressItem spi = new StatusProgressItem { Description = "Testing .EndFlow", Status = "Running" };
            pIExecutionEngine.AddStatusInformation(spi);

            try
            {
                pIExecutionEngine.EndFlow = true;
                spi.Status = "SHOULD NOT WORK!";
            }
            catch (NotImplementedException)
            {
                spi.Status = "NotImplementedException";
            }

            StatusProgressItem spi2 = new StatusProgressItem { Description = "Testing .LogToEvents", Status = "Running" };
            pIExecutionEngine.AddStatusInformation(spi2);

            try
            {
                pIExecutionEngine.LogToEvents = true;
                spi2.Status = "SHOULD NOT WORK!";
            }
            catch (NotImplementedException)
            {
                spi2.Status = "NotImplementedException";
            }

            StatusProgressItem spi3 = new StatusProgressItem { Description = "Testing .ResponseInBrowser", Status = "Running" };
            pIExecutionEngine.AddStatusInformation(spi3);

            try
            {
                IResponseInBrowser i = pIExecutionEngine.ResponseInBrowser;
                spi3.Status = "SHOULD NOT WORK!";
            }
            catch (NotImplementedException)
            {
                spi3.Status = "NotImplementedException";
            }

        }
        #endregion
    }
}
