
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CompanyPlaceModel
    {
        public CompanyPlaceModel(CompanyPlace item)
        {
            this.Set(item);
        }

        public CompanyPlaceModel(CompanyPlace item, PlaceModel place, CompanyModel company)
            : this(item)
        {
            this.Place = place;
            this.Company = company;
        }

        private void Set(CompanyPlace item)
        {
            if (item != null)
            {
                this.Id = item.Id;
                this.CompanyId = item.CompanyId;
                this.PlaceId = item.PlaceId;
                this.DateCreatedUtc = item.DateCreatedUtc;
                this.CreatedByUserId = item.CreatedByUserId;
                this.DateDeletedUtc = item.DateDeletedUtc;
                this.DeletedByUserId = item.DeletedByUserId;
            }
        }

        public int Id { get; set; }

        public int CompanyId { get; set; }

        public int PlaceId { get; set; }

        public PlaceModel Place { get; set; }

        public CompanyModel Company { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public int CreatedByUserId { get; set; }

        public DateTime? DateDeletedUtc { get; set; }

        public int? DeletedByUserId { get; set; }

        public bool IsDeleted
        {
            get { return this.DateDeletedUtc.HasValue && this.DeletedByUserId.HasValue; }
        }
    }
}
