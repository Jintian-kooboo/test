using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public ActionResult test()
        {
            var a = 1;
            var b = 0;
            var c = a / b;
            var d = 1;
            return Content("dfgdfg");
        }
    }
}
