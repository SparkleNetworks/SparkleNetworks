
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract]
    public class InformationNoteModel
    {
        public InformationNoteModel()
        {
        }

        public InformationNoteModel(Entities.Networks.InformationNote item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            this.Id = item.Id;
            this.Name = item.Name;
            this.StartDateUtc = item.StartDateUtc.AsUtc();
            this.EndDateUtc = item.EndDateUtc.AsUtc();
            this.Description = item.Description;
            this.UserId = item.UserId;
            this.NetworkId = item.NetworkId;
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int NetworkId { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public DateTime StartDateUtc { get; set; }

        [DataMember]
        public DateTime EndDateUtc { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [IgnoreDataMember]
        public bool IsFuture
        {
            get { return DateTime.UtcNow < this.StartDateUtc; }
        }

        [IgnoreDataMember]
        public bool CanEdit
        {
            get { return !this.IsDeleted; }
        }

        [IgnoreDataMember]
        public bool IsPast
        {
            get { return this.EndDateUtc < DateTime.UtcNow; }
        }

        [IgnoreDataMember]
        public bool IsCurrent
        {
            get { return this.StartDateUtc <= DateTime.UtcNow && DateTime.UtcNow <= this.EndDateUtc; }
        }

        [IgnoreDataMember]
        public string CssClass
        {
            get
            {
                return
                    (this.IsCurrent ? "IsCurrent " : "IsNotCurrent ") +
                    (this.IsFuture ? "IsFuture " : "") +
                    (this.IsPast ? "IsPast " : "");
            }
        }

        public override string ToString()
        {
            return "InformationNote " + this.Id + " " + this.Name
                + (this.IsCurrent ? " IsCurrent" : "")
                + (this.IsFuture ? " IsFuture" : "")
                + (this.IsPast ? " IsPast" : "")
                ;
        }
    }
}
