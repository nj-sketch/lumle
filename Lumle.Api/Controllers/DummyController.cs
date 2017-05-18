using Microsoft.AspNetCore.Mvc;



namespace Lumle.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DummyController : Controller
    {

        [HttpGet]
        public string Get() => "Hello world v1!";


    }

    [ApiVersion("2.0")]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/dummy")]
    public class Dummy2Controller : Controller
    {
        [HttpGet]
        public string Get() => "Hello world v2!";

        [HttpGet, MapToApiVersion("3.0")]
        public string GetV3() => "Hello world v3!";
    }
}
