using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItSoftware.CompuFlow.Common.HostRuntime;
using ItSoftware.CompuFlow.Generator.Interfaces;
using System.Xml;
using System.Reflection;
namespace ItSoftware.CompuFlow.Generator.HostRuntime
{
    public class RetrivalBindingConfig : BindingConfig
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="xd"></param>
        public RetrivalBindingConfig(XmlDocument xd)
            : base(xd)
        {
            
        }
        private IGenerator m_pIGenerator = null;
        /// <summary>
        /// Retrival interface.
        /// If we don't have the interface create it. If we have it and 
        /// we don't pool then create it again. othervise return interface.
        /// </summary>
        public IGenerator IGeneratorRef 
        {
            get
            {
                if (m_pIGenerator == null)
                {
                    m_pIGenerator = this.CreateInterface();
                }
                else
                {
                    if (this.Pooled == false)
                    {
                        m_pIGenerator = this.CreateInterface();
                    }
                }
                return m_pIGenerator;
            } 
        }
        /// <summary>
        /// Create interface.
        /// </summary>
        /// <returns></returns>
        private IGenerator CreateInterface()
        {
            Assembly assembly = Assembly.Load(this.DisplayName);
            object retrival = assembly.CreateInstance(this.Type, true);
            if (retrival == null)
            {
                string msg = string.Format("Could not create instance of type.\r\nDisplayName: {0}\r\nType: {1}", this.DisplayName, this.Type);
                throw new HostRuntimeException(msg);
            }
            IGenerator retVal = retrival as IGenerator;
            if (retVal == null)
            {
                string msg = string.Format("Type did not implement supported interface.\r\nDisplayName: {0}\r\nType: {1}", this.DisplayName, this.Type);
                throw new HostRuntimeException(msg);
            }
            return retVal;
        }
    }// class
}// namespace
