using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TelegramBotWebhook.Services;

namespace TelegramBotWebhook.Controllers
{
    public class BotController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> WebhookPost([FromBody] Update update, [FromServices] UpdateHandleService updateHandleService, [FromServices] ISessionService sessionService)
        {
            sessionService.StartSession(update.Message!.From!.Id);
            
            await updateHandleService.HandleUpdateAsync(update);

            return Ok();
        }
    }
}
