using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.Extensions.Logging;
using Lumle.Api.Data.Entities;

namespace Lumle.Api.Controllers
{
    [Route("[controller]")]
    public class PlaceController : JsonApiController<Place>
    {
        private readonly IResourceService<Place> _resourceService;

        public PlaceController(IJsonApiContext jsonApiContext, IResourceService<Place> resourceService, ILoggerFactory loggerFactory) : base(jsonApiContext, resourceService, loggerFactory)
        {

            _resourceService = resourceService;

        }




        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            // custom code
            if (RequestIsValid() == false)
                return BadRequest();

            var entity = await _resourceService.GetAsync(id);

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        // some custom validation logic
        private bool RequestIsValid() => true;

    }
}

