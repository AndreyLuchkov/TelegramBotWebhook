using TelegramBotWebhook.Web.HttpFactories;
using TelegramBotWebhook.Extensions;

namespace TelegramBotWebhook.Services
{
    public class MPEIEmailReadService : IEmailReadService
    {
        public readonly EmailProfileConfiguration emailProfile;
        private readonly IHttpFactory httpFactory;

        public MPEIEmailReadService(IHttpFactory httpFactory, IConfiguration configuration)
        {
            this.httpFactory = httpFactory;
            emailProfile = configuration.GetSection("EmailProfileConfig").Get<EmailProfileConfiguration>();
        }

        public async Task<DirectoryInfo> ReadLetters()
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
