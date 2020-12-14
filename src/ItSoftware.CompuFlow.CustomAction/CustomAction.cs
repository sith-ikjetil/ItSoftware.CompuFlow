using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Messaging;
namespace ItSoftware.CompuFlow.CustomAction
{
    public class CustomActions
    {
        [CustomAction]        
        public static ActionResult IsMsmqInstalled(Session session)
        {
            string path = "SYSTEM\\ControlSet001\\services\\MSMQ";            

            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey pathKey = localMachineKey.OpenSubKey(path, false);
            if (pathKey == null || pathKey.Handle.IsInvalid)
            {
                MessageBox.Show("This setup requires MSMQ to be installed on the machine.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ActionResult.Failure;
            }

            object val = pathKey.GetValue("ObjectName");
            if ( val == null )
            {
                MessageBox.Show("This setup requires MSMQ to be installed on the machine.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ActionResult.Failure;
            }
            
            return ActionResult.Success;
        }

    
        [CustomAction]
        public static ActionResult SetMsmqAcl(Session session)
        {
            try
            {
                MessageQueue mqEvents = new MessageQueue(".\\private$\\CompuFlowEvents");
                mqEvents.SetPermissions("Everyone", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);

                MessageQueue mqGenerator = new MessageQueue(".\\private$\\CompuFlowGenerator");
                mqGenerator.SetPermissions("Everyone", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);

                MessageQueue mqPublisher = new MessageQueue(".\\private$\\CompuFlowPublisher");
                mqPublisher.SetPermissions("Everyone", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);

                MessageQueue mqRetrival = new MessageQueue(".\\private$\\CompuFlowRetrival");
                mqRetrival.SetPermissions("Everyone", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
            }
            catch ( Exception x )
            {
                MessageBox.Show(x.ToString(), "Error");
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ExecuteInstallUtilEvents(Session session)
        {
            try
            {
                session.Log("Begin ExecuteInstallUtilEvents");

                string customActionData = session["CustomActionData"];
                string version = "v4.0.30319";

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = " -i ItSoftware.CompuFlow.Events.exe";
                startInfo.WorkingDirectory = customActionData;
                startInfo.Verb = "runas";
                startInfo.FileName = Path.Combine(Path.Combine(GetInstallUtilDirectory(), version), "InstallUtil.exe");
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }

                // Configure App.Config
                string configFilename = Path.Combine(customActionData, "ItSoftware.CompuFlow.Events.exe.config");
                XmlDocument xd = new XmlDocument();
                xd.Load(configFilename);

                var nodes = xd.SelectNodes("/configuration/appSettings/add");
                foreach ( var node in nodes)
                {
                    XmlNode xn = node as XmlNode;
                    XmlNode xnKey = xn.Attributes.GetNamedItem("key");
                    XmlNode xnValue = xn.Attributes.GetNamedItem("value");
                    if ( xnKey.Value == "HandlersDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Handlers");
                    }
                    else if ( xnKey.Value == "TemporaryHandlerFilesDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Temporary Handler Files");
                    }
                    else if ( xnKey.Value == "FailureDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Failure");
                    }
                }
                xd.Save(configFilename);

                ProcessStartInfo startInfo2 = new ProcessStartInfo();
                startInfo2.Arguments = " start \"CompuFlow : Events\"";// ItSoftware.CaseFlow.Events.exe";
                startInfo2.WorkingDirectory = customActionData;
                startInfo2.Verb = "runas";
                startInfo2.FileName = "sc";
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo2))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ExecuteInstallUtilGenerator(Session session)
        {
            try
            {
                session.Log("Begin ExecuteInstallUtilGenerator");

                string customActionData = session["CustomActionData"];
                string version = "v4.0.30319";

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = " -i ItSoftware.CompuFlow.Generator.exe";
                startInfo.WorkingDirectory = customActionData;
                startInfo.Verb = "runas";
                startInfo.FileName = Path.Combine(Path.Combine(GetInstallUtilDirectory(), version), "InstallUtil.exe");
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }

                // Configure App.Config
                string configFilename = Path.Combine(customActionData, "ItSoftware.CompuFlow.Generator.exe.config");
                XmlDocument xd = new XmlDocument();
                xd.Load(configFilename);

                var nodes = xd.SelectNodes("/configuration/appSettings/add");
                foreach (var node in nodes)
                {
                    XmlNode xn = node as XmlNode;
                    XmlNode xnKey = xn.Attributes.GetNamedItem("key");
                    XmlNode xnValue = xn.Attributes.GetNamedItem("value");
                    if (xnKey.Value == "GeneratorsDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Generators");
                    }
                    else if ( xnKey.Value == "TempDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Temp");
                    }
                    else if (xnKey.Value == "TemporaryGeneratorFilesDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Temporary Generator Files");
                    }
                    else if ( xnKey.Value == "OutputDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Output");
                    }
                    else if (xnKey.Value == "FailureDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Failure");
                    }
                }
                xd.Save(configFilename);

                ProcessStartInfo startInfo2 = new ProcessStartInfo();
                startInfo2.Arguments = " start \"CompuFlow : Generator\"";// ItSoftware.CaseFlow.Generator.exe";
                startInfo2.WorkingDirectory = customActionData;
                startInfo2.Verb = "runas";
                startInfo2.FileName = "sc";
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo2))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ExecuteInstallUtilPublisher(Session session)
        {
            try
            {
                session.Log("Begin ExecuteInstallUtilPublisher");

                string customActionData = session["CustomActionData"];
                string version = "v4.0.30319";

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = " -i ItSoftware.CompuFlow.Publisher.exe";
                startInfo.WorkingDirectory = customActionData;
                startInfo.Verb = "runas";
                startInfo.FileName = Path.Combine(Path.Combine(GetInstallUtilDirectory(), version), "InstallUtil.exe");
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }

                // Configure App.Config
                string configFilename = Path.Combine(customActionData, "ItSoftware.CompuFlow.Publisher.exe.config");
                XmlDocument xd = new XmlDocument();
                xd.Load(configFilename);

                var nodes = xd.SelectNodes("/configuration/appSettings/add");
                foreach (var node in nodes)
                {
                    XmlNode xn = node as XmlNode;
                    XmlNode xnKey = xn.Attributes.GetNamedItem("key");
                    XmlNode xnValue = xn.Attributes.GetNamedItem("value");
                    if (xnKey.Value == "PublishersDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Publishers");
                    }
                    else if (xnKey.Value == "TempDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Temp");
                    }
                    else if (xnKey.Value == "TemporaryPublisherFilesDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Temporary Publisher Files");
                    }
                    else if (xnKey.Value == "FailureDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Failure");
                    }
                }
                xd.Save(configFilename);

                ProcessStartInfo startInfo2 = new ProcessStartInfo();
                startInfo2.Arguments = " start \"CompuFlow : Publisher\"";// ItSoftware.CaseFlow.Publisher.exe";
                startInfo2.WorkingDirectory = customActionData;
                startInfo2.Verb = "runas";
                startInfo2.FileName = "sc";
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo2))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ExecuteInstallUtilRetrival(Session session)
        {
            try
            {
                session.Log("Begin ExecuteInstallUtilRetrival");

                string customActionData = session["CustomActionData"];
                string version = "v4.0.30319";

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = " -i ItSoftware.CompuFlow.Retrival.exe";
                startInfo.WorkingDirectory = customActionData;
                startInfo.Verb = "runas";
                startInfo.FileName = Path.Combine(Path.Combine(GetInstallUtilDirectory(), version), "InstallUtil.exe");
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }

                // Configure App.Config
                string configFilename = Path.Combine(customActionData, "ItSoftware.CompuFlow.Retrival.exe.config");
                XmlDocument xd = new XmlDocument();
                xd.Load(configFilename);

                var nodes = xd.SelectNodes("/configuration/appSettings/add");
                foreach (var node in nodes)
                {
                    XmlNode xn = node as XmlNode;
                    XmlNode xnKey = xn.Attributes.GetNamedItem("key");
                    XmlNode xnValue = xn.Attributes.GetNamedItem("value");
                    if (xnKey.Value == "RetrivalsDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Retrivals");
                    }
                    else if (xnKey.Value == "TempDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Temp");
                    }
                    else if (xnKey.Value == "TemporaryRetrivalFilesDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Temporary Retrival Files");
                    }
                    else if ( xnKey.Value == "OutputDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Output");
                    }
                    else if (xnKey.Value == "FailureDirectory")
                    {
                        xnValue.Value = Path.Combine(customActionData, "Failure");
                    }
                }
                xd.Save(configFilename);

                ProcessStartInfo startInfo2 = new ProcessStartInfo();
                startInfo2.Arguments = " start \"CompuFlow : Retrival\"";// ItSoftware.CaseFlow.Retrival.exe";
                startInfo2.WorkingDirectory = customActionData;
                startInfo2.Verb = "runas";
                startInfo2.FileName = "sc";
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo2))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ExecuteUnInstallUtilEvents(Session session)
        {
            try
            {
                session.Log("Begin ExecuteUnInstallUtilEvents");

                string customActionData = session["CustomActionData"];
                string version = "v4.0.30319";

                ProcessStartInfo startInfo0 = new ProcessStartInfo();
                startInfo0.Arguments = " stop \"CompuFlow : Events\"";// ItSoftware.CaseFlow.Events.exe";
                startInfo0.WorkingDirectory = customActionData;
                startInfo0.Verb = "runas";
                startInfo0.FileName = "sc";
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo0))
                {
                    exeProcess.WaitForExit();
                }                

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = " /u ItSoftware.CaseFlow.Events.exe";
                startInfo.WorkingDirectory = customActionData;
                startInfo.Verb = "runas";
                startInfo.FileName = Path.Combine(Path.Combine(GetInstallUtilDirectory(), version), "InstallUtil.exe");
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }

                ProcessStartInfo startInfo2 = new ProcessStartInfo();
                startInfo2.Arguments = " delete \"CompuFlow : Events\"";// ItSoftware.CaseFlow.Events.exe";
                startInfo2.WorkingDirectory = customActionData;
                startInfo2.Verb = "runas";
                startInfo2.FileName = "sc";
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo2))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ExecuteUnInstallUtilGenerator(Session session)
        {
            try
            {
                session.Log("Begin ExecuteUnInstallUtilGenerator");

                string customActionData = session["CustomActionData"];
                string version = "v4.0.30319";

                ProcessStartInfo startInfo0 = new ProcessStartInfo();
                startInfo0.Arguments = " stop \"CompuFlow : Generator\"";// ItSoftware.CaseFlow.Generator.exe";
                startInfo0.WorkingDirectory = customActionData;
                startInfo0.Verb = "runas";
                startInfo0.FileName = "sc";
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo0))
                {
                    exeProcess.WaitForExit();
                }

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = " /u ItSoftware.CaseFlow.Generator.exe";
                startInfo.WorkingDirectory = customActionData;
                startInfo.Verb = "runas";
                startInfo.FileName = Path.Combine(Path.Combine(GetInstallUtilDirectory(), version), "InstallUtil.exe");
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }

                ProcessStartInfo startInfo2 = new ProcessStartInfo();
                startInfo2.Arguments = " delete \"CompuFlow : Generator\"";// ItSoftware.CaseFlow.Generator.exe";
                startInfo2.WorkingDirectory = customActionData;
                startInfo2.Verb = "runas";
                startInfo2.FileName = "sc";
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo2))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ExecuteUnInstallUtilPublisher(Session session)
        {
            try
            {
                session.Log("Begin ExecuteUnInstallUtilPublisher");

                string customActionData = session["CustomActionData"];
                string version = "v4.0.30319";

                ProcessStartInfo startInfo0 = new ProcessStartInfo();
                startInfo0.Arguments = " stop \"CompuFlow : Publisher\"";// ItSoftware.CaseFlow.Publisher.exe";
                startInfo0.WorkingDirectory = customActionData;
                startInfo0.Verb = "runas";
                startInfo0.FileName = "sc";
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo0))
                {
                    exeProcess.WaitForExit();
                }

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = " /u ItSoftware.CaseFlow.Publisher.exe";
                startInfo.WorkingDirectory = customActionData;
                startInfo.Verb = "runas";
                startInfo.FileName = Path.Combine(Path.Combine(GetInstallUtilDirectory(), version), "InstallUtil.exe");
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }

                ProcessStartInfo startInfo2 = new ProcessStartInfo();
                startInfo2.Arguments = " delete \"CompuFlow : Publisher\"";// ItSoftware.CaseFlow.Publisher.exe";
                startInfo2.WorkingDirectory = customActionData;
                startInfo2.Verb = "runas";
                startInfo2.FileName = "sc";
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo2))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ExecuteUnInstallUtilRetrival(Session session)
        {
            try
            {
                session.Log("Begin ExecuteUnInstallUtilRetrival");

                string customActionData = session["CustomActionData"];
                string version = "v4.0.30319";

                ProcessStartInfo startInfo0 = new ProcessStartInfo();
                startInfo0.Arguments = " stop \"CompuFlow : Retrival\"";// ItSoftware.CaseFlow.Retrival.exe";
                startInfo0.WorkingDirectory = customActionData;
                startInfo0.Verb = "runas";
                startInfo0.FileName = "sc";
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo0))
                {
                    exeProcess.WaitForExit();
                }

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = " /u ItSoftware.CaseFlow.Retrival.exe";
                startInfo.WorkingDirectory = customActionData;
                startInfo.Verb = "runas";
                startInfo.FileName = Path.Combine(Path.Combine(GetInstallUtilDirectory(), version), "InstallUtil.exe");
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }

                ProcessStartInfo startInfo2 = new ProcessStartInfo();
                startInfo2.Arguments = " delete \"CompuFlow : Retrival\"";// ItSoftware.CaseFlow.Retrival.exe";
                startInfo2.WorkingDirectory = customActionData;
                startInfo2.Verb = "runas";
                startInfo2.FileName = "sc";
                using (Process exeProcess = System.Diagnostics.Process.Start(startInfo2))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }


        private static string GetInstallUtilDirectory()
        {
            string path = "SOFTWARE\\Microsoft\\.NETFramework";

            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey pathKey = localMachineKey.OpenSubKey(path, false);
            object objectInstallRoot = pathKey.GetValue("InstallRoot");
            return objectInstallRoot as string;
        }
    }// class
}// namespace
