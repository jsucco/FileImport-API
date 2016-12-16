using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Text;

namespace MCS.Helpers
{
    public class MCSTest
    {
        public static int sourceId = 1; 
        public static string mcsCaraddress = @"C:\Users\johns\Dropbox (Standard Textile)\APR_Publish\MCS\MCSTesting\LAVORA_CAROLINA.txt";
        public static string mcsJoraddress = @"C:\Users\johns\Dropbox (Standard Textile)\APR_Publish\MCS\MCSTesting\LAVORA_JORDAN.txt";
        private JavaScriptSerializer jser = new JavaScriptSerializer();
        public async Task<String> ProcessAuthLocalMCS(string TestFilePath)
        {
            if (TestFilePath == null || TestFilePath.Length == 0)
                return "File Path Not Set"; 

            HttpClient httpClient = new HttpClient();
            List<Models.MCSImport> retlist = new List<Models.MCSImport>();
            string returnobj = "Nan";
            retlist = GetFileasObject(TestFilePath);

            if (retlist.Count > 0)
            {
                try
                {

                    setAuthorizationHeader(httpClient);

                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

                    string retfiltobj_ser = jser.Serialize(retlist);

                    var obj = await httpClient.PostAsJsonAsync("http://localhost:60521/api/CSV", retfiltobj_ser)
                            .ConfigureAwait(continueOnCapturedContext: false);

                    var returnval = obj.Content.ReadAsStringAsync().Result;

                    returnobj = returnval;
                }
                catch (Exception err)
                {
                    string erroress = err.Message;
                }
            }
            return returnobj;
        }

        public async Task<Models.Source> getSourceId()
        {
            int sourceId = 1;
            HttpClient httpclient = new HttpClient();

            Models.Source record = new Models.Source() { MachineName = Environment.MachineName, Name = Environment.MachineName, IPAddress = GetLocalIPAddress() };

            if (record != null)
            {
                setAuthorizationHeader(httpclient);

                httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

                string Content = jser.Serialize(record);

                var obj = await httpclient.PostAsJsonAsync("http://localhost:60521/api/Sources", Content)
                            .ConfigureAwait(continueOnCapturedContext: false);

                var returnval = obj.Content.ReadAsStringAsync().Result;

                if (returnval != null && returnval.Length > 0)
                {
                    try
                    {
                        record.Id = Convert.ToInt32(returnval);

                    }
                    catch (Exception e)
                    {
                        record.Id = 1;
                    }
                }

            }

            return record;
        }

        public async Task<Models.sourceDocInfo> CallDocAsync(int testId)
        {
            

            Models.sourceDocInfo docInfo = new Models.sourceDocInfo();
            System.Web.Script.Serialization.JavaScriptSerializer jser = new System.Web.Script.Serialization.JavaScriptSerializer();

            using (var client = new HttpClient())
            {
                setAuthorizationHeader(client);
                string resourceaddress = "http://localhost:60521/api/CSV/" + testId.ToString();

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = await client.GetAsync("http://localhost:60521/api/CSV/3", HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(continueOnCapturedContext: false);
                string returnstr = result.Content.ReadAsStringAsync().Result;
                if (returnstr != "")
                {
                    try
                    {
                        string[] stringArr = jser.Deserialize<string[]>(returnstr);
                        if (stringArr.Length > 1)
                        {
                            docInfo.DocNumber = Convert.ToInt32(stringArr[0]);
                            docInfo.LastPath = stringArr[1]; 
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            return docInfo;
        }

        public static string GetLocalIPAddress()
        {
            string ipaddress = "NAN";
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipaddress = ip.ToString();
                }
            }
            return ipaddress;
        }

        private void setAuthorizationHeader(HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "sa", "nimda"))
                        ));
        }

        public List<MCS.Models.MCSImport> GetFileasObject(string pathname)
        {

            List<MCS.Models.MCSImport> retlist = new List<MCS.Models.MCSImport>();
            try
            {
                using (StreamReader sr = new StreamReader(pathname, Encoding.Default))
                {
                    string line;
                    int counter = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Trim().Length > 0)
                        {                
                            retlist.Add(new MCS.Models.MCSImport() { LineBlob = line, DocNumber = 1, Location = pathname, SourceId = sourceId, Timestamp = DateTime.Now });
                            counter++; 
                        }
                    }

                }
            }
            catch (Exception e)
            {

            }


            return retlist;
        }

    }
}