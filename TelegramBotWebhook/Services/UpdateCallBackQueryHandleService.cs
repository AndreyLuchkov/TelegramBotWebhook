using Telegram.Bot.Types;

namespace TelegramBotWebhook.Services
{
    public class UpdateCallBackQueryHandleService
    {
        private readonly ICommandExecuteService<ExecuteResult> _commandExecuteService;
        private readonly TelegramMessageEditService _messageEditService;

        public UpdateCallBackQueryHandleService(ICommandExecuteService<ExecuteResult> commandExecuteService, TelegramMessageEditService messageEditService)
        {
            _commandExecuteService = commandExecuteService;
            _messageEditService = messageEditService;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            var user = update.CallbackQuery!.From;
            var callback = update.CallbackQuery.Data!;

            if (callback.First() == ';' || callback.First() == '/')
            {
                var result = _commandExecuteService.ExecuteCommand(callback, user.Id);

                await _messageEditService.EditMessage(update.CallbackQuery.Message!, await result);
            }
        }
    }
}
