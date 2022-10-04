using Microsoft.AspNetCore.Mvc;
using RabbitMQ_FactoryDI.MQFactory;

namespace RabbitMQ_FactoryDI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MessageApiController : ControllerBase
    {
        private readonly ILogger<MessageApiController> _logger;
        private readonly IMessageQueueFactory _messageQueues;

        public MessageApiController(ILogger<MessageApiController> logger, IMessageQueueFactory messageQueues)
        {
            _logger = logger;
            _messageQueues = messageQueues;
        }

        /// <summary>
        /// Attempts to fetch a single message from RabbitMQ.
        /// </summary>
        /// <returns>A message string if there was one available, otherwise null.</returns>
        /// <response code="200">Returns a message.</response>
        /// <response code="204">A message was not available.</response>
        [HttpGet(Name = "GetMessage")]
        public IActionResult Get()
        {
            // Retrieve message from RabbitMQ.
            var q = _messageQueues.GetQueue("MyReceiveQueue");
            var message = q.Get();

            if (message != null)
                return StatusCode(StatusCodes.Status200OK, message);

            return StatusCode(StatusCodes.Status204NoContent);
        }

        /// <summary>
        /// Publishes a string message to RabbitMQ.
        /// </summary>
        /// <returns></returns>
        /// <response code="202">The message was accepted.</response>
        [HttpPost(Name = "PostMessage")]
        public IActionResult Post(string message)
        {
            // Send message to RabbitMQ.
            var q = _messageQueues.GetQueue("MySendQueue");
            q.Publish(message);

            return StatusCode(StatusCodes.Status202Accepted);
        }
    }
}