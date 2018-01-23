using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace ItSoftware.CompuFlow.Common.Status.Contracts
{
    [ServiceContract(Namespace = "http://schemas.itsoftware.no/2010/CompuFlow/IStatusInformation")]
    public interface IStatusInformation
    {
        [OperationContract]
        string GatherStatusInformation();
    }
}
