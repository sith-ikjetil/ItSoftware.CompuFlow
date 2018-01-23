using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItSoftware.CompuFlow.Common;
using ItSoftware.CompuFlow.Retrival.Interfaces;
using System.IO;
using System.Net;
using System.Security;
namespace ItSoftware.CompuFlow.Retrival.TestRetriver
{
    public class RetriveSiteHtml : IRetrival
    {
        private const string HTTP_HEADER_AUTHENTICATE = "WWW-Authenticate";
        private const string HTTP_HEADER_AUTHENTICATE_NTLM_VALUE = "Negotiate,NTLM";
        private const string DEFAULT_CHARACTER_SET = "UTF-8";
        private const string CHARSET_TOKEN = "charset";

        #region IRetrival Members

        public void Retrive(System.Collections.Specialized.NameValueCollection inputParameters, System.Collections.Hashtable applicationCache, string tempDirectory, string configDirectory, string dataDirectory, string outputDirectory, IExecutionEngine pIExecutionEngine)
        {
            Common.Status.StatusProgressItem piDownloading = new Common.Status.StatusProgressItem { Description = "Downloading", Status = "..." };

            try
            {
                string html = this.Download(new Uri(inputParameters["Site"]), CredentialCache.DefaultCredentials);
                piDownloading.Status = "OK";
                using (StreamWriter sw = File.CreateText(Path.Combine(outputDirectory, "Site.html")))
                {
                    sw.Write(html);
                }
            }
            catch (Exception x)
            {
                piDownloading.Status = "Failed";
                piDownloading.ErrorInfo = x.ToString();
            } 
        }

        #endregion
        private string Download(Uri uri, ICredentials credentials)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            httpWebRequest.Method = WebRequestMethods.Http.Get;
            httpWebRequest.Timeout = 3600000;
            if (credentials != null)
            {
                httpWebRequest.Credentials = credentials;
            }
            string syndication = null;
            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                Stream s = httpWebResponse.GetResponseStream();


                MemoryStream ms = new MemoryStream();
                byte[] b = new byte[1];
                int count = 0;
                do
                {
                    count = s.Read(b, 0, 1);
                    if (count != 0)
                    {
                        ms.Write(b, 0, 1);
                    }
                } while (count != 0);
                ms.Seek(0, SeekOrigin.Begin);

                byte[] buffer = new byte[ms.Length];
                ms.Read(buffer, 0, Convert.ToInt32(ms.Length));
                ms.Seek(0, SeekOrigin.Begin);

                Encoding srcEncoding = Encoding.GetEncoding(GetCharacterSet(httpWebResponse.CharacterSet, httpWebResponse.ContentType));
                using (StreamReader sr = new StreamReader(ms, srcEncoding))
                {
                    syndication = sr.ReadToEnd();
                }// using


                // If we did not have a proper encoding recode it
                if (string.IsNullOrEmpty(httpWebResponse.CharacterSet) && IsXmlContentType(httpWebResponse.ContentType))
                {
                    ms = new MemoryStream(buffer);
                    ms.Seek(0, SeekOrigin.Begin);
                    syndication = ConvertToEncoding(syndication, ms);
                }// if                 

            }
            return syndication;
        }

        /// <summary>
        /// Check content type for xml data.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private bool IsXmlContentType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            string type = contentType.ToLower();

            if (type == "application/xml")
            {
                return true;
            }

            if (type == "text/xml")
            {
                return true;
            }

            if (type == "xml")
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Convert an xml document from one encoding to another.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="srcEncoding"></param>
        /// <returns></returns>
        private string ConvertToEncoding(string xml, Stream s)
        {
            int indexStart = xml.IndexOf("encoding=\"");
            if (indexStart > 0)
            {
                int indexStop = xml.IndexOf("\"", indexStart + 10);
                string xmlEncoding = xml.Substring(indexStart + 10, indexStop - indexStart - 10);

                // Get dst encoding
                Encoding dstEncoding = Encoding.GetEncoding(xmlEncoding);

                using (StreamReader sr = new StreamReader(s, dstEncoding))
                {
                    xml = sr.ReadToEnd();
                }

                /*                // Convert from srcEncoding to dstEncoding
                                byte[] byteString = srcEncoding.GetBytes(xml);
                                Encoding.Convert(srcEncoding, dstEncoding, byteString);
                                char[] chrs = dstEncoding.GetChars(byteString);
                                StringBuilder buffer = new StringBuilder();
                                buffer.Append(chrs);
                                xml = buffer.ToString();
                 * */
            }
            return xml;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="syndication"></param>
        /// <returns></returns>
        internal string NormalizeSyndication(string syndication)
        {
            if (syndication == null)
            {
                throw new ArgumentNullException("syndication");
            }

            int indexA = syndication.IndexOf('<');
            if (indexA != 0)
            {
                syndication = syndication.Substring(indexA, syndication.Length - indexA);
            }

            int indexB = syndication.LastIndexOf('>');
            if (indexB != syndication.Length - 1)
            {
                syndication = syndication.Substring(0, indexB + 1);
            }
            return syndication;
        }
        private string GetCharacterSet(string characterSet, string contentType)
        {
            if (!string.IsNullOrEmpty(characterSet))
            {
                return characterSet.Trim(new char[] { ' ', '\"', '\'' });
            }

            if (string.IsNullOrEmpty(contentType))
            {
                return DEFAULT_CHARACTER_SET;
            }

            string[] elements = contentType.ToLower().Split(';');
            foreach (string element in elements)
            {
                string[] parts = element.Trim().Split('=');
                if (parts[0] == CHARSET_TOKEN)
                {
                    return parts[1];
                }
            }
            return DEFAULT_CHARACTER_SET;
        }
    }
}
