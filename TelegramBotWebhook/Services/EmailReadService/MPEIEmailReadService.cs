using TelegramBotWebhook.Web.HttpFactories;
using TelegramBotWebhook.Extensions;
using TelegramBotWebhook.MPEIEmail.EmailEntities;
using TelegramBotWebhook.HtmlParser;
using AngleSharp.Html.Dom;

namespace TelegramBotWebhook.Services
{
    public class MPEIEmailReadService : IEmailReadService
    {
        private readonly IHttpFactory httpFactory;
        private readonly EmailProfileConfiguration emailProfile;

        public MPEIEmailReadService(IHttpFactory httpFactory, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            this.httpFactory = httpFactory;
            emailProfile = configuration.GetSection("EmailProfileConfig").Get<EmailProfileConfiguration>();
        }

        public async Task<IEnumerable<Letter>> ReadLetters()
        {
            var response = await GetResponseFromEmail();
            if (response is null)
            {
                return Array.Empty<Letter>();
            }
            else
            {
                var letterParser = new EmailLettersParser();

                List<Letter> letters = letterParser.Parse(response);
                response.Dispose();

                return letters;
            }
        }
        private async Task<IHtmlDocument> GetResponseFromEmail()
        {
            var request = httpFactory.GetRequest();
            var responseHandler = httpFactory.GetResponseHandler();

            var options = new HttpRequestOptions();
            options.GetLoginOptions(emailProfile.Login, emailProfile.Password);

            var response = await request.Send(options);

            return await responseHandler.HandleResponse(response);
        }
    }
}
