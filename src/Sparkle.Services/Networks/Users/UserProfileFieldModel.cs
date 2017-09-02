
namespace Sparkle.Services.Networks.Users
{
    using Newtonsoft.Json;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Profile;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserProfileFieldModel : IProfileFieldValueModel
    {
        // TODO: MERGE WITH CompanyProfileFieldModel
        // TODO: MERGE WITH PartnerResourceProfileFieldModel

        private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
        };

        private PositionProfileFieldModel positionModel;
        private PositionProfileFieldModel educationModel;
        private PositionProfileFieldModel volunteerModel;
        private PositionProfileFieldModel certificationModel;

        private LanguageProfileFieldModel languageModel;

        private PatentProfileFieldModel patentModel;

        private RecommendationProfileFieldModel recommedationModel;

        public UserProfileFieldModel()
        {
        }

        public UserProfileFieldModel(ProfileFieldType type)
        {
            this.Type = type;
        }

        public UserProfileFieldModel(UserProfileField item)
        {
            this.Id = item.Id;
            this.UserId = item.UserId;
            this.TypeId = item.ProfileFieldId;
            this.Type = item.ProfileFieldType;
            this.Source = item.SourceType;
            this.Data = item.Data;
            this.DateCreatedUtc = item.DateCreatedUtc;
            this.DateUpdatedUtc = item.DateUpdatedUtc;

            this.SetValue(item.Value);
        }

        public UserProfileFieldModel(int userId, ProfileFieldType fieldType, string distantValue, ProfileFieldSource source)
        {
            this.UserId = userId;
            this.TypeId = (int)fieldType;
            this.Type = fieldType;
            this.Source = source;

            this.SetValue(distantValue);
        }

        private void SetValue(string value)
        {
            this.Value = value;

            if (this.Type == ProfileFieldType.Twitter)
            {
                string username = null;
                if (SrkToolkit.DataAnnotations.TwitterUsernameAttribute.GetUsername(this.Value, out username))
                    this.Value = username;
            }
        }

        public int Id { get; set; }

        public int UserId { get; set; }

        public int TypeId { get; set; }

        int IProfileFieldValueModel.EntityId
        {
            get { return this.UserId; }
            set { this.UserId = value; }
        }

        public ProfileFieldType Type { get; set; }

        public string Value { get; set; }

        public ProfileFieldSource Source { get; set; }

