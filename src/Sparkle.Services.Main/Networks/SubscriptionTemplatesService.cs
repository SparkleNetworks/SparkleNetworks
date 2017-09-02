
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Profile;
    using SrkToolkit.Domain;
    using Sparkle.Services.Networks.Subscriptions;
    using Sparkle.Services.Networks.Texts;
    using System.Globalization;

    public class SubscriptionTemplatesService : ServiceBase, ISubscriptionTemplatesService
    {
        private const string TextTypeConfirm = "Confirm";
        private const string TextTypeRenew = "Renew";
        private const string TextTypeExpire = "Expire";
        private readonly string[] TextTypes = { TextTypeConfirm, TextTypeRenew, TextTypeExpire, };

        [DebuggerStepThrough]
        internal SubscriptionTemplatesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public IList<SubscriptionTemplateModel> GetAll()
        {
            return this.Repo.SubscriptionTemplates.GetAll(this.Services.NetworkId)
                .Select(x => new SubscriptionTemplateModel(x))
                .ToList();
        }

        public IList<SubscriptionTemplateModel> GetUserSubscribable()
        {
            return this.Repo.SubscriptionTemplates.GetUserSubscribable(this.Services.NetworkId)
                .Select(x => new SubscriptionTemplateModel(x))
                .ToList();
        }

        public SubscriptionTemplateModel GetDefaultUserTemplate()
        {
            var item = this.Repo.SubscriptionTemplates.GetDefaultUserTemplate(this.Services.NetworkId);
            if (item == null)
                return null;

            return new SubscriptionTemplateModel(item);
        }

        public SubscriptionTemplateModel GetById(int id)
        {
            var item = this.Repo.SubscriptionTemplates.GetById(id);
            if (item == null)
                return null;

            return new SubscriptionTemplateModel(item);
        }

        public EditTextRequest GetEditTextRequest(int subscriptionTemplateId, string culture, string templateName)
        {
            return this.GetEditTextRequest(subscriptionTemplateId, new CultureInfo(culture), templateName);
        }

        public EditTextRequest GetEditTextRequest(int subscriptionTemplateId, CultureInfo culture, string templateName)
        {
            var item = this.Repo.SubscriptionTemplates.GetById(subscriptionTemplateId);
            if (item == null)
                return null;

            EditTextRequest request;
            switch (templateName)
            {
                case TextTypeConfirm:
                    request = this.Services.Text.GetEditRequest(item.ConfirmEmailTextId, culture);
                    break;

                case TextTypeRenew:
                    request = this.Services.Text.GetEditRequest(item.RenewEmailTextId, culture);
                    break;

                case TextTypeExpire:
                    request = this.Services.Text.GetEditRequest(item.ExpireEmailTextId, culture);
                    break;

                default:
                    throw new ArgumentException("Invalid templateName '" + templateName + "'", "templateName");
            }
            
            if (request == null)
                return null;

            if (!this.TextTypes.Contains(templateName))
                return null;

            request.TargetId = item.Id;
            request.TargetName = templateName;

            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Value))
            {
                request.Title = this.Services.Lang.T(culture, "Subscriptions: " + templateName + "TextTemplateTitle");
                request.Value = this.Services.Lang.T(culture, "Subscriptions: " + templateName + "TextTemplateValue");
            }

            request.Rules = this.GetUserFriendlyRules(culture);

            return request;
        }

        // Add Lang rules and format date in this function
        private IDictionary<string, string> GetUserFriendlyRules(CultureInfo culture)
        {
            var dateRules = new string[]
            {
                culture.DateTimeFormat.ShortDatePattern,
                culture.DateTimeFormat.LongDatePattern,
            };
            var now = DateTime.UtcNow;

            var genericRules = this.Services.Subscriptions.GetEmailSubstitutionRules();
            var friendlyRules = new Dictionary<string, string>();

            foreach (var item in genericRules)
            {
                if (item.Key == "DateBeginUtc" || item.Key == "DateEndUtc")
                {
                    friendlyRules.Add(item.Key, "");
                    dateRules.ToList().ForEach(o => friendlyRules.Add(item.Key + " " + o, now.ToString(o, culture)));
                }
                else
                {
                    friendlyRules.Add(item.Key, this.Services.Lang.T("TextTemplateVars: " + item.Key));
                }
            }

            return friendlyRules;
        }

        public EditTextResult SaveTextTemplate(EditTextRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new EditTextResult(request);

            var item = this.Repo.SubscriptionTemplates.GetById(request.TargetId);
            if (item == null)
            {
                result.Errors.Add(EditTextError.NoSuchSubscriptionTemplate, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // DAFUKK
            ////request.TextId = item.ConfirmEmailTextId;

            switch (request.TargetName)
            {
                case TextTypeConfirm:
                    request.TextId = item.ConfirmEmailTextId;
                    break;

                case TextTypeRenew:
                    request.TextId = item.RenewEmailTextId;
                    break;

                case TextTypeExpire:
                    request.TextId = item.ExpireEmailTextId;
                    break;

                default:
                    throw new ArgumentException("Invalid templateName '" + request.TargetName + "'", "request.TargetName");
            }

            result = this.Services.Text.SaveText(request);

            if (result.Succeed && request.TextId == null)
            {
                switch (request.TargetName)
                {
                    case TextTypeConfirm:
                        item.ConfirmEmailTextId = result.InsertedTextId;
                        break;

                    case TextTypeRenew:
                        item.RenewEmailTextId = result.InsertedTextId;
                        break;

                    case TextTypeExpire:
                        item.ExpireEmailTextId = result.InsertedTextId;
                        break;

                    default:
                        throw new ArgumentException("Invalid templateName '" + request.TargetName + "'", "request.TargetName");
                }

                this.Repo.SubscriptionTemplates.Update(item);
            }

            return result;
        }

        public IDictionary<string, IList<EditTextRequest>> GetTextsByTemplateId(int templateId)
        {
            var template = this.GetById(templateId);
            if (template == null)
                return null;

            var subModel = new SubscriptionEmailModel
            {
                Template = template,
                NetworkName = this.Services.Network.Name,
                NetworkDomain = this.Services.Lang.T("AppDomain"),
            };
            var rules = this.Services.Subscriptions.GetEmailSubstitutionRules();

            var textTemplates = new Dictionary<string, IList<EditTextRequest>>();
            foreach (var item in template.Texts)
            {
                var texts = this.Services.Text.GetAllByTextId(item.Id);
                foreach (var text in texts)
                {
                    text.TargetId = template.Id;
                    if (string.IsNullOrEmpty(text.Title) || string.IsNullOrEmpty(text.Value))
                    {
                        var cultureInfo = new CultureInfo(text.CultureName);
                        text.Title = this.Services.Lang.T(cultureInfo, "Subscriptions: " + item.Name + "TextTemplateTitle");
                        text.Value = this.Services.Lang.T(cultureInfo, "Subscriptions: " + item.Name + "TextTemplateValue");
                    }

                    // tests substitutions
                    text.Title = this.Services.I18N.ReplaceVariables(text.Title, subModel, rules);
                    text.Value = this.Services.I18N.ReplaceVariables(text.Value, subModel, rules);
                }

                textTemplates.Add(item.Name, texts);
            }

            return textTemplates;
        }
    }
}
