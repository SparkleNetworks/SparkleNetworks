
namespace Sparkle.Services.Networks.Models.Tags
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class TagModel
    {
        public TagModel(Sparkle.Entities.Networks.Skill item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            this.Id = item.Id;
            this.Name = item.TagName;
            this.Type = TagModelType.Skill;
        }

        public TagModel(Sparkle.Entities.Networks.Interest item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            this.Id = item.Id;
            this.Name = item.TagName;
            this.Type = TagModelType.Interest;
        }

        public TagModel(Sparkle.Entities.Networks.Recreation item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            this.Id = item.Id;
            this.Name = item.TagName;
            this.Type = TagModelType.Recreation;
        }

        public TagModel()
        {
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public TagModelType Type { get; set; }

        [DataMember]
        public int CreatedByUserId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Weight { get; set; }

        [DataMember]
        public bool? Activated { get; set; }
        ////public bool IsActivated
        ////{
        ////    get { return this.DeletedDateUtc == null && this.DeletedByUserId == null ? true : false }
        ////}

        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public Dictionary<string, int> Numbers { get; set; }
    }

    public static class TagModelExtensions
    {
        public static IEnumerable<TagModel> ToTagModel(this IEnumerable<TimelineItemSkill> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var item in collection)
            {
                yield return new TagModel
                {
                    Id = item.SkillId,
                    Name = item.Skill.TagName,
                    Type = TagModelType.Skill,
                    CreatedByUserId = item.CreatedByUserId,
                    Activated = item.DeletedDateUtc == null && item.DeletedByUserId == null ? true : false,
                };
            }
        }

        public static IEnumerable<TagModel> ToTagModel(this IEnumerable<GroupSkill> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var item in collection)
            {
                yield return new TagModel
                {
                    Id = item.SkillId,
                    Name = item.Skill.TagName,
                    Type = TagModelType.Skill,
                    CreatedByUserId = item.CreatedByUserId,
                    Activated = item.DeletedDateUtc == null && item.DeletedByUserId == null ? true : false,
                };
            }
        }

        public static IEnumerable<TagModel> ToTagModel(this IEnumerable<GroupInterest> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var item in collection)
            {
                yield return new TagModel
                {
                    Id = item.InterestId,
                    Name = item.Interest.TagName,
                    Type = TagModelType.Interest,
                    CreatedByUserId = item.CreatedByUserId,
                    Activated = item.DeletedDateUtc == null && item.DeletedByUserId == null ? true : false,
                };
            }
        }

        public static IEnumerable<TagModel> ToTagModel(this IEnumerable<GroupRecreation> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var item in collection)
            {
                yield return new TagModel
                {
                    Id = item.RecreationId,
                    Name = item.Recreation.TagName,
                    Type = TagModelType.Recreation,
                    CreatedByUserId = item.CreatedByUserId,
                    Activated = item.DeletedDateUtc == null && item.DeletedByUserId == null ? true : false,
                };
            }
        }
    }

    public enum TagModelType
    {
        Skill,
        Interest,
        Recreation,
    }
}
