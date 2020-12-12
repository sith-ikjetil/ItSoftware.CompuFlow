using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using Microsoft.Win32;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using ItSoftware.CompuFlow.Common;
using System.ServiceModel;
using System.Xml;
using ItSoftware.CompuFlow.Common.Status.Proxies;
using ItSoftware.CompuFlow.Manifest;
using ItSoftware.ExceptionHandler;
/*
PreviewSelectedTabChanged="RadRibbonBar_PreviewSelectedTabChanged"
            SelectedTabChanged="RadRibbonBar_SelectedTabChanged"
*/
namespace ItSoftware.CompuFlow.ControlCenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private DetailsType Details { get; set; }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            
            this.Details = DetailsType.Progress;
           // Refresh(ExecutionEngineType.Retrival);
        }

        private void Refresh(ExecutionEngineType type)
        {
            string endpointAddress = null;
            if (type == ExecutionEngineType.Retrival)
            {
                endpointAddress = "net.pipe://localhost/ItSoftware.CompuFlow.Retrival"; 
            }
            else if (type == ExecutionEngineType.Generator)
            {
                endpointAddress = "net.pipe://localhost/ItSoftware.CompuFlow.Generator";
            }
            else if (type == ExecutionEngineType.Publisher)
            {
                endpointAddress = "net.pipe://localhost/ItSoftware.CompuFlow.Publisher";
            }
            else if (type == ExecutionEngineType.Events)
            {
                endpointAddress = "net.pipe://localhost/ItSoftware.CompuFlow.Events";
            }

            if (string.IsNullOrEmpty(endpointAddress) || string.IsNullOrWhiteSpace(endpointAddress) )
            {
                return;
            }

            try
            {
                StatusInformationProxy proxy = new StatusInformationProxy(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), new EndpointAddress(endpointAddress));
                //string info = @"<?xml version=""1.0"" encoding=""utf-8""?><statusInformation><channels><channel name=""ChannelA"" queueCount=""5""><queue><item>&lt;?xml version=""1.0"" encoding=""utf-8""?&gt;&lt;FlowManifest&gt;&lt;GUID&gt;789ceeed-b3c3-43ce-9311-d9a7c61bac35&lt;/GUID&gt;&lt;RequestInitiated&gt;2010-06-29T15:04:28&lt;/RequestInitiated&gt;&lt;RequestCompleted&gt;0001-01-01T00:00:00&lt;/RequestCompleted&gt;&lt;FlowID&gt;1&lt;/FlowID&gt;&lt;FlowStatus&gt;CompletedGateway&lt;/FlowStatus&gt;&lt;InputParameters&gt;&lt;InputParameter&gt;&lt;Key&gt;FlowID&lt;/Key&gt;&lt;Value&gt;1&lt;/Value&gt;&lt;/InputParameter&gt;&lt;InputParameter&gt;&lt;Key&gt;Site&lt;/Key&gt;&lt;Value&gt;kjetil.info&lt;/Value&gt;&lt;/InputParameter&gt;&lt;/InputParameters&gt;&lt;/FlowManifest&gt;</item><item>&lt;?xml version=""1.0"" encoding=""utf-8""?&gt;&lt;FlowManifest&gt;&lt;GUID&gt;b70f784c-bb62-4269-bcd5-4ef667250b97&lt;/GUID&gt;&lt;RequestInitiated&gt;2010-06-29T15:04:29&lt;/RequestInitiated&gt;&lt;RequestCompleted&gt;0001-01-01T00:00:00&lt;/RequestCompleted&gt;&lt;FlowID&gt;1&lt;/FlowID&gt;&lt;FlowStatus&gt;CompletedGateway&lt;/FlowStatus&gt;&lt;InputParameters&gt;&lt;InputParameter&gt;&lt;Key&gt;FlowID&lt;/Key&gt;&lt;Value&gt;1&lt;/Value&gt;&lt;/InputParameter&gt;&lt;InputParameter&gt;&lt;Key&gt;Site&lt;/Key&gt;&lt;Value&gt;kjetil.info&lt;/Value&gt;&lt;/InputParameter&gt;&lt;/InputParameters&gt;&lt;/FlowManifest&gt;</item><item>&lt;?xml version=""1.0"" encoding=""utf-8""?&gt;&lt;FlowManifest&gt;&lt;GUID&gt;2ec98202-d449-4a41-a623-8d536f35ebf7&lt;/GUID&gt;&lt;RequestInitiated&gt;2010-06-29T15:04:30&lt;/RequestInitiated&gt;&lt;RequestCompleted&gt;0001-01-01T00:00:00&lt;/RequestCompleted&gt;&lt;FlowID&gt;1&lt;/FlowID&gt;&lt;FlowStatus&gt;CompletedGateway&lt;/FlowStatus&gt;&lt;InputParameters&gt;&lt;InputParameter&gt;&lt;Key&gt;FlowID&lt;/Key&gt;&lt;Value&gt;1&lt;/Value&gt;&lt;/InputParameter&gt;&lt;InputParameter&gt;&lt;Key&gt;Site&lt;/Key&gt;&lt;Value&gt;kjetil.info&lt;/Value&gt;&lt;/InputParameter&gt;&lt;/InputParameters&gt;&lt;/FlowManifest&gt;</item><item>&lt;?xml version=""1.0"" encoding=""utf-8""?&gt;&lt;FlowManifest&gt;&lt;GUID&gt;61ce2cdb-33a9-40ac-8c75-ebadea38417b&lt;/GUID&gt;&lt;RequestInitiated&gt;2010-06-29T15:04:31&lt;/RequestInitiated&gt;&lt;RequestCompleted&gt;0001-01-01T00:00:00&lt;/RequestCompleted&gt;&lt;FlowID&gt;1&lt;/FlowID&gt;&lt;FlowStatus&gt;CompletedGateway&lt;/FlowStatus&gt;&lt;InputParameters&gt;&lt;InputParameter&gt;&lt;Key&gt;FlowID&lt;/Key&gt;&lt;Value&gt;1&lt;/Value&gt;&lt;/InputParameter&gt;&lt;InputParameter&gt;&lt;Key&gt;Site&lt;/Key&gt;&lt;Value&gt;kjetil.info&lt;/Value&gt;&lt;/InputParameter&gt;&lt;/InputParameters&gt;&lt;/FlowManifest&gt;</item><item>&lt;?xml version=""1.0"" encoding=""utf-8""?&gt;&lt;FlowManifest&gt;&lt;GUID&gt;4c0fe694-df5f-4dc0-bd02-b035a0f4053d&lt;/GUID&gt;&lt;RequestInitiated&gt;2010-06-29T15:04:32&lt;/RequestInitiated&gt;&lt;RequestCompleted&gt;0001-01-01T00:00:00&lt;/RequestCompleted&gt;&lt;FlowID&gt;1&lt;/FlowID&gt;&lt;FlowStatus&gt;CompletedGateway&lt;/FlowStatus&gt;&lt;InputParameters&gt;&lt;InputParameter&gt;&lt;Key&gt;FlowID&lt;/Key&gt;&lt;Value&gt;1&lt;/Value&gt;&lt;/InputParameter&gt;&lt;InputParameter&gt;&lt;Key&gt;Site&lt;/Key&gt;&lt;Value&gt;kjetil.info&lt;/Value&gt;&lt;/InputParameter&gt;&lt;/InputParameters&gt;&lt;/FlowManifest&gt;</item></queue><flows><flow filename=""C:\ItSoftware.CompuFlow\Retrival\Retrivals\ChannelA\1\1.zip"" flowID=""1""><lastExecTime>00:00:00</lastExecTime><minExecTime>00:00:00</minExecTime><maxExecTime>00:00:00</maxExecTime><avgExecTime>00:00:00</avgExecTime><progressItems /></flow></flows></channel></channels></statusInformation>";
                string info = proxy.GatherStatusInformation();
                RenderStatusInformation(info, type);
            }
            catch (Exception x) {
                ExceptionManager.PublishException(x, "Error");
                ErrorInfoWindow wei = new ErrorInfoWindow();
                wei.ErrorInformation = this.FormatException(x);
                wei.ShowDialog();
            }
        }

        private void RenderStatusInformation(string info, ExecutionEngineType type)
        {
            if (type == ExecutionEngineType.Retrival)
            {
                DataTable dt = null;
                if (this.Details == DetailsType.Status)
                {
                    dt = CreateDataTableStatus(info);
                }
                else if (this.Details == DetailsType.Progress)
                {
                    dt = CreateDataTableProgress(info);
                }
                else if (this.Details == DetailsType.Manifest)
                {
                    dt = CreateDataTableManifest(info);
                }
                else if (this.Details == DetailsType.Queue)
                {
                    dt = CreateDataTableQueue(info);
                }
                if (dt != null)
                {
                    this.RetrivalDataGrid.ItemsSource = null;
                    this.RetrivalDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            else if (type == ExecutionEngineType.Generator)
            {
                DataTable dt = null;
                if (this.Details == DetailsType.Status)
                {
                    dt = CreateDataTableStatus(info);
                }
                else if (this.Details == DetailsType.Progress)
                {
                    dt = CreateDataTableProgress(info);
                }
                else if (this.Details == DetailsType.Manifest)
                {
                    dt = CreateDataTableManifest(info);
                }
                else if (this.Details == DetailsType.Queue)
                {
                    dt = CreateDataTableQueue(info);
                }
                if (dt != null)
                {
                    this.GeneratorDataGrid.ItemsSource = null;
                    this.GeneratorDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            else if (type == ExecutionEngineType.Publisher)
            {
                DataTable dt = null;
                if (this.Details == DetailsType.Status)
                {
                    dt = CreateDataTableStatus(info);
                }
                else if (this.Details == DetailsType.Progress)
                {
                    dt = CreateDataTableProgress(info);
                }
                else if (this.Details == DetailsType.Manifest)
                {
                    dt = CreateDataTableManifest(info);
                }
                else if (this.Details == DetailsType.Queue)
                {
                    dt = CreateDataTableQueue(info);
                }
                if (dt != null)
                {
                    this.PublisherDataGrid.ItemsSource = null;
                    this.PublisherDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            else if (type == ExecutionEngineType.Events)
            {
                DataTable dt = null;
                if (this.Details == DetailsType.Status)
                {
                    dt = CreateDataTableStatus(info);
                }
                else if (this.Details == DetailsType.Progress)
                {
                    dt = CreateDataTableProgress(info);
                }
                else if (this.Details == DetailsType.Manifest)
                {
                    dt = CreateDataTableManifest(info);
                }
                else if (this.Details == DetailsType.Queue)
                {
                    dt = CreateDataTableQueue(info);
                }
                if (dt != null)
                {
                    this.EventsDataGrid.ItemsSource = null;
                    this.EventsDataGrid.ItemsSource = dt.DefaultView;
                }
            }
        }

        private DataTable CreateDataTableStatus(string info)
        {
            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(info))
            {
                return dt;
            }

            try
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(info);

                // Add columns
                DataColumn dtChannelName = dt.Columns.Add("Channel Name");
                DataColumn dtQueueCount = dt.Columns.Add("Queue Count");
                DataColumn dtFilename = dt.Columns.Add("Filename");
                DataColumn dcFlowID = dt.Columns.Add("FlowID");
                DataColumn dcLastExecTime = dt.Columns.Add("Last Execution Time");
                DataColumn dcMinExecTime = dt.Columns.Add("Min Execution Time");
                DataColumn dcMaxExecTime = dt.Columns.Add("Max Execution Time");
                DataColumn dcAvgExecTime = dt.Columns.Add("Avg Execution Time");

                XmlNodeList xnlChannels = xd.SelectNodes("/statusInformation/channels/channel");
                foreach (XmlNode xnChannel in xnlChannels)
                {
                    //
                    // New DataRow
                    //
                    DataRow dr = dt.NewRow();

                    //
                    // Add channel info
                    //
                    dr[dtChannelName] = xnChannel.Attributes["name"].InnerText;
                    dr[dtQueueCount] = xnChannel.Attributes["queueCount"].InnerText;
                    
                    //
                    // Add Flows
                    //
                    XmlNodeList xnlFlows = xnChannel.SelectNodes("flows/flow");
                    foreach (XmlNode xnFlow in xnlFlows)
                    {
                        dr[dtFilename] = xnFlow.Attributes["filename"].InnerText;
                        dr[dcFlowID] = xnFlow.Attributes["flowID"].InnerText;

                        dr[dcLastExecTime] = xnFlow.SelectSingleNode("lastExecTime").InnerText; 
                        dr[dcMinExecTime] = xnFlow.SelectSingleNode("minExecTime").InnerText;
                        dr[dcMaxExecTime] = xnFlow.SelectSingleNode("maxExecTime").InnerText;
                        dr[dcAvgExecTime] = xnFlow.SelectSingleNode("avgExecTime").InnerText;
                    }
                    //
                    // Add row to data table
                    //
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception)
            {
                
            }
            return dt;
        }
        private DataTable CreateDataTableProgress(string info)
        {
            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(info))
            {
                return dt;
            }

            try
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(info);

                // Add columns
                DataColumn dtChannelName = dt.Columns.Add("Channel Name");
                DataColumn dtFilename = dt.Columns.Add("Filename");
                DataColumn dcFlowID = dt.Columns.Add("FlowID");
                DataColumn dcDescription = dt.Columns.Add("Description");
                DataColumn dcStatus = dt.Columns.Add("Status");
                DataColumn dcStartTime = dt.Columns.Add("Start Time");
                DataColumn dcExecTime = dt.Columns.Add("Execution Time");
                DataColumn dcMinTime = dt.Columns.Add("Min Time");
                DataColumn dcAvgTime = dt.Columns.Add("Avg Time");
                DataColumn dcMaxTime = dt.Columns.Add("Max Time");
                DataColumn dcErrorInfo = dt.Columns.Add("Error Information");
                if (dcErrorInfo.MaxLength <= 0)
                {
                    dcErrorInfo.MaxLength = 30000;
                }


                XmlNodeList xnlChannels = xd.SelectNodes("/statusInformation/channels/channel");
                foreach (XmlNode xnChannel in xnlChannels)
                {
                    //
                    // Add Flows
                    //
                    XmlNodeList xnlFlows = xnChannel.SelectNodes("flows/flow");
                    foreach (XmlNode xnFlow in xnlFlows)
                    {
                        

                        XmlNodeList xnlProgressItems = xnFlow.SelectNodes("progressItems/progressItem");
                        foreach (XmlNode xnProgressItem in xnlProgressItems)
                        {
                            DataRow drPI = dt.NewRow();

                            drPI[dtChannelName] = xnChannel.Attributes["name"].InnerText;

                            drPI[dtFilename] = xnFlow.Attributes["filename"].InnerText;
                            drPI[dcFlowID] = xnFlow.Attributes["flowID"].InnerText;

                            drPI[dcDescription] = xnProgressItem.SelectSingleNode("description").InnerText;
                            drPI[dcStatus] = xnProgressItem.SelectSingleNode("status").InnerText;
                            drPI[dcStartTime] = xnProgressItem.SelectSingleNode("startTime").InnerText;
                            drPI[dcExecTime] = xnProgressItem.SelectSingleNode("execTime").InnerText;
                            drPI[dcMinTime] = xnProgressItem.SelectSingleNode("minExecTime").InnerText;
                            drPI[dcAvgTime] = xnProgressItem.SelectSingleNode("avgExecTime").InnerText;
                            drPI[dcMaxTime] = xnProgressItem.SelectSingleNode("maxExecTime").InnerText;
                            if (xnProgressItem.SelectSingleNode("errorInfo").InnerText.Length > dcErrorInfo.MaxLength)
                            {
                                drPI[dcErrorInfo] = xnProgressItem.SelectSingleNode("errorInfo").InnerText.Substring(0, dcErrorInfo.MaxLength);
                            }
                            else
                            {
                                drPI[dcErrorInfo] = xnProgressItem.SelectSingleNode("errorInfo").InnerText;
                            }
                            dt.Rows.Add(drPI);
                        }
                    }
                }
            }
            catch (Exception x)
            {
                ExceptionManager.PublishException(x, "Error");
            }
            return dt;
        }
        private DataTable CreateDataTableManifest(string info)
        {
            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(info))
            {
                return dt;
            }

            try
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(info);

                // Add columns
                DataColumn dtChannelName = dt.Columns.Add("Channel Name");
                DataColumn dcFlowID = dt.Columns.Add("Flow ID");
                DataColumn dcGuid = dt.Columns.Add("Guid");
                DataColumn dcKey = dt.Columns.Add("Key");
                DataColumn dcValue = dt.Columns.Add("Value");

                XmlNodeList xnlChannels = xd.SelectNodes("/statusInformation/channels/channel");
                foreach (XmlNode xnChannel in xnlChannels)
                {
                    if (xnChannel.SelectSingleNode("currentFlowManifest") == null)
                    {
                        break;
                    }

                    FlowManifest manifest = new FlowManifest(xnChannel.SelectSingleNode("currentFlowManifest").InnerText);
                    //
                    // Add key value
                    //
                    foreach (string key in manifest.InputParameters.Keys)
                    {
                        //
                        // New DataRow
                        //
                        DataRow dr = dt.NewRow();

                        //
                        // Add channel info
                        //
                        dr[dtChannelName] = xnChannel.Attributes["name"].InnerText;
                        dr[dcFlowID] = manifest.FlowID;
                        dr[dcGuid] = manifest.GUID.ToString();
                        dr[dcKey] = key;
                        dr[dcValue] = manifest.InputParameters[key];
                        //
                        // Add row to data table
                        //
                        dt.Rows.Add(dr);
                    }// for each     
               }
            }
            catch (Exception)
            {

            }
            return dt;
        }
        private DataTable CreateDataTableQueue(string info)
        {
            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(info))
            {
                return dt;
            }

            try
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(info);

                // Add columns
                DataColumn dtChannelName = dt.Columns.Add("Channel Name");
                DataColumn dtQueueCount = dt.Columns.Add("Queue Count");
                DataColumn dcFlowID = dt.Columns.Add("Flow ID");
                DataColumn dcGuid = dt.Columns.Add("Guid");
                DataColumn dcKey = dt.Columns.Add("Key");
                DataColumn dcValue = dt.Columns.Add("Value");

                XmlNodeList xnlChannels = xd.SelectNodes("/statusInformation/channels/channel");
                foreach (XmlNode xnChannel in xnlChannels)
                {
                    XmlNodeList xnlQueueItems = xnChannel.SelectNodes("queue/item");
                    foreach (XmlNode xnQueueItem in xnlQueueItems)
                    {
                        FlowManifest manifest = new FlowManifest(xnQueueItem.InnerText);
                        
                        foreach (string key in manifest.InputParameters.Keys)
                        {
                            //
                            // New DataRow
                            //
                            DataRow dr = dt.NewRow();
                            //
                            // Add channel info
                            //
                            dr[dtChannelName] = xnChannel.Attributes["name"].InnerText;
                            dr[dtQueueCount] = xnChannel.Attributes["queueCount"].InnerText;

                            dr[dcFlowID] = manifest.FlowID;
                            dr[dcGuid] = manifest.GUID.ToString();

                            dr[dcKey] = key;
                            dr[dcValue] = manifest.InputParameters[key];

                            dt.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return dt;
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void ToggleStatusButton_Click(object sender, RoutedEventArgs e)
        {
            this.Details = DetailsType.Status;

            if (object.ReferenceEquals(this.TabControlMain.SelectedItem, RetrivalDocumentPane))
            {
                Refresh(ExecutionEngineType.Retrival);
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, GeneratorDocumentPane))
            {
                Refresh(ExecutionEngineType.Generator);
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, PublisherDocumentPane))
            {
                Refresh(ExecutionEngineType.Publisher);
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, EventsDocumentPane))
            {
                Refresh(ExecutionEngineType.Events);
            }
        }

        private string RenderCurrentTableToCSV()
        {
            StringBuilder text = new StringBuilder();
            if (object.ReferenceEquals(this.TabControlMain.SelectedItem, RetrivalDocumentPane))
            {
                if (this.RetrivalDataGrid.Columns == null || this.RetrivalDataGrid.ItemsSource == null)
                {
                    return string.Empty;
                }

                int columnCount = 0;
                foreach (var gvc in this.RetrivalDataGrid.Columns)
                {
                    if (text.Length > 0)
                    {
                        text.Append(";");
                    }
                    text.Append(gvc.Header.ToString().Replace(";", "_"));
                    columnCount++;
                }

                if (text.Length == 0)
                {
                    return string.Empty;
                }

                text.AppendLine();

                DataView dv = this.RetrivalDataGrid.ItemsSource as DataView;
                DataTable dt = dv.Table;
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        if (i > 0)
                        {
                            text.Append(";");
                        }
                        text.Append(dr[i].ToString().Replace(";", "_"));
                    }
                    text.AppendLine();
                }
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, GeneratorDocumentPane))
            {
                if (this.GeneratorDataGrid.Columns == null || this.GeneratorDataGrid.ItemsSource == null)
                {
                    return string.Empty;
                }

                int columnCount = 0;
                foreach (var gvc in this.GeneratorDataGrid.Columns)
                {
                    if (text.Length > 0)
                    {
                        text.Append(";");
                    }
                    text.Append(gvc.Header.ToString().Replace(";", "_"));
                    columnCount++;
                }

                if (text.Length == 0)
                {
                    return string.Empty;
                }

                text.AppendLine();

                DataView dv = this.GeneratorDataGrid.ItemsSource as DataView;
                DataTable dt = dv.Table;
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        if (i > 0)
                        {
                            text.Append(";");
                        }
                        text.Append(dr[i].ToString().Replace(";", "_"));
                    }
                    text.AppendLine();
                }
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, PublisherDocumentPane))
            {
                if (this.PublisherDataGrid.Columns == null || this.PublisherDataGrid.ItemsSource == null)
                {
                    return string.Empty;
                }

                int columnCount = 0;
                foreach (var gvc in this.PublisherDataGrid.Columns)
                {
                    if (text.Length > 0)
                    {
                        text.Append(";");
                    }
                    text.Append(gvc.Header.ToString().Replace(";", "_"));
                    columnCount++;
                }

                if (text.Length == 0)
                {
                    return string.Empty;
                }

                text.AppendLine();

                DataView dv = this.PublisherDataGrid.ItemsSource as DataView;
                DataTable dt = dv.Table;
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        if (i > 0)
                        {
                            text.Append(";");
                        }
                        text.Append(dr[i].ToString().Replace(";", "_"));
                    }
                    text.AppendLine();
                }
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, EventsDocumentPane))
            {

            }
            return text.ToString();
        }

        private void ToggleExportCSVButton_Click(object sender, RoutedEventArgs e)
        {
            string csv = RenderCurrentTableToCSV();
            if (csv == null || string.IsNullOrWhiteSpace(csv) | string.IsNullOrEmpty(csv))
            {
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Comma separated files (*.csv)|*.csv";
            sfd.FilterIndex = 0;
            sfd.FileName = "export.csv";
            if (sfd.ShowDialog().HasValue == true)
            {
                if (File.Exists(sfd.FileName))
                {
                    File.Delete(sfd.FileName);
                }

                using (StreamWriter sw = File.CreateText(sfd.FileName))
                {
                    sw.Write(csv);
                }
            }

        }

        private void ToggleCopyButton_Click(object sender, RoutedEventArgs e)
        {
            string csv = RenderCurrentTableToCSV();
            if (csv != null && !string.IsNullOrEmpty(csv) && !string.IsNullOrWhiteSpace(csv))
            {
                Clipboard.SetText(csv);
            }
        }

        private void ToggleManifestButton_Click(object sender, RoutedEventArgs e)
        {
            this.Details = DetailsType.Manifest;

            if (object.ReferenceEquals(this.TabControlMain.SelectedItem, RetrivalDocumentPane))
            {
                Refresh(ExecutionEngineType.Retrival);
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, GeneratorDocumentPane))
            {
                Refresh(ExecutionEngineType.Generator);
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, PublisherDocumentPane))
            {
                Refresh(ExecutionEngineType.Publisher);
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, EventsDocumentPane))
            {
                Refresh(ExecutionEngineType.Events);
            }
        }

        private void ToggleProgressButton_Click(object sender, RoutedEventArgs e)
        {
            this.Details = DetailsType.Progress;

            if ( object.ReferenceEquals(this.TabControlMain.SelectedItem, RetrivalDocumentPane))
            {
                Refresh(ExecutionEngineType.Retrival);
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, GeneratorDocumentPane))
            {
                Refresh(ExecutionEngineType.Generator);
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, PublisherDocumentPane))
            {
                Refresh(ExecutionEngineType.Publisher);
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, EventsDocumentPane))
            {
                Refresh(ExecutionEngineType.Events);
            }
        }
        private void ToggleQueueButton_Click(object sender, RoutedEventArgs e)
        {
            this.Details = DetailsType.Queue;

            if (object.ReferenceEquals(this.TabControlMain.SelectedItem, RetrivalDocumentPane))
            {
                Refresh(ExecutionEngineType.Retrival);
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, GeneratorDocumentPane))
            {
                Refresh(ExecutionEngineType.Generator);
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, PublisherDocumentPane))
            {
                Refresh(ExecutionEngineType.Publisher);
            }
            else if (object.ReferenceEquals(this.TabControlMain.SelectedItem, EventsDocumentPane))
            {
                Refresh(ExecutionEngineType.Events);
            }
        }
       
        public string FormatException(Exception exception)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("###################################");
            output.AppendLine(string.Format("## Timestamp: {0}", DateTime.Now.ToString("s").Replace('T', ' ')));
            output.AppendLine();

            output.AppendLine("###################################");
            output.AppendLine("## Environment");
            output.AppendLine(string.Format("Machine Name: {0}", Environment.MachineName));
            output.AppendLine(string.Format("Current Directory: {0}", Environment.CurrentDirectory));
            output.AppendLine(string.Format("Is 64 Bit Operating System: {0}", Environment.Is64BitOperatingSystem));
            output.AppendLine(string.Format("Is 64 Bit Process: {0}", Environment.Is64BitProcess));
            output.AppendLine(string.Format("OS Version: {0}", Environment.OSVersion));
            output.AppendLine(string.Format("Processor Count: {0}", Environment.ProcessorCount));
            output.AppendLine(string.Format("CLR Version: {0}", Environment.Version));
            output.AppendLine();

            RenderException(output, exception);

            return output.ToString();
        }

       

        private void RenderException(StringBuilder output, Exception exception)
        {
            output.AppendLine("###################################");
            output.AppendLine("## Exception");
            output.AppendLine(string.Format("Full Name: {0}", exception.GetType().FullName));
            output.AppendLine(string.Format("Message: {0}", exception.Message ?? "<NULL>"));
            output.AppendLine(string.Format("Source: {0}", exception.Source ?? "<NULL>"));
            output.AppendLine(string.Format("Stack Trace: {0}", exception.StackTrace ?? "<NULL>"));
            if (exception.TargetSite != null)
            {
                output.AppendLine(string.Format("Target Site: {0}", exception.TargetSite.Name ?? "<NULL>"));
            }
            output.AppendLine(string.Format("Help Link: {0}", exception.HelpLink ?? "<NULL>"));
            if (exception.Data != null)
            {
                foreach (string key in exception.Data.Keys)
                {
                    output.AppendLine(string.Format("{0}: {1}", key, exception.Data[key] ?? "<NULL>"));
                }
            }
            output.AppendLine();

            if (exception.InnerException != null)
            {
                RenderException(output, exception.InnerException);
            }
        }

    }
}
