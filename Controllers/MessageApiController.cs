using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ_FactoryDI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MessageApiController : ControllerBase
    {
        private readonly ILogger<MessageApiController> _logger;

        public MessageApiController(ILogger<MessageApiController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetMessage")]
        public IActionResult Get()
        {
            // Retrieve message from RabbitMQ.

            return StatusCode(StatusCodes.Status200OK, "Message");
        }

        [HttpPost(Name = "PostMessage")]
        public IActionResult Post(string message)
        {
            // Send message to RabbitMQ.

            return StatusCode(StatusCodes.Status202Accepted);
        }
    }
}