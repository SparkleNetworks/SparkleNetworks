
namespace Sparkle.UnitTests.Mocks
{
    using Sparkle.Services.EmailTemplates;
    using System.Collections.Generic;

    public class SimpleEmailModel : BaseEmailModel
    {
        public SimpleEmailModel()
            : base(default(string), string.Empty, Sparkle.UI.Lang.Source)
        {
        }

        public SimpleEmailModel(Entities.Networks.Neutral.SimpleContact contact, string p, UI.Strings strings)
            : base(contact, p, strings)
        {
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public IList<SimpleEmailModel> Items { get; set; }
    }
}
