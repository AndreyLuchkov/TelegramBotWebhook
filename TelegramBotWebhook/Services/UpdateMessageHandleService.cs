using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotWebhook.Services
{
    public class UpdateMessageHandleService 
    {
        private readonly ILogger<UpdateMessageHandleService> _logger;
        private readonly ICommandExecuteService<ExecuteResult> _commandExecuteService;
        private readonly IMessageSendingService<ExecuteResult> _messageSendingService;

        public UpdateMessageHandleService(ILogger<UpdateMessageHandleService> logger, ICommandExecuteService<ExecuteResult> executeService, IMessageSendingService<ExecuteResult> messageSendingService)
        {
            _logger = logger;
            _commandExecuteService = executeService;
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

            ExecuteResult result;
            if (IsCommandOnly(messageText))
            {
                string command = messageText.Split(' ').First();

                _logger.LogInformation($"Get the command {command} from {user.FirstName} {user.LastName}(ID:{user.Id}) {update.Message.Date}.");

                result = _commandExecuteService.ExecuteCommand(command, user.Id).Result;
            }
            else if (_commandExecuteService.IsExecuteOver(user.Id))
            {
                _logger.LogInformation($"Get a text message from {user.FirstName} {user.LastName}(ID:{user.Id}) {update.Message.Date}.");

                result = _commandExecuteService.ExecuteCommand("/start", user.Id).Result;
            }
            else
            {
                result = _commandExecuteService.HandleResponse(messageText, user.Id).Result;
            }

            Chat chat = new Chat(update.Message.Chat.Id);
            await _messageSendingService.SendMessage(chat, result);
        }
        private bool IsCommandOnly(string message) => message.Split(' ').First().Contains('/') && message.Split(' ').Count() == 1;
    }
}
