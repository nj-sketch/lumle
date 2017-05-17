using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lumle.AuthServer.Models.Token;
using Lumle.AuthServer.Models.ApiResponse;
using System.Net;
using Lumle.AuthServer.Data.Store;

namespace Lumle.AuthServer.Controllers
{
    [Route("api")]
    public class TokenController : Controller
    {
        private readonly ITokenSnapShotStore _tokenSnapShotStore;

        public TokenController(ITokenSnapShotStore tokenSnapShotStore)
        {
            _tokenSnapShotStore = tokenSnapShotStore;
        }
        
        
        [HttpPost("revoketoken")]
        public async Task<IActionResult> Post([FromBody]RevokeToken model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new ApiResponse { Status = "error", Message = "Please add both Jti Token and subjectId." });
                }


                await _tokenSnapShotStore.UpdateUserStatusAsync(model.SubId);

                return StatusCode((int)HttpStatusCode.OK, new ApiResponse { Status = "success", Message = "Access token has been revoked." });

            }
            catch (Exception)
            {
                return StatusCode( (int)HttpStatusCode.InternalServerError, new ApiResponse { Status = "error", Message = "Unable to revoke access token." });
            }

        }
    }
}
