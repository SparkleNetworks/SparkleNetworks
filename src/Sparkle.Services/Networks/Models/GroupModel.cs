
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GroupModel
    {
        public GroupModel()
        {
        }

        public GroupModel(Group item)
        {
            this.Set(item);
        }

        public GroupModel(Group group, GroupMember member)
        {
            this.Set(group);
            this.Set(member);
        }

        public GroupModel(int groupId)
        {
            this.Id = groupId;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public GroupCategory GroupCategory { get; set; }

        public bool IsPrivate { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public byte NotificationFrequency { get; set; }

        public int CreatedByUserId { get; set; }

        public int NetworkId { get; set; }

        public string ImportedId { get; set; }

        public bool IsDeleted { get; set; }

        public string PictureUrl { get; set; }

        public string Alias { get; set; }

        private void Set(Group item)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.Alias = item.Alias;
            this.GroupCategory = item.GroupCategory;
            this.IsPrivate = item.IsPrivate;
            this.Description = item.Description;
            this.Date = item.Date.AsUtc();
            this.NotificationFrequency = item.NotificationFrequency;
            this.CreatedByUserId = item.CreatedByUserId;
            this.NetworkId = item.NetworkId;
            this.ImportedId = item.ImportedId;
            this.IsDeleted = item.IsDeleted;
        }

        private void Set(GroupMember item)
        {
        }
    }
}
