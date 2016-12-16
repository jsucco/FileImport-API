using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MCS.Controllers
{
    public class SourcesController : ApiController
    {
        // GET: api/Sources
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Sources/5
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Helpers.CustomAuthenication]
        public HttpResponseMessage Get(string MachineName)
        {
            HttpResponseMessage response = ControllerContext.Request.CreateResponse(HttpStatusCode.Created, "");

            response.Content = new StringContent(Helpers.SourcesAdapter.getSourceId(MachineName).ToString());

            return response;
        }

        // POST: api/Sources
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Helpers.CustomAuthenication]
        public HttpResponseMessage Post([FromBody]string value)
        {
            HttpResponseMessage response = ControllerContext.Request.CreateResponse(HttpStatusCode.Created, "");

            response.Content = new StringContent(Helpers.SourcesAdapter.AddSource(value).ToString());

            return response;
        }

        // PUT: api/Sources/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Sources/5
        public void Delete(int id)
        {
        }
    }
}
