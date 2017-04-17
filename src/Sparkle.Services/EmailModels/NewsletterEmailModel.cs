
namespace Sparkle.Services.EmailModels
{
    using System.Collections.Generic;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.Services.Objects;
    using System;
    using Sparkle.UI;
    using Sparkle.Services.Networks.Timelines;
    using Sparkle.Services.Networks.Models;

    public class NewsletterEmailModel : BaseEmailModel
    {
        public NewsletterEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public int contactRequests { get; set; }

        public bool HasNewRegistrants { get; set; }
        public IList<NewRegistrant> NewRegistrants { get; set; }
        public NewRegistrant LastNewRegistrants { get; set; }

        public bool HasEvents { get; set; }
        public IList<EventModel> Events { get; set; }

        public bool HasOtherEvents { get; set; }
        public IList<EventModel> OtherEvents { get; set; }

        public bool HasCompaniesPublications { get; set; }
        public IList<CompanyPublication> CompaniesPublications { get; set; }
        public BasicTimelineItemModel CompaniesTimeline { get; set; }

        public bool HasPeoplePublications { get; set; }
        public IList<PeoplePublication> PeoplePublications { get; set; }
        public BasicTimelineItemModel PeopleTimeline { get; set; }

        public bool HasNewGroups { get; set; }
        public IList<NewGroup> NewGroups { get; set; }

        public IList<Sparkle.Data.Networks.Objects.CompanyExtended> Incubators { get; set; }
        public IList<Sparkle.Data.Networks.Objects.CompanyExtended> Startups { get; set; }
        public IList<Sparkle.Data.Networks.Objects.CompanyExtended> Companies { get; set; }

        public WeeklyMailSubscriber Subscriber { get; set; }

        public int CountNewRegistrants { get; set; }

        public int OtherCountNewRegistrants { get; set; }

        public bool IsCompanyFeatureEnabled { get; set; }

        public string AboutNetwork { get; set; }

        public IList<CompanyModel> NewCompanies { get; set; }

        public IDictionary<short, Networks.Models.CompanyCategoryModel> CompanyCategories { get; set; }

        public bool HasPartnersPublications { get; set; }

        public BasicTimelineItemModel PartnersTimeline { get; set; }

        public int AdsTotal { get; set; }
        public IList<Networks.Ads.AdModel> Ads { get; set; }
    }

    public class NewRegistrant
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string CompanyName { get; set; }
        public string JobName { get; set; }
        public string PictureUrl { get; set; }
    }

    public class PeoplePublication
    {
        public int Id { get; set; }
        public User PostedBy { get; set; }
        public string Text { get; set; }
        public string PictureUrl { get; set; }
        public DateTime DateCreated { get; set; }
        public int LikeCount { get; set; }
        public string CommentCount { get; set; }
    }

    public class CompanyPublication
    {
        public string CompanyName { get; set; }
        public string CompanyAlias { get; set; }
        public string CompanyPictureUrl { get; set; }
        public string Publication { get; set; }

        public int PublicationId { get; set; }

        public int LikeCount { get; set; }

        public string CommentCount { get; set; }

        public DateTime DateCreated { get; set; }
    }

    public class NewGroup
    {
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
    }
}
