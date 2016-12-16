using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;


namespace MCS.Controllers
{
    [AllowAnonymous]
    public class CSVController : ApiController
    {
        // GET: api/CSV
        public IEnumerable<string> Get(string id)
        {
            int sourceint = Convert.ToInt32(id);
            JavaScriptSerializer jser = new JavaScriptSerializer();
            Models.sourceDocInfo docInfo = new Models.sourceDocInfo();
            HttpResponseMessage response = ControllerContext.Request.CreateResponse(System.Net.HttpStatusCode.Created, "true");
            if (sourceint > 0)
            {
                try
                {
                    int newDocNumber = -1;
                    using (var _db = new Models.BleacherDb())
                    {
                        var importRecord = (from x in _db.MCSImports where x.SourceId == sourceint orderby x.id descending select x).Take(1);
                        if (importRecord != null && importRecord.Count() > 0)
                        {
                            newDocNumber = importRecord.ToArray()[0].DocNumber + 1 ?? 1;
                            docInfo.LastPath = importRecord.ToArray()[0].Location;
                        }
                        else
                        {
                            newDocNumber = 1;
                            docInfo.LastPath = "";
                        }
                        docInfo.DocNumber = newDocNumber;

                    }
                }
                catch (Exception e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                }
            }

            return new string[] { docInfo.DocNumber.ToString(), docInfo.LastPath };
        }

        // GET: api/CSV/5
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Helpers.CustomAuthenication]
        [HttpGet]
        public HttpResponseMessage Get(int SourceId)
        {
            int sourceint = Convert.ToInt32(SourceId); 
            JavaScriptSerializer jser = new JavaScriptSerializer(); 
            Models.sourceDocInfo docInfo = new Models.sourceDocInfo(); 
            HttpResponseMessage response = ControllerContext.Request.CreateResponse(System.Net.HttpStatusCode.Created, "true");
            if (sourceint > 0)
            {
                try
                {
                    int newDocNumber = -1;
                    using (var _db = new Models.BleacherDb())
                    {
                        var importRecord = (from x in _db.MCSImports where x.SourceId == sourceint orderby x.id descending select x).Take(1);
                        if (importRecord != null && importRecord.Count() > 0)
                        {
                            newDocNumber = importRecord.ToArray()[0].DocNumber + 1 ?? 1;
                            docInfo.LastPath = importRecord.ToArray()[0].Location;
                        }
                        else
                        {
                            newDocNumber = 1;
                            docInfo.LastPath = "";
                        }
                        docInfo.DocNumber = newDocNumber;
                        
                    }
                }
                catch (Exception e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                }
            }
            response.Content = new StringContent(jser.Serialize(docInfo));

           return response;
           
        }

        // POST: api/CSV
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Helpers.CustomAuthenication]
        public HttpResponseMessage Post([FromBody]string contents)
        {
            HttpResponseMessage response = ControllerContext.Request.CreateResponse(HttpStatusCode.Created, contents);
            Int32 rowsAffected = 0; 

            try
            {
                if (contents != null && contents.Length > 0)
                {
                    rowsAffected = MCS.Helpers.MCSAdapter.updateContents(contents);
                }

                response.Content = new StringContent(rowsAffected.ToString());
            } catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            return response;
        }

        // PUT: api/CSV/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CSV/5
        public void Delete(int id)
        {
        }
    }
}
