
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Companies;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class CompanyModel
    {
        public CompanyModel()
        {
        }

        public CompanyModel(Entities.Networks.Company company)
        {
            this.Set(company);
        }

        public CompanyModel(int id, string name, string alias)
        {
            this.Id = id;
            this.Name = name;
            this.Alias = alias;
        }

        public CompanyModel(Entities.Networks.Neutral.CompanyPoco item)
        {
            this.Set(item);
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int NetworkId { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsApproved { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        public List<Tags.TagModel> Skills { get; set; }

        [DataMember]
        public short CategoryId { get; set; }

        [DataMember]
        public string PictureUrl { get; set; }

        [DataMember]
        public string SmallProfilePictureUrl { get; set; }

        [DataMember]
        public string MediumProfilePictureUrl { get; set; }

        [DataMember]
        public string LargeProfilePictureUrl { get; set; }

        [DataMember]
        public string ProfileUrl { get; set; }

        [DataMember]
        public string Baseline { get; set; }

        [DataMember]
        public string About { get; set; }

        [DataMember]
        public IList<CompanyProfileFieldModel> Fields { get; set; }

        [DataMember]
        public IDictionary<string, object> Data { get; set; }

        [DataMember]
        public IList<PlaceModel> Places { get; set; }

        [DataMember]
        public IList<Networks.Tags.Tag2Model> Tags { get; set; }

        public void SetFields(IList<CompanyProfileFieldModel> fields)
        {
            if (this.Fields == null)
            {
                this.Fields = new List<CompanyProfileFieldModel>(fields.Count);
            }
            else
            {
                var remove = this.Fields.Where(f => fields.Any(f1 => f1.Type == f.Type)).ToArray();
                for (int i = 0; i < remove.Length; i++)
                {
                    this.Fields.Remove(remove[i]);
                }
            }

            foreach (var field in fields)
            {
                this.Fields.Add(field);

                switch (field.Type)
                {
                    case ProfileFieldType.Site:
                        break;
                    case ProfileFieldType.Phone:
                        break;
                    case ProfileFieldType.About:
                        this.About = field.Value;
                        break;
                    case ProfileFieldType.City:
                        break;
                    case ProfileFieldType.ZipCode:
                        break;
                    case ProfileFieldType.FavoriteQuotes:
                        break;
                    case ProfileFieldType.CurrentTarget:
                        break;
                    case ProfileFieldType.Contribution:
                        break;
                    case ProfileFieldType.Country:
                        break;
                    case ProfileFieldType.Headline:
                        break;
                    case ProfileFieldType.ContactGuideline:
                        break;
                    case ProfileFieldType.Industry:
                        break;
                    case ProfileFieldType.LinkedInPublicUrl:
                        break;
                    case ProfileFieldType.Language:
                        break;
                    case ProfileFieldType.Education:
                        break;
                    case ProfileFieldType.Twitter:
                        break;
                    case ProfileFieldType.GTalk:
                        break;
                    case ProfileFieldType.Msn:
                        break;
                    case ProfileFieldType.Skype:
                        break;
                    case ProfileFieldType.Yahoo:
                        break;
                    case ProfileFieldType.Volunteer:
                        break;
                    case ProfileFieldType.Certification:
                        break;
                    case ProfileFieldType.Patent:
                        break;
                    case ProfileFieldType.Location:
                        break;
                    case ProfileFieldType.Contact:
                        break;
                    case ProfileFieldType.Recommendation:
                        break;
                    case ProfileFieldType.Email:
                        break;
                    case ProfileFieldType.Facebook:
                        break;
                    case ProfileFieldType.AngelList:
                        break;
                    case ProfileFieldType.Position:
                        break;
                    default:
                        break;
                }
            }
        }

        private void Set(Entities.Networks.Company item)
        {
            if (item != null)
            {
                this.Id = item.ID;
                this.Name = item.Name;
                this.Alias = item.Alias;
                this.IsEnabled = item.IsEnabled;
                this.IsActive = item.IsApproved && item.IsEnabled;
                this.NetworkId = item.NetworkId;
            }
        }

        private void Set(Entities.Networks.Neutral.CompanyPoco item)
        {
            if (item != null)
            {
                this.Id = item.ID;
                this.Name = item.Name;
                this.Alias = item.Alias;
                this.IsEnabled = item.IsEnabled;
                this.IsActive = item.IsApproved && item.IsEnabled;
                this.NetworkId = item.NetworkId;
            }
        }
    }
}
