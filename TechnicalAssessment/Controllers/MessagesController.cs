using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TechnicalAssessment.Data;
using TechnicalAssessment.Model;

namespace TechnicalAssessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly RabbitMQService _rabbitMQService;
        private readonly DtechLoggerContext _context;
        public MessagesController(ILogger<MessagesController> logger, DtechLoggerContext context)
        {
            _logger = logger;
            _rabbitMQService = new RabbitMQService();
            _context = context;
        }

        [HttpPost(Name = "WriteMessage")]
        public IActionResult WriteMessages(Request myRequest)
        {
            try
            {
                _rabbitMQService.SendMessage(JsonConvert.SerializeObject(myRequest));
                MyLogger logger = new MyLogger
                {
                    Originator = myRequest.Originator,
                    FileName = myRequest.FileName,
                    LogDate = DateTime.Now,
                    Status = "Received"
                };
                _context.MyLoggers.Add(logger);
                _context.SaveChanges();
                return Ok(new {returnCode=0,fileName=myRequest.FileName,status= "Succesfully Processed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while writing message");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

           
        }
    }
}
