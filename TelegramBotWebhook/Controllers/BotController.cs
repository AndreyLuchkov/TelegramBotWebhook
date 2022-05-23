using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TelegramBotWebhook.Services;

namespace TelegramBotWebhook.Controllers
{
    public class BotController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> WebhookPost([FromBody] Update update, 
            [FromServices] UpdateMessageHandleService updateMessageHandleService,
            [FromServices] UpdateCallBackQueryHandleService updateCallBackQueryHandleService,
            [FromServices] ISessionService<MPEISession> sessionService)
        {
            if (update.Message is not null)
                await sessionService.StartSession(update.Message.From!.Id);

            var updateHandler = update.Type switch
            {
                Telegram.Bot.Types.Enums.UpdateType.Message => updateMessageHandleService.HandleUpdateAsync(update),
                Telegram.Bot.Types.Enums.UpdateType.CallbackQuery => updateCallBackQueryHandleService.HandleUpdateAsync(update),
                _ => throw new Exception("The unknown update type.")
            };

            await updateHandler;
            return Ok();
        }
    }
}
