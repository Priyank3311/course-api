using Course.DataModel.Dtos.RequestDTOs;
using Course.Services.Common;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/chat")]
public class ChatController(ChatGptService _chatService) : ControllerBase
{

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] ChatRequestDto request)
    {
        string message = request.Prompt?.ToLower() ?? "";

        if (message.Contains("how many courses"))
        {

            return Ok($"📚 There are 5 courses available.");
        }
        else if (message.Contains("hello") || message.Contains("hi"))
        {
            return Ok("👋 Hello! How can I help you today?");
        }
        else
        {
            return Ok("🤖 Sorry, I didn't understand. Try asking about courses or enrollment.");
        }
    }
}