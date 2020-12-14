using System;
using System.Collections.Generic;
using System.Text;

namespace ItSoftware.CompuFlow.Retrival.HostRuntime
{
    [Serializable]
    public class HostRuntimeSettings
    {
        public HostRuntimeSettings( )
        {
        }

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string TempDirectory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OutputDirectory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FailureDirectory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool OnErrorResumeNext { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Log { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DestinationMsmqPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EventsMsmqPath { get; set; }  
        #endregion
    }// class
}// namespace