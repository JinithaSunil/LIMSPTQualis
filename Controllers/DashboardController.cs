using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace LIMS.Controllers
{
    [RoutePrefix("api/dashboard")]
    public class DashboardController : ApiController
    {
        [HttpGet]
        [Route("summary")]
        public IHttpActionResult Summary()
        {
            return Ok("API Working");
        }
    }
}