        public string Data { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime? DateUpdatedUtc { get; set; }

        public PositionProfileFieldModel PositionModel
        {
            get { return this.GetModel(ref this.positionModel, ProfileFieldType.Position); }
            set
            {
                this.SetModel(ref this.positionModel, ProfileFieldType.Position, value);
                if (this.PositionModel != null)
                {
                    if (!string.IsNullOrEmpty(this.PositionModel.Title))
                    {
                        if (!string.IsNullOrEmpty(this.PositionModel.CompanyName))
                            this.Value = this.PositionModel.Title + " @ " + this.PositionModel.CompanyName;
                        else
                            this.Value = this.PositionModel.Title;
                    }
                }
            }
        }

        public PositionProfileFieldModel EducationModel
        {
            get { return this.GetModel(ref this.educationModel, ProfileFieldType.Education); }
            set
            {
                this.SetModel(ref this.educationModel, ProfileFieldType.Education, value);
                if (this.EducationModel != null)
                {
                    if (!string.IsNullOrEmpty(this.EducationModel.CompanyName))
                    {
                        if (!string.IsNullOrEmpty(this.EducationModel.Title))
                            this.Value = this.EducationModel.Title + " @ " + this.EducationModel.CompanyName;
                        else
                            this.Value = this.EducationModel.CompanyName;
                    }
                }
            }
        }

        public PositionProfileFieldModel VolunteerModel
        {
            get { return this.GetModel(ref this.volunteerModel, ProfileFieldType.Volunteer); }
            set
            {
                this.SetModel(ref this.volunteerModel, ProfileFieldType.Volunteer, value);
                if (this.VolunteerModel != null)
                {
                    if (!string.IsNullOrEmpty(this.VolunteerModel.Title))
                    {
                        this.Value = this.VolunteerModel.Title;
                        if (!string.IsNullOrEmpty(this.VolunteerModel.CompanyName))
                            this.Value = string.Format("{0} @ {1}", this.Value, this.VolunteerModel.CompanyName);
                    }
                }
            }
        }

        public PositionProfileFieldModel CertificationModel
        {
            get { return this.GetModel(ref this.certificationModel, ProfileFieldType.Certification); }
            set
            {
                this.SetModel(ref this.certificationModel, ProfileFieldType.Certification, value);
                if (this.CertificationModel != null)
                {
                    if (!string.IsNullOrEmpty(this.CertificationModel.Title))
                    {
                        this.Value = this.CertificationModel.Title;
                        if (!string.IsNullOrEmpty(this.CertificationModel.CompanyName))
                        {
                            if (!string.IsNullOrEmpty(this.CertificationModel.Summary))
                                this.Value = string.Format("{0} ({1}, License {2})", this.Value, this.CertificationModel.CompanyName, this.CertificationModel.Summary);
                            else
                                this.Value = string.Format("{0} ({1})", this.Value, this.CertificationModel.CompanyName);
                        }
                    }
                }
            }
        }

        public LanguageProfileFieldModel LanguageModel
        {
            get { return this.GetModel(ref this.languageModel, ProfileFieldType.Language); }
            set 
            {
                this.SetModel(ref this.languageModel, ProfileFieldType.Language, value);
                if (this.LanguageModel != null)
                {
                    if (!string.IsNullOrEmpty(this.LanguageModel.Name))
                    {
                        this.Value = this.LanguageModel.Name;
                    }
                }
            }
        }

        public PatentProfileFieldModel PatentModel
        {
            get { return this.GetModel(ref this.patentModel, ProfileFieldType.Patent); }
            set
            {
                this.SetModel(ref this.patentModel, ProfileFieldType.Patent, value);
                if (this.PatentModel != null)
                {
                    if (!string.IsNullOrEmpty(this.PatentModel.Title))
                    {
                        this.Value = this.PatentModel.Title;
                        if (!string.IsNullOrEmpty(this.PatentModel.Number))
                            this.Value = string.Format("{0}, No. {1}", this.Value, this.PatentModel.Number);
                        if (!string.IsNullOrEmpty(this.PatentModel.Office))
                            this.Value = string.Format("{0} ({1})", this.Value, this.PatentModel.Office);
                    }
                }
            }
        }

        public RecommendationProfileFieldModel RecommendationModel
        {
            get { return this.GetModel(ref this.recommedationModel, ProfileFieldType.Recommendation); }
            set
            {
                this.SetModel(ref this.recommedationModel, ProfileFieldType.Recommendation, value);
                if (this.RecommendationModel != null)
                {
                    if (!string.IsNullOrEmpty(this.RecommendationModel.Text))
                    {
                        this.Value = this.RecommendationModel.Text;
                    }
                }
            }
        }

        private T GetModel<T>(ref T field, ProfileFieldType type)
        {
            if (field == null)
            {
                if (this.Data != null && this.Type == type)
                {
                    field = JsonConvert.DeserializeObject<T>(this.Data);
                }
            }

            return field;
        }

        private void SetModel<T>(ref T field, ProfileFieldType type, T value)
        {
            if (this.Type != type)
                throw new InvalidOperationException("Cannot set a " + typeof(T) + " into a field of type " + type);

            field = value;

            if (value != null)
            {
                this.Data = JsonConvert.SerializeObject(value, Formatting.None, jsonSettings);
            }
            else
            {
                this.Data = null;
            }
        }
    }

    public static class UserProfileFieldModelExtensions
    {
        public static UserProfileFieldModel SingleByType(this IEnumerable<UserProfileFieldModel> query, ProfileFieldType type)
        {
            return query.Where(o => o.Type == type).SingleOrDefault();
        }
    }
}
