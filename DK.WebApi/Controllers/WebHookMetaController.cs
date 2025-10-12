using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DK.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebHookMetaController : ControllerBase
    {
        [HttpGet("Event")]
        public string Event(
            [FromQuery(Name = "hub.mode")] string mode, 
            [FromQuery(Name = "hub.challenge")] string challenge, 
            [FromQuery(Name = "hub.verify_token")] string verify_token)
        {
            if (verify_token.Equals("TEST"))
                return challenge;
            return string.Empty;
        }

        [HttpPost("Event")]
        public IActionResult Event([FromBody] dynamic data)
        {
            var result = JsonSerializer.Serialize(data);
            return Ok();
        }
    }
}
