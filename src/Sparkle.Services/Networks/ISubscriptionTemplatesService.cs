
namespace Sparkle.Services.Networks
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Subscriptions;
    using Sparkle.Services.Networks.Texts;
    using System.Globalization;

    public interface ISubscriptionTemplatesService
    {
        SubscriptionTemplateModel GetById(int id);
        IList<SubscriptionTemplateModel> GetAll();

        IList<SubscriptionTemplateModel> GetUserSubscribable();

        SubscriptionTemplateModel GetDefaultUserTemplate();

        EditTextRequest GetEditTextRequest(int subscriptionTemplatesId, string culture, string templateName);
        EditTextRequest GetEditTextRequest(int subscriptionTemplatesId, CultureInfo culture, string templateName);
        EditTextResult SaveTextTemplate(EditTextRequest request);

        IDictionary<string, IList<EditTextRequest>> GetTextsByTemplateId(int templateId);
    }
}
