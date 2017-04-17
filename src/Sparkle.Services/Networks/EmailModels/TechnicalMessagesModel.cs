
namespace Sparkle.Services.Networks.EmailModels
{
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TechnicalMessagesModel : BaseEmailModel
    {
        public TechnicalMessagesModel(string recipientEmailAddress, string networkAccentColor, Strings lang)
            : base(recipientEmailAddress, networkAccentColor, lang)
        {
        }

        public IList<string> Messages { get; set; }
        public IList<Person> Recipients { get; set; }
    }
}
