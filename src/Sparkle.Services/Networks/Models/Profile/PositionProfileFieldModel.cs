
namespace Sparkle.Services.Networks.Models.Profile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PositionProfileFieldModel
    {
        /// <summary>
        /// Gets or sets the company position/school diploma/volunteer role/certification name.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the name of the company/school/volunteer organization/certification authority.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the summary (big text)/volunteer cause/certification license number.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the FieldOfStudy of an education.
        /// </summary>
        public string FieldOfStudy { get; set; }

        public short? BeginYear { get; set; }
        public short? BeginMonth { get; set; }
        public short? BeginDay { get; set; }
        public short? EndYear { get; set; }
        public short? EndMonth { get; set; }
        public short? EndDay { get; set; }

        public bool IsCurrent { get; set; }

        public long? LinkedInId { get; set; }
        public int? CompanyId { get; set; }

        public void UpdateFrom(LinkedInNET.Profiles.PersonPosition item)
        {
            this.LinkedInId =   item.Id;
            this.CompanyId =    item.Company != null ? item.Company.Id : default(int?);
            this.Title =        item.Title;
            this.Summary =      item.Summary;
            this.CompanyName =  item.Company != null ? item.Company.Name : null;
            this.BeginYear =    item.StartDate != null ? item.StartDate.Year : null;
            this.BeginMonth =   item.StartDate != null ? item.StartDate.Month : null;
            this.BeginDay =     item.StartDate != null ? item.StartDate.Day : null;
            this.EndYear =      item.EndDate != null ? item.EndDate.Year : null;
            this.EndMonth =     item.EndDate != null ? item.EndDate.Month : null;
            this.EndDay =       item.EndDate != null ? item.EndDate.Day : null;
            this.IsCurrent =    item.IsCurrent;
        }

        public void UpdateFrom(LinkedInNET.Profiles.Education item)
        {
            this.LinkedInId =   item.Id;
            this.CompanyName =  item.SchoolName;
            this.Title =        item.Degree;
            this.Summary =      item.Notes;
            this.FieldOfStudy = item.FieldOfStudy;
            this.BeginYear =    item.StartDate != null ? item.StartDate.Year : null;
            this.BeginMonth =   item.StartDate != null ? item.StartDate.Month : null;
            this.BeginDay =     item.StartDate != null ? item.StartDate.Day : null;
            this.EndYear =      item.EndDate != null ? item.EndDate.Year : null;
            this.EndMonth =     item.EndDate != null ? item.EndDate.Month : null;
            this.EndDay =       item.EndDate != null ? item.EndDate.Day : null;
        }

        public void UpdateFrom(LinkedInNET.Profiles.VolunteerExperience item)
        {
            this.LinkedInId =   item.Id;
            this.Title =        item.Role;
            this.CompanyName =  item.Organization != null ? item.Organization.Name : null;
            this.Summary =      item.Cause != null ? item.Cause.Name : null;
        }

        public void UpdateFrom(LinkedInNET.Profiles.Certification item)
        {
            this.LinkedInId =   item.Id;
            this.Title =        item.Name;
            this.CompanyName =  item.Authority != null ?item.Authority.Name : null;
            this.Summary =      item.Number;
            this.BeginYear =    item.StartDate != null ? item.StartDate.Year : null;
            this.BeginMonth =   item.StartDate != null ? item.StartDate.Month : null;
            this.BeginDay =     item.StartDate != null ? item.StartDate.Day : null;
            this.EndYear =      item.EndDate != null ? item.EndDate.Year : null;
            this.EndMonth =     item.EndDate != null ? item.EndDate.Month : null;
            this.EndDay =       item.EndDate != null ? item.EndDate.Day : null;
        }
    }
}
