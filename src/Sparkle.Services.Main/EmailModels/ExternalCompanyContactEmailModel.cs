
namespace Sparkle.Services.Main.EmailModels
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class ExternalCompanyContactEmailModel : BaseEmailModel
    {
        public ExternalCompanyContactEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string FromUserName { get; set; }

        public string FromUserEmail { get; set; }

        public string FromCompanyName { get; set; }

        public string Message { get; set; }

        public string ResponseUrl { get; set; }
    }
}
