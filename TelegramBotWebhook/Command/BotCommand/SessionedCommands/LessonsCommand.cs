using System.Text;
using TelegramBotWebhook.Services;
using TelegramBotWebhook.Web.MPEIEmail.EmailEntities;

namespace TelegramBotWebhook.Command.BotCommand
{
    public sealed class LessonsCommand : SessionedBotCommand
    {
        private IEmailReadService? _emailReadService;
        private IEmailLetterReadService<LessonLetter>? _emailLetterReadService;

        internal LessonsCommand() : base("/", "lessons")
        {
            AddRequiredServiceTypes(new[] { typeof(IEmailReadService), typeof(IEmailLetterReadService<LessonLetter>) });
        }

        protected override void AddNewService(object service)
        {
            if (service is IEmailReadService emailReadService)
            {
                _emailReadService = emailReadService;
            }
            else if (service is IEmailLetterReadService<LessonLetter> emailLetterReadService)
            {
                _emailLetterReadService = emailLetterReadService;
            }
        }
        protected override async Task<ExecuteResult> ConcreteExecute(string option)
        {
            var letterRecords = _emailReadService!.GetLetters(Session);

            var lessonLetters = _emailLetterReadService!.ReadLetters(
                Session,
                (await letterRecords)
                    .Where((letterRecord) => (letterRecord.Type == "IPM.Schedule.Meeting.Request"
                                              || letterRecord.From.Contains("messenger@webex"))
                                              && !letterRecord.Theme.Contains("Присоединяйтесь")
                                              && !letterRecord.Theme.Contains("Напоминание")
                                              && !letterRecord.Theme.Contains("Сеанс обучения отменен")));

            var actualLessonLetters = (await lessonLetters)
                                        .Where((lessonLetters) => lessonLetters.SessionLink != String.Empty
                                                                  && lessonLetters.LessonStartDate > DateTime.Now.AddHours(-1).AddMinutes(-30));

            if (actualLessonLetters.Count() == 0)
            {
                return new ExecuteResult(ResultType.Text, "Писем с приглашением на занятие не найдено.");
            }

            StringBuilder resultMessage = new StringBuilder("Письма с приглашением на занятие: \n");

            AppendLessonLetters(
                resultMessage, 
                actualLessonLetters
                    .OrderByDescending(lessonLetter => lessonLetter.LessonStartDate));

            return new ExecuteResult(ResultType.Text, resultMessage);
        }
        private void AppendLessonLetters(StringBuilder resultMessage, IEnumerable<LessonLetter> lessonLetters)
        {
            if (IsCurrentLesson(lessonLetters.First()))
            {
                resultMessage.AppendLine("<b>Текущее занятие</b>");
            }

            foreach (var lessonLetter in lessonLetters)
            {
                
                resultMessage.AppendLine(lessonLetter.Teacher)
                    .AppendLine(lessonLetter.LessonStartDate.ToString())
                    .AppendLine(lessonLetter.SessionLink);
                
                if (lessonLetter.SessionNumber != String.Empty && lessonLetter.SessionPassword != String.Empty)
                {
                    resultMessage.AppendFormat("{0} | {1}\n", lessonLetter.SessionNumber, lessonLetter.SessionPassword);
                }

                resultMessage.AppendLine("--------------------------------");
            }
        }
        private bool IsCurrentLesson(LessonLetter lessonLetter) => lessonLetter.LessonStartDate < DateTime.Now && DateTime.Now < lessonLetter.LessonStartDate.AddHours(1).AddMinutes(30);
    }
}
