
namespace Sparkle.Services.Networks.Places
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Resources;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class EditPlaceRequest : BaseRequest
    {
        private string importedId;
        public EditPlaceRequest()
        {
        }

        public EditPlaceRequest(Place item)
            : this()
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.Alias = item.Alias;
            this.Description = item.Description;
            this.ZipCode = item.ZipCode;
            this.Floor = item.Floor;
            this.Door = item.Door;
            this.City = item.City;
            this.Building = item.Building;
            this.Address = item.Address;
            this.Access = item.Access;
            this.FoursquareId = item.FoursquareId;
            this.CategoryId = item.CategoryId;
            this.ParentId = item.ParentId;
            this.Country = item.Country;

            this.Place = new PlaceModel(item);
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Name", ResourceType = typeof(NetworksLabels))]
        [StringLength(80, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [StringLength(80, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Alias { get; set; }

        [Required]
        [Display(Name = "Description", ResourceType = typeof(NetworksLabels))]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Description { get; set; }

        [Display(Name = "Address", ResourceType = typeof(NetworksLabels))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Address { get; set; }

        [Display(Name = "Zip", ResourceType = typeof(NetworksLabels))]
        [StringLength(10, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string ZipCode { get; set; }

        [Display(Name = "City", ResourceType = typeof(NetworksLabels))]
        [StringLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string City { get; set; }

        public int? ParentId { get; set; }

        [Display(Name = "Country", ResourceType = typeof(NetworksLabels))]
        [StringLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Country { get; set; }

        public string Building { get; set; }

        public string Access { get; set; }

        public int? Floor { get; set; }

        public string Door { get; set; }

        [RegularExpression("^[a-fA-F0-9]+$", ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "InvalidFoursquarePlaceId")]
        public string FoursquareId { get; set; } // 4dc3ce542271f27051027de1

        public IList<PlaceModel> Children { get; set; }
        
        public int CategoryId { get; set; }

        public IList<PlaceCategoryModel> Categories { get; set; }

        public IList<PlaceModel> Places { get; set; }

        public PlaceModel Place { get; set; }

        public string CompanyAlias { get; set; }

        public string CompanyName { get; set; }

        public int ActingUserId { get; set; }

        public bool IsAuthorized { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? Latitude { get; set; }

        public string GetImportedId()
        {
            return this.importedId;
        }

        public void SetImportedId(string importedId)
        {
            this.importedId = importedId;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();

            if (this.CategoryId == 0)
            {
                this.AddValidationError("CategoryId", NetworksEnumMessages.AddPlaceError_MissingCategory);
            }
        }
    }
}
