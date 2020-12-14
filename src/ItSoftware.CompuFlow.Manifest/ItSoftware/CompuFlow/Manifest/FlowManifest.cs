using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.Specialized;
namespace ItSoftware.CompuFlow.Manifest
{
    [Serializable]
    public class FlowManifest
    {
        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public FlowManifest()
        {
        }
        /// <summary>
        /// Deserializes the Manifest from XML string.
        /// </summary>
        /// <param name="manifest"></param>
        public FlowManifest(string manifest)
        {
            XmlDocument xdManifest = new XmlDocument();
            xdManifest.LoadXml(manifest);

            this.DeserializeFromXml(xdManifest); 
        }
        /// <summary>
        /// Deserializes the Manifest from XML.
        /// </summary>
        /// <param name="xdManifest"></param>
        public FlowManifest(XmlDocument xdManifest)
        {
            this.DeserializeFromXml(xdManifest);
        }
        /// <summary>
        /// Deserialize XmlDocument to manifest.
        /// </summary>
        /// <param name="xdManifest"></param>
        private void DeserializeFromXml(XmlDocument xdManifest)
        {
            XmlNode xnGUID = xdManifest.SelectSingleNode("/FlowManifest/GUID");
            if (xnGUID != null)
            {
                this.GUID = Guid.Parse(xnGUID.InnerText);
            }

            XmlNode xnRetrivalOutputDirectory = xdManifest.SelectSingleNode("/FlowManifest/RetrivalOutputDirectory");
            if (xnRetrivalOutputDirectory != null)
            {
                this.RetrivalOutputDirectory = xnRetrivalOutputDirectory.InnerText;
            }

            XmlNode xnGeneratorOutputDirectory = xdManifest.SelectSingleNode("/FlowManifest/GeneratorOutputDirectory");
            if (xnGeneratorOutputDirectory != null)
            {
                this.GeneratorOutputDirectory = xnGeneratorOutputDirectory.InnerText;
            }

            XmlNode xnRequestInitiated = xdManifest.SelectSingleNode("/FlowManifest/RequestInitiated");
            if (xnRequestInitiated != null)
            {
                this.RequestInitiated = DateTime.Parse(xnRequestInitiated.InnerText);
            }

            XmlNode xnRequestCompleted = xdManifest.SelectSingleNode("/FlowManifest/RequestCompleted");
            if (xnRequestCompleted != null)
            {
                this.RequestCompleted = DateTime.Parse(xnRequestCompleted.InnerText);
            }

            XmlNode xnFlowID = xdManifest.SelectSingleNode("/FlowManifest/FlowID");
            if (xnFlowID != null)
            {
                this.FlowID = xnFlowID.InnerText;
            }

            XmlNode xnFlowStatus = xdManifest.SelectSingleNode("/FlowManifest/FlowStatus");
            if (xnFlowStatus != null)
            {
                this.FlowStatus = (FlowStatus)Enum.Parse(typeof(FlowStatus), xnFlowStatus.InnerText);
            }

            XmlNode xnHasReturnedResponseInBrowser = xdManifest.SelectSingleNode("/FlowManifest/HasReturnedResponseInBrowser");
            if (xnHasReturnedResponseInBrowser != null)
            {
                this.HasReturnedResponseInBrowser = bool.Parse(xnHasReturnedResponseInBrowser.InnerText); 
            }

            XmlNode xnInputParameters = xdManifest.SelectSingleNode("/FlowManifest/InputParameters");
            if (xnInputParameters != null)
            {
                XmlNodeList xnlInputParameter = xnInputParameters.SelectNodes("InputParameter");
                NameValueCollection nvc = new NameValueCollection();
                foreach (XmlNode xnIP in xnlInputParameter)
                {
                    string key = xnIP.SelectSingleNode("Key").InnerText;
                    string value = xnIP.SelectSingleNode("Value").InnerText;
                    nvc.Add(key, value);
                }
                this.InputParameters = nvc;
            }
        }
        /// <summary>
        /// The flow guid. Each flow has its own unique guid. Used for naming instance directories etc.
        /// </summary>
        public Guid GUID { get; set; }
        /// <summary>
        /// The retrival stage output directory.
        /// </summary>
        public string RetrivalOutputDirectory { get; set; }
        /// <summary>
        /// The generator stage output directory.
        /// </summary>
        public string GeneratorOutputDirectory { get; set; }
        /// <summary>
        /// The DateTime when the flow request was initiated.
        /// </summary>
        public DateTime RequestInitiated { get; set; }
        /// <summary>
        /// The DateTime when the flow was completed.
        /// </summary>
        public DateTime RequestCompleted { get; set; }
        /// <summary>
        /// The flow id.
        /// </summary>
        public string FlowID { get; set; }
        /// <summary>
        /// The flow status.
        /// </summary>
        public FlowStatus FlowStatus { get; set; }
        /// <summary>
        /// The input parameters.
        /// </summary>
        public NameValueCollection InputParameters { get; set; }
        /// <summary>
        /// If we have returned response in browser. If at any stage we try to do it again
        /// we ignore it.
        /// </summary>
        public bool HasReturnedResponseInBrowser { get; set; }
        /// <summary>
        /// Serialized the manifest to XML.
        /// </summary>
        /// <returns></returns>
        public XmlDocument ToXmlDocument()
        {
            XmlDocument xdManifest = new XmlDocument();
            xdManifest.AppendChild(xdManifest.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement xeFlowManifest = xdManifest.CreateElement("FlowManifest");
            xdManifest.AppendChild(xeFlowManifest);

            if (this.GUID != null)
            {
                XmlElement xeGUID = xdManifest.CreateElement("GUID");
                xeGUID.InnerText = this.GUID.ToString();
                xeFlowManifest.AppendChild(xeGUID);
            }

            if (this.RetrivalOutputDirectory != null)
            {
                XmlElement xeRetrivalOutputDirectory = xdManifest.CreateElement("RetrivalOutputDirectory");
                xeRetrivalOutputDirectory.InnerText = this.RetrivalOutputDirectory;
                xeFlowManifest.AppendChild(xeRetrivalOutputDirectory);
            }

            if (this.GeneratorOutputDirectory != null)
            {
                XmlElement xeGeneratorOutputDirectory = xdManifest.CreateElement("GeneratorOutputDirectory");
                xeGeneratorOutputDirectory.InnerText = this.GeneratorOutputDirectory;
                xeFlowManifest.AppendChild(xeGeneratorOutputDirectory);
            }

            if (this.RequestInitiated != null)
            {
                XmlElement xeRequestInitiated = xdManifest.CreateElement("RequestInitiated");
                xeRequestInitiated.InnerText = this.RequestInitiated.ToString("s");
                xeFlowManifest.AppendChild(xeRequestInitiated);
            }

            if (this.RequestCompleted != null)
            {
                XmlElement xeRequestCompleted = xdManifest.CreateElement("RequestCompleted");
                xeRequestCompleted.InnerText = this.RequestCompleted.ToString("s");
                xeFlowManifest.AppendChild(xeRequestCompleted);
            }

            if (this.FlowID != null)
            {
                XmlElement xeFlowID = xdManifest.CreateElement("FlowID");
                xeFlowID.InnerText = this.FlowID;
                xeFlowManifest.AppendChild(xeFlowID);
            }

            
            XmlElement xeFlowStatus = xdManifest.CreateElement("FlowStatus");
            xeFlowStatus.InnerText = this.FlowStatus.ToString();
            xeFlowManifest.AppendChild(xeFlowStatus);

            XmlElement xeHasReturnedResponseInBrowser = xdManifest.CreateElement("HasReturnedResponseInBrowser");
            xeHasReturnedResponseInBrowser.InnerText = this.HasReturnedResponseInBrowser.ToString();
            xeFlowManifest.AppendChild(xeHasReturnedResponseInBrowser); 
            

            if (this.InputParameters != null)
            {
                XmlElement xeInputParameters = xdManifest.CreateElement("InputParameters");

                foreach (string key in this.InputParameters.Keys)
                {
                    XmlElement xeInputParameter = xdManifest.CreateElement("InputParameter");
                    
                    XmlElement xeKey = xdManifest.CreateElement("Key");
                    xeKey.InnerText = key;
                    xeInputParameter.AppendChild(xeKey);

                    XmlElement xeValue = xdManifest.CreateElement("Value");
                    xeValue.InnerText = this.InputParameters[key];
                    xeInputParameter.AppendChild(xeValue);

                    xeInputParameters.AppendChild(xeInputParameter); 
                }

                if (xeInputParameters.ChildNodes.Count > 0)
                {
                    xeFlowManifest.AppendChild(xeInputParameters);
                }
            }

            return xdManifest;
        }
    }// class
}// namespace
