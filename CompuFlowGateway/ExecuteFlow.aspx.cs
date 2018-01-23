using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Specialized;
using ItSoftware.CompuFlow.Gateway.SectionHandlers;
using System.Configuration;
using System.Messaging;
using System.Threading;
using ItSoftware.ExceptionHandler;
using ItSoftware.CompuFlow.Manifest;
namespace CompuFlowGateway
{
    public partial class ExecuteFlow : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string flowID = Request.QueryString["FlowID"];
            if ( string.IsNullOrEmpty(flowID) ) {
                flowID = Request.Form["FlowID"];
                if ( string.IsNullOrEmpty(flowID) ) {
                    Response.Write("Missing required parameter FlowID.");
                    return;
                }
            }

            NameValueCollection inputParameters = GenerateInputParameters(flowID);
            FlowManifest manifest = new FlowManifest();
            manifest.RequestInitiated = DateTime.Now;
            manifest.InputParameters = inputParameters;
            manifest.FlowID = flowID;
            manifest.GUID = Guid.NewGuid();
            
            //foreach (string key in inputParameters.Keys)
            //{
            //    Response.Write("<br>" + key + "=" + inputParameters[key]);
            //}
            
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
                //
                // If we have ResponseInBrowser = true then redirect to response in browser handler
                //
                bool bRespInBrowser = false;
                string respInBrowser = inputParameters["ResponseInBrowser"];
                if (!string.IsNullOrEmpty(respInBrowser) && !string.IsNullOrWhiteSpace(respInBrowser))
                {
                    try
                    {
                        bRespInBrowser = bool.Parse(respInBrowser);
                    }
                    catch (FormatException)
                    {
                    }
                }

                //
                // Do we have response in browser
                //
                if (bRespInBrowser)
                {
                    // Redirect to response in browser handler.
                    Response.Redirect("ResponseInBrowserHandler.ashx?guid=" + manifest.GUID.ToString(), true);
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
                ExceptionManager.PublishException(new Exception(msg, x),"Error");
            }
        }
        /// <summary>
        /// Create InputParameters namevaluecollection.
        /// </summary>
        /// <param name="flowID"></param>
        /// <returns></returns>
        private NameValueCollection GenerateInputParameters(string flowID)
        {
            NameValueCollection queryStrings = Request.QueryString;
            NameValueCollection formStrings = Request.Form;
            
            InputParametersCollection ipc = ConfigurationManager.GetSection("inputParameters") as InputParametersCollection;

            NameValueCollection defaultParameters = ipc.DefaultParameters[flowID];
            NameValueCollection overrideParameters = ipc.OverrideParameters[flowID];

            NameValueCollection inputParameters = new NameValueCollection();
            if (defaultParameters != null)
            {
                AddParametersToCollection(inputParameters, defaultParameters);
            }
            AddParametersToCollection(inputParameters, queryStrings);
            AddParametersToCollection(inputParameters, formStrings);
            
            if (overrideParameters != null)
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
    }
}