using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;

namespace MCS_API.Helpers
{

    public class dlayer
    {
        private JavaScriptSerializer json = new JavaScriptSerializer();
        public static int SourceId = 1;
        public static string mcsaddress = @"C:\Dpa95Win\Termo\Dati\LAVORA.TMP";

        public static string apiaddress = "http://mcs.standardtextile.com/api/csv";
        public static int DocNumber = 1;

        public static List<MCSImport> Statretlist = new List<MCSImport>();

        public static void Setdocnumber()
        {
            Helpers.dlayer dl = new Helpers.dlayer(); 
            sourceDocInfo docSource = dl.CallGetdocnumber();
            if (docSource != null)
            {
                Helpers.dlayer.DocNumber = docSource.DocNumber;
                Helpers.dlayer.mcsaddress = docSource.LastPath;
            }
            

        }

        public async Task<Source> getSourceId()
        {
            int sourceId = 1;
            HttpClient httpclient = new HttpClient();

            Source record = new Source() { MachineName = Environment.MachineName, Name = Environment.MachineName, IPAddress = dlayer.GetLocalIPAddress() };

            if (record != null)
            {
                setAuthorizationHeader(httpclient);

                httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

                string Content = json.Serialize(record);

                var obj = await httpclient.PostAsJsonAsync("http://mcs.standardtextile.com/api/Sources", Content)
                            .ConfigureAwait(continueOnCapturedContext: false);

                var returnval = obj.Content.ReadAsStringAsync().Result;

                if (returnval != null && returnval.Length > 0 )
                {
                    try
                    {
                        record.Id = Convert.ToInt32(returnval);
                       
                    } catch (Exception e)
                    {
                        record.Id = 1; 
                    }
                }

            }

            return record;
        }

        public async Task<String> ProcessAuthLocalMCS(string FilePath)
        {
            if (FilePath == null || FilePath.Length == 0)
                return "File Path not set";
            HttpClient httpClient = new HttpClient();
            List<MCSImport> retlist = new List<MCSImport>();
            string returnobj = "Nan";
            retlist = GetFileasObject(FilePath);

            JavaScriptSerializer jser = new JavaScriptSerializer();
            if (retlist.Count > 0)
            {
                try
                {

                    setAuthorizationHeader(httpClient);

                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

                    string retfiltobj_ser = jser.Serialize(retlist);

                    var obj = await httpClient.PostAsJsonAsync("http://mcs.standardtextile.com/api/CSV", retfiltobj_ser)
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

        private void setAuthorizationHeader(HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "****", "****"))
                        ));
        }

        public List<MCSImport> GetFileasObject(string filepath)
        {

            List<MCSImport> retlist = new List<MCSImport>();
            try
            {
                using (StreamReader sr = new StreamReader(filepath, Encoding.Default))
                {
                    string line;
                    int rowcnter = 0; 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Trim().Length > 0)
                        {
                            retlist.Add(new MCSImport() { LineBlob = line, DocNumber = dlayer.DocNumber, SourceId = CopyToAPI.Form1.thisSource.Id, Location = filepath, Timestamp = DateTime.Now });
                            rowcnter++;
                        }
                    }

                }
            }
            catch (Exception e)
            {

            }


            return retlist;
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
        public sourceDocInfo CallGetdocnumber()
        {
            var task = CallDocAsync();
            
            return task.Result;
        }

        public string callPost()
        {
            var result = ProcessAuthLocalMCS(MCS_API.Helpers.dlayer.mcsaddress);
            result.Wait();
            return result.Result.ToString();
        }

        public string testauthcall()
        {
            var task = ProcessAuthLocalMCS(@"C:\Users\johns\Dropbox (Standard Textile)\APR_Publish\MCS\MCSTesting\LAVORA_CAROLINA.txt");

            return task.Result;
        }
       

        public async Task<sourceDocInfo> CallDocAsync()
        {

            sourceDocInfo docInfo = new sourceDocInfo();
            System.Web.Script.Serialization.JavaScriptSerializer jser = new System.Web.Script.Serialization.JavaScriptSerializer(); 

            using (var client = new HttpClient())
            {
                setAuthorizationHeader(client);
                string resourceaddress = "http://mcs.standardtextile.com/api/CSV/" + CopyToAPI.Form1.thisSource.Id.ToString();

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = await client.GetAsync(resourceaddress, HttpCompletionOption.ResponseHeadersRead)
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

    }
}
