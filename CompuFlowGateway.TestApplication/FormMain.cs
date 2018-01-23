using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace CompuFlowGateway.TestApplication
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void toolStripButtonExecuteFlow_Click(object sender, EventArgs e)
        {
            FormKeyValue fkv = new FormKeyValue();
            fkv.ShowDialog();
            if (fkv.Keys != null && fkv.Values != null)
            {

                CFGateway.ArrayOfString keys = new CFGateway.ArrayOfString();
                keys.AddRange(fkv.Keys);

                CFGateway.ArrayOfString values = new CFGateway.ArrayOfString();
                values.AddRange(fkv.Values);
                
                CFGateway.ExecuteFlowWebServiceSoapClient client = new CFGateway.ExecuteFlowWebServiceSoapClient();
                client.ExecuteFlow(this.toolStripTextBoxFlowID.Text, keys, values);  
            }
        }

        private void toolStripButtonExecuteFlowWithReturn_Click(object sender, EventArgs e)
        {
            FormKeyValue fkv = new FormKeyValue();
            fkv.ShowDialog();
            if (fkv.Keys != null && fkv.Values != null)
            {

                CFGateway.ArrayOfString keys = new CFGateway.ArrayOfString();
                keys.AddRange(fkv.Keys);

                CFGateway.ArrayOfString values = new CFGateway.ArrayOfString();
                values.AddRange(fkv.Values);

                CFGateway.ExecuteFlowWebServiceSoapClient client = new CFGateway.ExecuteFlowWebServiceSoapClient();
                client.Endpoint.Binding.OpenTimeout = new TimeSpan(0, 0, 50); 
                client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 0, 50);
                client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 50);
                client.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 0, 50);
                //client.Endpoint.   
                //((System.ServiceModel.BasicHttpBinding)client.Endpoint.Binding).MaxReceivedMessageSize = 1024 * 1024 * 1024;
                //((System.ServiceModel.BasicHttpBinding)client.Endpoint.Binding).MaxBufferSize  = 1024 * 1024 * 1024;
                //((System.ServiceModel.BasicHttpBinding)client.Endpoint.Binding).ReaderQuotas.MaxStringContentLength = 1024 * 1024 * 1024;
                //((System.ServiceModel.BasicHttpBinding)client.Endpoint.Binding).ReaderQuotas.MaxBytesPerRead = 1024 * 1024 * 1024;
                //((System.ServiceModel.BasicHttpBinding)client.Endpoint.Binding).ReaderQuotas.MaxArrayLength = 1024 * 1024 * 1024;
                
                
                try
                {
                    byte[] retVal = client.ExecuteFlowWithReturn(this.toolStripTextBoxFlowID.Text, keys, values);
                    string filename = "C:\\Temp\\rib.html";
                    using (FileStream fs = File.Create(filename))
                    {
                        fs.Write(retVal, 0, retVal.Length);
                    }
                    this.webBrowser1.Navigate(filename);
                }
                catch (Exception x)
                {
                    MessageBox.Show("Exception:" + x.GetType().FullName + "\r\n" + x.ToString());
                }
            }
        }
    }
}
