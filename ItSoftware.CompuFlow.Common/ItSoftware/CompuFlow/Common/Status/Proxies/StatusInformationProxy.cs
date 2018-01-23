using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ItSoftware.CompuFlow.Common.Status.Contracts;
namespace ItSoftware.CompuFlow.Common.Status.Proxies
{
    public class StatusInformationProxy : ClientBase<IStatusInformation>, IStatusInformation
    {
        public StatusInformationProxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress endpointAddress)
            : base(binding, endpointAddress)
        { }

        #region IStatusInformation Members
        public string GatherStatusInformation()
        {
            return Channel.GatherStatusInformation();
        }
        #endregion
    }
}
