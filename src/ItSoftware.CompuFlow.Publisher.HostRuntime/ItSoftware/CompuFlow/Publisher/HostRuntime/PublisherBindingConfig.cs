using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItSoftware.CompuFlow.Common.HostRuntime;
using ItSoftware.CompuFlow.Publisher.Interfaces;
using System.Xml;
using System.Reflection;
namespace ItSoftware.CompuFlow.Publisher.HostRuntime
{
    public class PublisherBindingConfig : BindingConfig
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="xd"></param>
        public PublisherBindingConfig(XmlDocument xd)
            : base(xd)
        {
            
        }
        private IPublisher m_pIRetrival = null;
        /// <summary>
        /// Retrival interface.
        /// If we don't have the interface create it. If we have it and 
        /// we don't pool then create it again. othervise return interface.
        /// </summary>
        public IPublisher IPublisherRef 
        {
            get
            {
                if (m_pIRetrival == null)
                {
                    m_pIRetrival = this.CreateInterface();
                }
                else
                {
                    if (this.Pooled == false)
                    {
                        m_pIRetrival = this.CreateInterface();
                    }
                }
                return m_pIRetrival;
            } 
        }
        /// <summary>
        /// Create interface.
        /// </summary>
        /// <returns></returns>
        private IPublisher CreateInterface()
        {
            Assembly assembly = Assembly.Load(this.DisplayName);
            object retrival = assembly.CreateInstance(this.Type, true);
            if (retrival == null)
            {
                string msg = string.Format("Could not create instance of type.\r\nDisplayName: {0}\r\nType: {1}", this.DisplayName, this.Type);
                throw new HostRuntimeException(msg);
            }
            IPublisher retVal = retrival as IPublisher;
            if (retVal == null)
            {
                string msg = string.Format("Type did not implement supported interface.\r\nDisplayName: {0}\r\nType: {1}", this.DisplayName, this.Type);
                throw new HostRuntimeException(msg);
            }
            return retVal;
        }
    }// class
}// namespace
