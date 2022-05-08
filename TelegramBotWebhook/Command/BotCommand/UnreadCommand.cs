using TelegramBotWebhook.Services;
using TelegramBotWebhook.HtmlParser;
using TelegramBotWebhook.MPEIEmail.EmailEntities;

using AngleSharp;
using System.Text;
using AngleSharp.Html.Dom;
using TelegramBot.Web.MPEIEmail;

namespace TelegramBotWebhook.Command.BotCommand
{
    public class UnreadCommand : BotCommand, IServiceRequired
    {
        public IEnumerable<Type> RequiredServicesTypes => new Type[1] { typeof(IEmailReadService) };
        public List<object> Services { get; } = new List<object>(1);

        public UnreadCommand() : base("/unread") { }

        public override object Clone() => new UnreadCommand();
        public override async Task<ExecuteResult> Execute(string option)
        {
            var emailReadService = (IEmailReadService)Services.First();
            DirectoryInfo lettersDirectory = await emailReadService.ReadLetters();

            var parser = new EmailLettersParser();
            List<Letter> letters = parser.Parse(await GetLettersHtmlFileFromDirectory(lettersDirectory));

            var resultLetters = letters.Where((letter) => !letter.IsRead);

            if (resultLetters.Count() > 0)
            {
                StringBuilder resultMessage = new StringBuilder("The list of your unread letters: \n");
                resultMessage.Append(LettersInfoToString(resultLetters));

                Session session = Session.Instance();
                return new ExecuteResult(ResultType.InlineKeyboarUrl, resultMessage, new string[] { "Link to the MPEI Email", $"https://legacy.mpei.ru/CookieAuth.dll?Logon?curl=Z2Fowa&flags=0&forcedownlevel=0&formdir=2&username={session.Login}&password={session.Password}&isUtf8=1&trusted=4" });
            }
            else
            {
                return new ExecuteResult(ResultType.Text, "Unread letters not found.");
            }
        }
        private async Task<IHtmlDocument> GetLettersHtmlFileFromDirectory(DirectoryInfo directory)
        {
            var html = File.ReadAllText(directory.FullName + @"\Page 1.html");

            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var doc = await context.OpenAsync(req => req.Content(html));

            return (IHtmlDocument)doc;
        }
        private StringBuilder LettersInfoToString(IEnumerable<Letter> letters)
        {
            StringBuilder lettersInfo = new StringBuilder();

            foreach (var letter in letters)
            {
                lettersInfo.AppendLine(Letter.FromTrimDots(letter.From));
                lettersInfo.AppendLine($"{letter.Theme} \n{letter.ReceivingTime.ToString("g")}\n");
            }

            return lettersInfo;
        }
    }
}
