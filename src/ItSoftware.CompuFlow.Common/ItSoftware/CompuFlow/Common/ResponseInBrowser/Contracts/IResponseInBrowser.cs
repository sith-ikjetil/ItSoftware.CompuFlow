using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace ItSoftware.CompuFlow.Common.ResponseInBrowser.Contracts
{
    [ServiceContract(Namespace = "http://schemas.itsoftware.no/2010/CompuFlow/IResponseInBrowser")]
    public interface IResponseInBrowser
    {
        [OperationContract]
        void WriteText(Guid guid, string content);

        [OperationContract]
        void WriteHtml(Guid guid, string html);

        [OperationContract]
        void WriteXml(Guid guid, string xml);
        
        [OperationContract]
        void WriteFile1(Guid guid, string flowID,byte[] content);

        [OperationContract]
        void WriteFile2(Guid guid, byte[] content, string filename, string contentType, string charset);
    }
}
