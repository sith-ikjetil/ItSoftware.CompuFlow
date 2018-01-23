using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ItSoftware.CompuFlow.Common.Status.Contracts;
namespace ItSoftware.CompuFlow.Common.Status.Services
{
    public class StatusInformationService<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings> : IStatusInformation
        where TSettings : class, IFlowSettings<THostRuntimeSettings>
        where TTransparentFlow : TransparentFlow<TRealFlow, TSettings, THostRuntimeSettings>, new()
        where TRealFlow : RealFlow<THostRuntimeSettings>
    {
        public static Controller<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings> Controller { get; set; }
        
        #region IStatusInformation Members

        public string GatherStatusInformation()
        {
            if (StatusInformationService<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings>.Controller == null)
            {
                return string.Empty;
            }
            return StatusInformationService<TSettings, TTransparentFlow, TRealFlow, THostRuntimeSettings>.Controller.GatherStatusInformation().SerializeToXml().OuterXml; 
        }

        #endregion
    }
}
