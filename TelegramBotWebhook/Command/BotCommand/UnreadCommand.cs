using System.Text;
using TelegramBotWebhook.Services;
using TelegramBotWebhook.Web.MPEIEmail;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Command.BotCommand
{
    public sealed class UnreadCommand : SessionedBotCommand
    {
        private IEmailReadService? _emailReadService;

        internal UnreadCommand() : base("/unread") 
        { 
            AddRequiredServiceType(typeof(IEmailReadService));
        }
       
        protected override void AddNewService(object service)
        {
            if (service is IEmailReadService emailReadService)
            {
                _emailReadService = emailReadService;
            }
        }
        protected override async Task<ExecuteResult> ConcreteExecute(string option)
        {
            if (Session!.UserKey is null)
            {
                return new ExecuteResult(ResultType.Text, "Для использования данной команды необходимо выполнить вход на почту МЭИ.\nВоспользуйтесь командой /login, чтобы войти на почту.");
            }

            var letterRecords = (await _emailReadService!.GetLetters(Session))
                .Where((letter) => !letter.IsRead);

            if (letterRecords.Count() > 0)
            {
                StringBuilder resultMessage = new StringBuilder("Список непрочитанных писем: \n");
                resultMessage.Append(LetterRecordsToString(letterRecords));

                return new ExecuteResult(ResultType.InlineKeyboarUrl, resultMessage, new string[] { "Ссылка на почту МЭИ", $"https://legacy.mpei.ru/CookieAuth.dll?Logon?curl=Z2Fowa&flags=0&forcedownlevel=0&formdir=2&username={Session.Login}&password={Session.Password}&isUtf8=1&trusted=4" }); ;
            }
            else
            {
                return new ExecuteResult(ResultType.Text, "Непрочитанные письма не найдены.");
            }
        }
        private StringBuilder LetterRecordsToString(IEnumerable<LetterRecord> letters)
        {
            StringBuilder lettersInfo = new StringBuilder();

            foreach (var letter in letters)
            {
                lettersInfo.AppendLine(LetterRecord.FromTrimDots(letter.From))
                    .AppendLine($"{letter.Theme} \n{letter.ReceivingTime.ToString("g")}\n");
            }

            return lettersInfo;
        }
    }
}
