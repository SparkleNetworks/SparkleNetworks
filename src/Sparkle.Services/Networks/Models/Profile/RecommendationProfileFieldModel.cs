
namespace Sparkle.Services.Networks.Models.Profile
{
    using Sparkle.LinkedInNET.Profiles;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RecommendationProfileFieldModel
    {
        public string LinkedInId { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public string RecommenderId { get; set; }

        public string RecommenderFirstname { get; set; }

        public string RecommenderLastname { get; set; }

        public string RecommenderHeadline { get; set; }

        public void UpdateFrom(Recommendation item)
        {
            this.LinkedInId = item.Id;
            this.Text = item.RecommendationText;

            if (item.RecommendationType != null)
            {
                this.Type = item.RecommendationType.Code;
            }

            if (item.Recommender != null)
            {
                this.RecommenderId = item.Recommender.Id;
                this.RecommenderFirstname = item.Recommender.Firstname;
                this.RecommenderLastname = item.Recommender.Lastname;
                this.RecommenderHeadline = item.Recommender.Headline;
            }
        }

        public string RecommenderPicture { get; set; }
    }
}
