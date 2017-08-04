using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lumle.Api.Infrastructures.Handlers.ApiResponse;
using Lumle.Api.Infrastructures.Handlers.ApiResponse.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lumle.Api.Infrastructures.Abstracts
{

    /// <summary>
    /// Base Controller which holds functions common for all API Controller
    /// </summary>
    public class LumleBaseController : Controller
    {
        private readonly IActionResponse _actionResponse;

        public LumleBaseController(IActionResponse actionResponse)
        {
            _actionResponse = actionResponse;
        }


        /// <summary>
        /// Prepare Internal Server Error Response
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public ObjectResult InternalServerError(IEnumerable<Message> messages)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                _actionResponse.GetResponse(null,
                    messages));
        }

        /// <summary>
        /// Prepare Bad Reequest Response
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public ObjectResult BadRequest(IEnumerable<Message> messages)
        {
            return BadRequest(_actionResponse.GetResponse(null, messages));
        }

        /// <summary>
        /// Prepare Success Response with Notification Message
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public ObjectResult SuccessNotification(IEnumerable<Message> messages)
        {
            return Ok(_actionResponse.GetResponse(null, null, messages));
        }

        /// <summary>
        /// Prepare Success Response with Nontification message and data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public ObjectResult SuccessNotificationWithData(object data, IEnumerable<Message> messages)
        {
            return Ok(_actionResponse.GetResponse(data, messages));
        }

        /// <summary>
        /// Prepare success response with data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ObjectResult SuccessWithData(object data)
        {
            return Ok(_actionResponse.GetResponse(data));
        }
    }
}
