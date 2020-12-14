using System;
using System.Collections.Generic;
using System.Text;

namespace ItSoftware.CompuFlow.Common
{
    public interface IFlowSettings<THostRuntimeSettings>
    {
        string FlowDirectory
        {
            get;                        
        }
        THostRuntimeSettings ToHostRuntimeSettings( );
    }
}
