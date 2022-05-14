using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Web.MPEIEmail;

namespace TelegramBotWebhook.Services
{
    public class UpdateHandleService 
    {
        private readonly ILogger<UpdateHandleService> _logger;
        private readonly ICommandExecuteService<ExecuteResult> _executeService;
        private readonly IMessageSendingService<ExecuteResult> _messageSendingService;

        public UpdateHandleService(ILogger<UpdateHandleService> logger, ICommandExecuteService<ExecuteResult> executeService, IMessageSendingService<ExecuteResult> messageSendingService)
        {
            _logger = logger;
            _executeService = executeService;
            _messageSendingService = messageSendingService;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            var user = update.Message!.From!;
            string messageText = update.Message.Text!;

            if (update.Message.Type != MessageType.Text)
            {
                _logger.LogInformation($"Get a not text message from {user.FirstName} {user.LastName}(ID:{user.Id}) {update.Message.Date}.");
                return;
            }

            var splitedText = messageText.Split(' ');

            ExecuteResult result;
            if (splitedText.First().Contains('/') && splitedText.Count() == 1)
            {
                string command = splitedText.First();

                _logger.LogInformation($"Get the command {command} from {user.FirstName} {user.LastName}(ID:{user.Id}) {update.Message.Date}.");

                result = _executeService.ExecuteCommand(command, user.Id).Result;
            }
            else if (_executeService.ExecuteIsOver())
            {
                _logger.LogInformation($"Get a text message from {user.FirstName} {user.LastName}(ID:{user.Id}) {update.Message.Date}.");

                result = _executeService.ExecuteCommand("/start", user.Id).Result;
            }
            else
            {
                result = _executeService.HandleResponse(messageText, user.Id).Result;
            }

            Chat chat = new Chat(update.Message.Chat.Id);
            await _messageSendingService.SendMessage(chat, result);
        }
    }
}
