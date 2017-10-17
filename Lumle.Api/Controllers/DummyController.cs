using Lumle.Api.Infrastructures.Abstracts;
using Lumle.Api.Infrastructures.Handlers.ApiResponse;
using Lumle.Api.Infrastructures.Handlers.ApiResponse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace Lumle.Api.Controllers
{

    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DummyController : LumleBaseController
    {

        public DummyController(IActionResponse actionResponse)
            : base(actionResponse)
        {

        }

        /// <summary>
        /// Get dummy data
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult GetDummyData()
        {

            try
            {
                var dummyEmpData = new Employee[]
            {
                new Employee
                {
                    Name = "John",
                    Position = "Software Engineer"
                },
                new Employee
                {
                    Name = "Bob",
                    Position = "Software Engineer II"
                },
                new Employee
                {
                    Name = "Elena",
                    Position = "Program Manager"
                },
                new Employee
                {
                    Name = "Alice",
                    Position = "Designeer"
                }
            };

                return Ok(SuccessNotificationWithData(dummyEmpData, new Message[] { new Message { Title = "Dummy employee records." } }));
            }
            catch (System.Exception)
            {
                return InternalServerError(new[]
                {
                    new Message
                    {
                        Title = "Internal server error."
                    }
                });
            }

        }
    }

    //[ApiVersion("2.0")]
    //[ApiVersion("3.0")]
    //[Route("api/v{version:apiVersion}/dummy")]
    //public class Dummy2Controller : Controller
    //{
    //    [HttpGet]
    //    public string Get() => "Hello world v2!";

    //    //[HttpGet, MapToApiVersion("3.0")]
    //    //public string GetV3() => "Hello world v3!";
    //}


    //Dummy Class
    public class Employee
    {
        public string Name { get; set; }

        public string Position { get; set; }
    }

}
