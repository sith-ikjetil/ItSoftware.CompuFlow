using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using ItSoftware.CompuFlow.Common;
using ItSoftware.CompuFlow.Manifest;
namespace ItSoftware.CompuFlow.Events.Interfaces
{
    public interface IEventHandler
    {
        void HandleEvent(FlowManifest manifest, IExecutionEngine pIExecutionEngine);
    }
}
