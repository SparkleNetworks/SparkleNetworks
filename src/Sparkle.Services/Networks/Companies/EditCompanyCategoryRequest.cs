
namespace Sparkle.Services.Networks.Companies
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Lang;
    using SrkToolkit;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    public class EditCompanyCategoryRequest : BaseRequest
    {
        public EditCompanyCategoryRequest()
        {
            this.InitLists();
        }

        public EditCompanyCategoryRequest(CompanyCategory item)
        {
            if (item != null)
            {
                this.Id = item.Id;
                this.Name = item.Name;
                this.KnownCategory = item.KnownCategoryValue;
            }

            this.InitLists();
        }

        private void InitLists()
        {
            var values = (KnownCompanyCategory[])Enum.GetValues(typeof(KnownCompanyCategory));
            this.AvailableKnownCategories = values.ToDictionary(o => o, o => EnumTools.GetDescription(o, NetworksEnumMessages.ResourceManager));
        }

        public short? Id { get; set; }

        [Required]
        [Display(Name = "Name", ResourceType = typeof(NetworksLabels))]
        public string Name { get; set; }

        [Required]
        [Display(Name = "KnownCategory", ResourceType = typeof(NetworksLabels))]
        public KnownCompanyCategory KnownCategory { get; set; }
        public IDictionary<KnownCompanyCategory, string> AvailableKnownCategories { get; set; }
    }

    public class EditCompanyCategoryResult : BaseResult<EditCompanyCategoryRequest, EditCompanyCategoryError>
    {
        public EditCompanyCategoryResult(EditCompanyCategoryRequest request)
            : base(request)
        {
        }
    }

    public enum EditCompanyCategoryError
    {
    }
}
