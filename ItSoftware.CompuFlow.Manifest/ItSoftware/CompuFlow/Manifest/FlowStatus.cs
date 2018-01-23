using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItSoftware.CompuFlow.Manifest
{
    [Serializable]
    public enum FlowStatus
    {
        ErrorInitialRequest = 1,
        ErrorRetrival = 2,
        ErrorGenerator = 3,
        ErrorPublisher = 4,
        CompletedGateway = 5,
        CompletedRetrival = 6,
        CompletedGenerator = 7,
        CompletedPublisher = 8
    }
}
