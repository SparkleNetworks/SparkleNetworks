
namespace Sparkle.Services.Networks.Timelines
{
    using Newtonsoft.Json;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.Users;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BasicTimelineItemModel
    {
        public BasicTimelineItemModel()
        {
            this.ApplyColors(TimelineType.Profile);
        }

        public bool IsRootNode { get; set; }
        
        public IList<BasicTimelineItemModel> Items { get; set; }

        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime DateUtc { get; set; }

        public string ForegroundColor { get; set; }
        public string BackgroundColor { get; set; }

        public string PostedByName { get; set; }
        public string PostedByUrl { get; set; }
        public string PostedByPictureUrl { get; set; }

        public string PostedIntoContainerName { get; set; }
        public string PostedIntoName { get; set; }
        public string PostedIntoUrl { get; set; }
        public string PostedIntoPictureUrl { get; set; }
        public string PostedIntoVerb { get; set; }

        public string Text { get; set; }

        public int? LikesCount { get; set; }
        public int? CommentsCount { get; set; }

        public TimelineType Type { get; set; }

        public IList<BasicTimelineItemAttachmentModel> Attachments { get; set; }

        public bool IsHidden { get; set; }
        public bool IsUserAuthorized { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is highlighted]. This avoids trimming content and shows which element is important.
        /// </summary>
        public bool IsHighlighted { get; set; }

        public bool IsVisible
        {
            get { return !this.IsHidden && this.IsUserAuthorized; }
        }

        public void Fill(IServiceFactory services, TimelineItem item, IEnumerable<TimelineItemComment> comments, Event evt = null, Group group = null, Company company = null, Place place = null, Project project = null, Team team = null, Ad ad = null, PartnerResource partnerResource = null)
        {
            this.FillItem(services, item, evt, group, company, place, project, team, ad, partnerResource);
            this.FillComments(services, comments, item);
        }

        private void FillComments(IServiceFactory services, IEnumerable<TimelineItemComment> comments, TimelineItem item)
        {
            this.Items = new List<BasicTimelineItemModel>();
            if (comments != null)
            {
                foreach (var comment in comments.OrderBy(i => i.CreateDate))
                {
                    var model = new BasicTimelineItemModel();
                    model.FillComment(services, comment, item);
                    this.Items.Add(model);
                }
            }

            this.LikesCount = services.Wall.CountLikes(item.Id);
            this.CommentsCount = services.WallComments.CountByItem(item.Id);

            var commentLikes = services.WallComments.CountLikes(this.Items.Select(i => i.Id).ToArray());
            foreach (var comment in this.Items)
            {
                if (commentLikes.ContainsKey(comment.Id))
                    comment.LikesCount = commentLikes[comment.Id];
                else
                    comment.LikesCount = 0;
            }
        }

        private void FillItem(IServiceFactory services, TimelineItem item, Event evt = null, Group group = null, Company company = null, Place place = null, Project project = null, Team team = null, Ad ad = null, PartnerResource partnerResource = null)
        {
            TimelineType type;
            services.Wall.TryGetTimelineType(item, out type);

            this.Type = type;
            this.PostedByName = item.PostedBy.FirstName + " " + item.PostedBy.LastName;
            this.PostedByPictureUrl = services.People.GetProfilePictureUrl(item.PostedBy, UserProfilePictureSize.Small, UriKind.Absolute);
            this.PostedByUrl = services.People.GetProfileUrl(item.PostedBy, UriKind.Absolute);

            this.ApplyColors(type);

            switch (type)
            {
                case TimelineType.Company:
                case TimelineType.CompanyNetwork:
                    company = company ?? item.Company;
                    if (item.TimelineItemScope == TimelineItemScope.Self)
                    {
                        this.PostedIntoVerb = "dans";
                        this.PostedIntoContainerName = "l'entreprise";
                        this.PostedIntoName = company.Name;
                        this.PostedIntoUrl = services.Company.GetProfileUrl(company, UriKind.Absolute);
                    }
                    else
                    {
                        this.PostedByName = company.Name;
                        this.PostedByUrl = services.Company.GetProfileUrl(company, UriKind.Absolute);
                        this.PostedByPictureUrl = services.Company.GetProfilePictureUrl(company, CompanyPictureSize.Small, UriKind.Absolute);
                    }
                    break;

                case TimelineType.Event:
                    evt = evt ?? item.Event;
                    this.PostedIntoVerb = "dans";
                    this.PostedIntoContainerName = "l'évènement";
                    this.PostedIntoName = evt.Name;
                    this.PostedIntoUrl = services.Events.GetProfileUrl(evt, UriKind.Absolute);
                    break;
                case TimelineType.Group:
                    group = group ?? item.Group;
                    this.PostedIntoVerb = "dans";
                    this.PostedIntoContainerName = "le groupe";
                    this.PostedIntoName = group.Name;
                    this.PostedIntoUrl = services.Groups.GetProfileUrl(group, UriKind.Absolute);
                    this.PostedIntoPictureUrl = services.Groups.GetProfilePictureUrl(group, CompanyPictureSize.Small, UriKind.Absolute);
                    break;
                case TimelineType.Place:
                    place = place ?? services.Places.SelectPlaceById(item.PlaceId.Value);
                    this.PostedIntoVerb = "dans";
                    this.PostedIntoContainerName = "le lieu";
                    this.PostedIntoName = place.Name;
                    this.PostedIntoUrl = services.Places.GetProfileUrl(place, UriKind.Absolute);
                    break;
                case TimelineType.Ad:
                    ad = ad ?? item.Ad;
                    this.PostedIntoVerb = "dans";
                    this.PostedIntoContainerName = "l'annonce";
                    this.PostedIntoName = ad.Title;
                    this.PostedIntoUrl = services.Ads.GetProfileUrl(ad, UriKind.Absolute);
                    break;
                case TimelineType.Project:
                    project = project ?? item.Project;
                    this.PostedIntoVerb = "dans";
                    this.PostedIntoContainerName = "le projet";
                    this.PostedIntoName = project.Name;
                    this.PostedIntoUrl = services.Projects.GetProfileUrl(project, UriKind.Absolute);
                    break;
                case TimelineType.Team:
                    team = team ?? item.Team;
                    this.PostedIntoVerb = "dans";
                    this.PostedIntoContainerName = "l'équipe";
                    this.PostedIntoName = team.Name;
                    this.PostedIntoUrl = services.Teams.GetProfileUrl(team, UriKind.Absolute);
                    break;
                default:
                    break;
            }

            switch (item.TimelineItemType)
            {
                case TimelineItemType.UserJoined:
                case TimelineItemType.UserInContact:
                case TimelineItemType.Poll:
                case TimelineItemType.Event:
                case TimelineItemType.Obsolete5:
                case TimelineItemType.Introducing:
                case TimelineItemType.CompanyProfileUpdated:
                case TimelineItemType.PeopleProfileUpdated:
                case TimelineItemType.Obsolete14:
                case TimelineItemType.Ad:
                case TimelineItemType.Deal:
                case TimelineItemType.CompanyJoined:
                case TimelineItemType.Twitter:
                    this.IsHidden = true;
                    break;

                case TimelineItemType.JoinedGroup:
                    this.Text = services.Lang.T("vient de rejoindre le groupe");
                    break;

                case TimelineItemType.TextPublication:
                    this.Text = item.Text;
                    break;

                case TimelineItemType.NewPartnerResource:
                case TimelineItemType.PartnerResourceUpdate:
                    partnerResource = partnerResource ?? item.PartnerResource;
                    this.PostedByName = item.TimelineItemType == TimelineItemType.NewPartnerResource ? services.Lang.T("Un nouveau partenaire a été ajouté !") : services.Lang.T("Un partenaire a été mis à jour !");
                    this.PostedByPictureUrl = services.PartnerResources.GetPictureUrl(partnerResource, CompanyPictureSize.Medium, UriKind.Absolute);
                    this.PostedIntoName = partnerResource.Name;
                    this.PostedByUrl = this.PostedIntoUrl = services.PartnerResources.GetProfileUrl(partnerResource, UriKind.Absolute);
                    this.Text = item.TimelineItemType == TimelineItemType.NewPartnerResource ? services.Lang.T(" a été ajouté, regardez ce qu'il a à vous offrir.") : services.Lang.T(" a été mis à jour, vous devriez jeter un oeil!");
                    this.ForegroundColor = "#FF2A3B";
                    this.BackgroundColor = "#FEB0BD";
                    break;

                default:
                    break;
            }

            this.DateUtc = item.CreateDate.AsLocal().ToUniversalTime();
            this.Id = item.Id;
            this.Url = services.Wall.GetUrl(this.Id, UriKind.Absolute);

            if (item.ExtraType != null)
            {
                this.FillExtra(services, item.ExtraTypeValue, item.Extra);
            }
        }

        private void FillComment(IServiceFactory services, TimelineItemComment comment, TimelineItem item)
        {
            this.IsUserAuthorized = true;
            this.PostedByName = comment.PostedBy.FirstName + " " + comment.PostedBy.LastName;
            this.PostedByPictureUrl = services.People.GetProfilePictureUrl(comment.PostedBy, UserProfilePictureSize.Small, UriKind.Absolute);
            this.PostedByUrl = services.People.GetProfileUrl(comment.PostedBy, UriKind.Absolute);

            this.Text = comment.Text;
            this.DateUtc = comment.CreateDate.AsLocal().ToUniversalTime();
            this.Id = item.Id;
            this.Url = services.Wall.GetUrl(item.Id, this.Id, UriKind.Absolute);

            this.ApplyColors(TimelineType.Profile);
        }

        private void FillExtra(IServiceFactory services, TimelineItemExtraType type, string extra)
        {
            switch (type)
            {
                case TimelineItemExtraType.OpenGraphDescription:
                    try
                    {
                        this.Attachments = this.Attachments ?? new List<BasicTimelineItemAttachmentModel>();
                        var item = JsonConvert.DeserializeObject<BasicTimelineItemAttachmentModel>(extra);
                        item.ExtraType = type;
                        this.Attachments.Add(item);
                    }
                    catch (Exception ex)
                    {
                        if (ex.IsFatal())
                            throw;
                    }

                    break;
                case TimelineItemExtraType.DealOrPicture:
                    {
                        this.Attachments = this.Attachments ?? new List<BasicTimelineItemAttachmentModel>();
                        var item = new BasicTimelineItemAttachmentModel();
                        item.ExtraType = type;
                        item.Title = services.Lang.T("Voir l'image");
                        item.Url = this.Url;
                        this.Attachments.Add(item);
                    }
                    break;

                case TimelineItemExtraType.None:
                case TimelineItemExtraType.TimelineSocialEntry:
                case TimelineItemExtraType.GoogleGroupImportedMessage:
                default:
                    break;
            }
        }
        
        public void ApplyColors(TimelineType type)
        {
            switch (type)
            {
                case TimelineType.Company:
                case TimelineType.CompanyNetwork:
                    this.ForegroundColor = "#FF2A3B";
                    this.BackgroundColor = "#FEB0BD";
                    break;

                case TimelineType.Event:
                    this.ForegroundColor = "#FFB006";
                    this.BackgroundColor = "#FFED64";
                    break;

                case TimelineType.Group:
                    this.ForegroundColor = "#1B9AF7";
                    this.BackgroundColor = "#87DDFD";
                    break;

                case TimelineType.Place:
                    this.ForegroundColor = "#1A9CCC";
                    this.BackgroundColor = "#DFECF1";
                    break;

                default:
                    this.ForegroundColor = "#6CC30C";
                    this.BackgroundColor = "#85ED80";
                    break;
            }
        }
    }

    public class BasicTimelineItemAttachmentModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public TimelineItemExtraType ExtraType { get; set; }
    }
}
