
namespace Sparkle.Services.Main.EmailModels
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class CompanyContactEmailModel : BaseEmailModel
    {
        public CompanyContactEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public User ToUser { get; set; }
        
        public Company ToCompany { get; set; }

        public string FromUserName { get; set; }

        public string FromUserEmail { get; set; }

        public string FromCompanyName { get; set; }

        public string Message { get; set; }

        public string ConversationUrl { get; set; }
    }
}
