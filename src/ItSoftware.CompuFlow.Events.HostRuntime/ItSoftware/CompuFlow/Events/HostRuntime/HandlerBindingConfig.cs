using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItSoftware.CompuFlow.Common.HostRuntime;
using ItSoftware.CompuFlow.Events.Interfaces;
using System.Xml;
using System.Reflection;
namespace ItSoftware.CompuFlow.Events.HostRuntime
{
    public class HandlerBindingConfig : BindingConfig
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="xd"></param>
        public HandlerBindingConfig(XmlDocument xd)
            : base(xd)
        {
            
        }
        private IEventHandler m_pIEventHandler = null;
        /// <summary>
        /// Retrival interface.
        /// If we don't have the interface create it. If we have it and 
        /// we don't pool then create it again. othervise return interface.
        /// </summary>
        public IEventHandler IEventHandlerRef 
        {
            get
            {
                if (m_pIEventHandler == null)
                {
                    m_pIEventHandler = this.CreateInterface();
                }
                else
                {
                    if (this.Pooled == false)
                    {
                        m_pIEventHandler = this.CreateInterface();
                    }
                }
                return m_pIEventHandler;
            } 
        }
        /// <summary>
        /// Create interface.
        /// </summary>
        /// <returns></returns>
        private IEventHandler CreateInterface()
        {
            Assembly assembly = Assembly.Load(this.DisplayName);
            object retrival = assembly.CreateInstance(this.Type, true);
            if (retrival == null)
            {
                string msg = string.Format("Could not create instance of type.\r\nDisplayName: {0}\r\nType: {1}", this.DisplayName, this.Type);
                throw new HostRuntimeException(msg);
            }
            IEventHandler retVal = retrival as IEventHandler;
            if (retVal == null)
            {
                string msg = string.Format("Type did not implement supported interface.\r\nDisplayName: {0}\r\nType: {1}", this.DisplayName, this.Type);
                throw new HostRuntimeException(msg);
            }
            return retVal;
        }
    }// class
}// namespace
