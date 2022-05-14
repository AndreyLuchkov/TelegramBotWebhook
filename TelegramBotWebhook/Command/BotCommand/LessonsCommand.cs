using System.Text;
using TelegramBot.Web.MPEIEmail;
using TelegramBotWebhook.MPEIEmail.EmailEntities;
using TelegramBotWebhook.Services;

namespace TelegramBotWebhook.Command.BotCommand
{
    internal class LessonsCommand : BotCommand, IServiceRequired, ISessionDepended
    {
        private List<object> _services;
        public IEnumerable<Type> RequiredServicesTypes { get; }
        public Session? Session { get; set; }

        internal LessonsCommand() : base("/lessons")
        {
            RequiredServicesTypes = new Type[2] { typeof(IEmailReadService), typeof(IEmailLetterReadService<LessonLetter>) };
            _services = new List<object>(2);
        }

        public void AddService(object service)
        {
            if (_services.Count != RequiredServicesTypes.Count())
                _services.Add(service);
        }
        public override async Task<ExecuteResult> Execute(string option)
        {
            if (Session is null)
                throw new InvalidOperationException("The session property is null.");
            if (Session.Login is null || Session.Password is null)
            {
                return new ExecuteResult(ResultType.Text, "Для использования данной команды необходима аутентификация. Воспользуйтесь командой /login, чтобы иметь доступ к почте МЭИ.");
            }

            var emailReader = (IEmailReadService)_services.First();

            var letters = emailReader.GetLetters(Session);

            var letterReader = (IEmailLetterReadService<LessonLetter>)_services.TakeLast(1).First();

            var lessonLetters = letterReader.ReadLetters(
                Session, 
                (await letters)
                    .Where((letter) => letter.Type == "IPM.Schedule.Meeting.Request"));

            StringBuilder resultMessage = new StringBuilder("Письма с приглашением на занятие: \n");

            foreach (var lessonLetter in await lessonLetters)
            {
                resultMessage.AppendLine(lessonLetter.Teacher)
                    .AppendLine(lessonLetter.LessonStartDate.ToString())
                    .AppendLine(lessonLetter.SessionLink)
                    .AppendLine();
            }

            return new ExecuteResult(ResultType.Text, resultMessage);
        }
    }
}
