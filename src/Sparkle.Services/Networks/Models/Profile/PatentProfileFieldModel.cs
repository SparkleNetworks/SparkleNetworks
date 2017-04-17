
namespace Sparkle.Services.Networks.Models.Profile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PatentProfileFieldModel
    {
        public long? LinkedInId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Summary { get; set; }
        public string Number { get; set; }
        public bool Granted { get; set; }
        public string Office { get; set; }
        public short? Year { get; set; }
        public short? Month { get; set; }
        public short? Day { get; set; }
        public string[] Inventors { get; set; }

        public void UpdateFrom(LinkedInNET.Profiles.Patent item)
        {
            this.LinkedInId = item.Id;
            this.Title = item.Title;
            this.Url = item.Url;
            this.Summary = item.Summary;
            this.Number = item.Number;
            this.Granted = item.Status != null ? (item.Status.Id == 0 ? false : true) : false;
            this.Office = item.Office != null ? item.Office.Name : null;
            this.Year = item.Date != null ? item.Date.Year : null;
            this.Month = item.Date != null ? item.Date.Month : null;
            this.Day = item.Date != null ? item.Date.Day : null;
            if (item.Inventors != null)
            {
                var inventors = new List<string>();
                foreach (var inventor in item.Inventors.Inventor)
                {
                    if (!string.IsNullOrEmpty(inventor.Name))
                    {
                        inventors.Add(inventor.Name);
                    }
                    else if (inventor.Person != null)
                    {
                        inventors.Add(string.Format("{0} {1}", inventor.Person.FirstName, inventor.Person.LastName));
                    }
                    else
                    {
                        // do nothing
                    }
                }
                this.Inventors = inventors.ToArray();
            }
            else
            {
                this.Inventors = new string[0];
            }
        }

    }
}
