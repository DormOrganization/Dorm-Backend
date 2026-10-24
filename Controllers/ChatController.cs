using DormAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DormAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly OllamaService _ollamaService;

        public ChatController(OllamaService ollamaService) {
            _ollamaService = ollamaService;
        }

        [HttpPost("ask/function-calling")]
        public async Task<IActionResult> AskAi([FromBody] string prompt)
        {
            // prompt = "find the unpaid student for this month"
            var response = await _ollamaService.ProcessChatAsync(DateTime.Now.ToString("yyyy-MM-dd") + prompt);
            return Ok(response);
        }

        [HttpPost("ask/generate-sql")]
        public async Task<IActionResult> GenerateSql([FromBody] string prompt)
        {
            // prompt = "find the unpaid student for this month"
            var response = await _ollamaService.ProcessChatSqlAcync( prompt);
            return Ok(response);
        }
    }
}
