using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItSoftware.CompuFlow.Util
{
    public static class Formatter
    {
        public static string FormatException(Exception exception)
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



        private static void RenderException(StringBuilder output, Exception exception)
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
