using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using ItSoftware.CompuFlow.Common;
namespace ItSoftware.CompuFlow.Publisher.Interfaces
{
    public interface IPublisher
    {
        /// <summary>
        /// Publish method.
        /// </summary>
        /// <param name="inputParameters">Input parameters.</param>
        /// <param name="applicationCache">Application cache. Lives as long as the appdomain does.</param>
        /// <param name="tempDirectory">Directory path of a temporary storage folder.</param>
        /// <param name="configDirectory">Directory path of the config folder of the packet(.zip) file.</param>
        /// <param name="dataDirectory">Direcotry path of the data folder of the packet(.zip) file.</param>
        /// <param name="outputDirectory">Directory path of the folder where output files are put.</param>
        void Publish(NameValueCollection inputParameters, Hashtable applicationCache, string tempDirectory, string configDirectory, string dataDirectory, string retrivalOutputDirectory, string generatorOutputDirectory, IExecutionEngine pIExecutionEngine);
    }
}
