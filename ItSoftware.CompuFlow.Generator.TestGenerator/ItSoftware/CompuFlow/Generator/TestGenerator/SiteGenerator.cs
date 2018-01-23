using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItSoftware.CompuFlow.Common.Status;
using ItSoftware.CompuFlow.Generator.Interfaces;
using System.IO;
namespace ItSoftware.CompuFlow.Generator.TestGenerator
{
    public class SiteGenerator : IGenerator
    {
        #region IGenerator Members

        public void Generate(System.Collections.Specialized.NameValueCollection inputParameters, System.Collections.Hashtable applicationCache, string tempDirectory, string configDirectory, string dataDirectory, string outputDirectory, string retrivalOutputDirectory, Common.IExecutionEngine pIExecutionEngine)
        {
            string html = null;

            StatusProgressItem siLoading = new StatusProgressItem { Description = "Loading HTML", Status = "Running" };
            pIExecutionEngine.AddStatusInformation(siLoading);
            try
            {
                using (StreamReader sr = File.OpenText(Path.Combine(retrivalOutputDirectory, "Site.html")))
                {
                    html = sr.ReadToEnd();
                }
                siLoading.Status = "OK";
            }
            catch (Exception x)
            {
                siLoading.ErrorInfo = x.ToString();
                siLoading.Status = "Failure";
            }

            string rib = inputParameters["ResponseInBrowser"];
            if ( rib != null ) {
                bool bRib = bool.Parse(rib);

                if (bRib)
                {
                    StatusProgressItem siResponseInBrowser = new StatusProgressItem { Description = "Returning information to browser", Status = "Running" };
                    pIExecutionEngine.AddStatusInformation(siResponseInBrowser);
                    try
                    {
                        pIExecutionEngine.ResponseInBrowser.WriteHtml(html);
                        siResponseInBrowser.Status = "OK";
                    }
                    catch (Exception y)
                    {
                        siResponseInBrowser.Status = "Failure";
                        siResponseInBrowser.ErrorInfo = y.ToString();
                    }
                }
            }
        }

        #endregion
    }
}
