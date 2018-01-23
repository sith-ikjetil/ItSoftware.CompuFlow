using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Services;
using System.Messaging;
using System.Collections.Specialized;
using System.Threading;
using ItSoftware.CompuFlow.Manifest;
using ItSoftware.CompuFlow.Gateway.SectionHandlers;
using ItSoftware.ExceptionHandler;
using ItSoftware.CompuFlow.Common.ResponseInBrowser.Hosts;
namespace CompuFlowGateway
{
    /// <summary>
    /// Summary description for ExecuteFlow1
    /// </summary>
    [WebService(Namespace = "http://schemas.itsoftware.no/2010/CompuFlow/ExecuteFlow")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ExecuteFlowWebService : System.Web.Services.WebService
    {
        private NameValueCollection GenerateInputParameters(string flowID, string[] keys, string[] values)
        {
            if (keys == null || values == null)
            {
                throw new ArgumentNullException("keys,values");
            }

            if (keys.Length != values.Length)
            {
                throw new ArgumentOutOfRangeException("keys,values");
            }

            InputParametersCollection ipc = ConfigurationManager.GetSection("inputParameters") as InputParametersCollection;

            NameValueCollection defaultParameters = ipc.DefaultParameters[flowID];
            NameValueCollection overrideParameters = ipc.OverrideParameters[flowID];

            NameValueCollection inputParameters = new NameValueCollection();
            if (defaultParameters != null && defaultParameters.Count > 0)
            {
                AddParametersToCollection(inputParameters, defaultParameters);
            }

            NameValueCollection nvcTemp = new NameValueCollection();
            for (int i = 0; i < keys.Length; i++)
            {
                nvcTemp.Add(keys[i], values[i]);
            }
            AddParametersToCollection(inputParameters, nvcTemp);

            if (overrideParameters != null && overrideParameters.Count > 0)
            {
                AddParametersToCollection(inputParameters, overrideParameters);
            }
            
            return inputParameters;
        }
        /// <summary>
        /// Adds key values from one namevaluecollection to another.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="add"></param>
        private void AddParametersToCollection(NameValueCollection source, NameValueCollection add)
        {
            foreach (string key in add)
            {
                if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                string[] vals = source.GetValues(key);
                if (vals == null || vals.Length > 0)
                {
                    source.Remove(key);
                    source.Add(key, add[key]);
                }
            }
        }
        [WebMethod]
        public void ExecuteFlow(string flowID, string[] keys, string[] values)
        {
            if (string.IsNullOrEmpty(flowID) || string.IsNullOrEmpty(flowID)) {
                throw new Exception("Missing required parameter FlowID.");
            }

            NameValueCollection inputParameters = GenerateInputParameters(flowID, keys,values);
            FlowManifest manifest = new FlowManifest();
            manifest.RequestInitiated = DateTime.Now;
            manifest.InputParameters = inputParameters;
            manifest.FlowID = flowID;
            manifest.GUID = Guid.NewGuid();

            string msmqPath = ConfigurationManager.AppSettings["DestinationMsmqPath"];
            try
            {
                manifest.FlowStatus = FlowStatus.CompletedGateway;

                //
                // Send to Retrival.
                //
                using (MessageQueue mqPublisher = new MessageQueue(msmqPath))
                {
                    mqPublisher.Formatter = new BinaryMessageFormatter();

                    System.Messaging.Message msgPublisher = new System.Messaging.Message();
                    msgPublisher.Formatter = new BinaryMessageFormatter();
                    msgPublisher.Body = manifest.ToXmlDocument().OuterXml;
                    msgPublisher.Label = string.Format("{0} - {1}", manifest.FlowID, manifest.FlowStatus.ToString());

                    mqPublisher.Send(msgPublisher);
                }
                //
                // Send message to log.
                //
                bool bLogToEvents = bool.Parse(ConfigurationManager.AppSettings["LogToEvents"]);
                if (bLogToEvents)
                {
                    msmqPath = ConfigurationManager.AppSettings["EventsMsmqPath"];
                    using (MessageQueue mqLog = new MessageQueue(msmqPath))
                    {
                        mqLog.Formatter = new BinaryMessageFormatter();

                        System.Messaging.Message msgLog = new System.Messaging.Message();
                        msgLog.Formatter = new BinaryMessageFormatter();
                        msgLog.Body = manifest.ToXmlDocument().OuterXml;
                        msgLog.Label = string.Format("{0} - {1}", manifest.FlowID, manifest.FlowStatus.ToString());

                        mqLog.Send(msgLog);
                    }
                }
            }
            catch (StackOverflowException)
            {
                throw;
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception x)
            {
                string msg = string.Format("Failure to send message to MSMQ Path: {0}\r\nFlow: {0}.", msmqPath, manifest.GUID.ToString());
                ExceptionManager.PublishException(new Exception(msg, x), "Error");
            }
        }

        private class FlowParams
        {
            public string FlowID { get; set; }
            public string[] Keys { get; set; }
            public string[] Values { get; set; }
            public ManualResetEvent ResetEvent { get; set; }
            public byte[] ReturnValue { get; set; }
        }

        private void ExecuteFlowWithReturnWorkerMethod(object param)
        {
            FlowParams flowParams = param as FlowParams;
            
            string flowID = flowParams.FlowID;
            string[] keys = flowParams.Keys;
            string[] values = flowParams.Values;

            NameValueCollection inputParameters = GenerateInputParameters(flowID, keys, values);

            //
            // Force insertion of ResponseInBrowser = true
            //
            string respInBrowser = inputParameters["ResponseInBrowser"];
            if (respInBrowser != null)
            {
                inputParameters.Remove("ResponseInBrowser");
            }
            inputParameters["ResponseInBrowser"] = "True";

            FlowManifest manifest = new FlowManifest();
            manifest.RequestInitiated = DateTime.Now;
            manifest.InputParameters = inputParameters;
            manifest.FlowID = flowID;
            manifest.GUID = Guid.NewGuid();

            byte[] returnValue = new byte[0];

            string msmqPath = ConfigurationManager.AppSettings["DestinationMsmqPath"];
            try
            {
                manifest.FlowStatus = FlowStatus.CompletedGateway;

                //
                // Send to Retrival.
                //
                using (MessageQueue mqPublisher = new MessageQueue(msmqPath))
                {
                    mqPublisher.Formatter = new BinaryMessageFormatter();

                    System.Messaging.Message msgPublisher = new System.Messaging.Message();
                    msgPublisher.Formatter = new BinaryMessageFormatter();
                    msgPublisher.Body = manifest.ToXmlDocument().OuterXml;
                    msgPublisher.Label = string.Format("{0} - {1}", manifest.FlowID, manifest.FlowStatus.ToString());

                    mqPublisher.Send(msgPublisher);
                }
                //
                // Send message to log.
                //
                bool bLogToEvents = bool.Parse(ConfigurationManager.AppSettings["LogToEvents"]);
                if (bLogToEvents)
                {
                    msmqPath = ConfigurationManager.AppSettings["EventsMsmqPath"];
                    using (MessageQueue mqLog = new MessageQueue(msmqPath))
                    {
                        mqLog.Formatter = new BinaryMessageFormatter();

                        System.Messaging.Message msgLog = new System.Messaging.Message();
                        msgLog.Formatter = new BinaryMessageFormatter();
                        msgLog.Body = manifest.ToXmlDocument().OuterXml;
                        msgLog.Label = string.Format("{0} - {1}", manifest.FlowID, manifest.FlowStatus.ToString());

                        mqLog.Send(msgLog);
                    }
                }

                // Handle response and return. 
                ResponseInBrowserHost host = new ResponseInBrowserHost(this.Context, "net.pipe://localhost/CompuFlow.Gateway." + manifest.GUID.ToString());
                host.NoHttpContextWrite = true;
                host.Start(manifest.GUID);
                returnValue = host.ResponseInBrowserData;
            }
            catch (StackOverflowException)
            {
                throw;
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception x)
            {
                string msg = string.Format("Failure to send message to MSMQ Path: {0}\r\nFlow: {0}.", msmqPath, manifest.GUID.ToString());
                ExceptionManager.PublishException(new Exception(msg, x), "Error");
            }

            flowParams.ReturnValue = returnValue;
            flowParams.ResetEvent.Set();
        }

        /// <summary>
        /// Execute Flow With Return.
        /// 
        /// Same as ReturnInBrowser except the response is retunred in a byte array.
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        [WebMethod]
        public byte[] ExecuteFlowWithReturn(string flowID, string[] keys, string[] values)
        {
            if (string.IsNullOrEmpty(flowID) || string.IsNullOrEmpty(flowID))
            {
                throw new Exception("Missing required parameter FlowID.");
            }
            
            FlowParams flowParams = new FlowParams();
            flowParams.FlowID = flowID;
            flowParams.Keys = keys;
            flowParams.Values = values;
            flowParams.ResetEvent = new ManualResetEvent(false);
            flowParams.ReturnValue = new byte[0];

            Thread thread = new Thread(new ParameterizedThreadStart(this.ExecuteFlowWithReturnWorkerMethod));
            thread.Start(flowParams);

            flowParams.ResetEvent.WaitOne();

            return flowParams.ReturnValue;
        }
    }
}
