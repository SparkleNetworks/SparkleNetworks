
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AchievementModel
    {
        public AchievementModel()
        {
        }

        public AchievementModel(AchievementsCompany item)
        {
            this.Id = item.AchievementId;
            this.DateEarned = item.DateEarned;
            this.Unlocked = true;

            if (item.Achievement != null)
            {
                this.Key = item.Achievement.Key;
                this.Title = item.Achievement.Title;
                this.Description = item.Achievement.Description;
                this.Level = item.Achievement.Level;

                this.FamilyId = item.Achievement.FamilyId;
            }
        }

        public AchievementModel(Achievement item)
        {
            this.Id = item.Id;
            this.Key = item.Key;
            this.Title = item.Title;
            this.Description = item.Description;
            this.Level = item.Level;

            this.FamilyId = item.FamilyId;
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Key { get; set; }

        public string Description { get; set; }

        public byte Level { get; set; }

        public DateTime DateEarned { get; set; }

        public int FamilyId { get; set; }

        public bool Unlocked { get; set; }
    }
}
