
namespace Sparkle.Services.Networks.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AjaxTagPickerModel
    {
        public IDictionary<TagCategoryModel, IList<Tag2Model>> Tags { get; set; }

        public bool CanEdit { get; set; }

        public bool IsFilterBox { get; set; }

        public AjaxTagPickerModel()
        {
        }

        public AjaxTagPickerModel(IDictionary<TagCategoryModel, IList<Tag2Model>> tags, bool canEdit)
            : this()
        {
            this.Tags = tags;
            this.CanEdit = canEdit;
        }

        public AjaxTagPickerModel(IDictionary<TagCategoryModel, IList<Tag2Model>> tags, bool canEdit, bool isFilterBox)
            : this(tags, canEdit)
        {
            this.IsFilterBox = isFilterBox;
        }
    }
}
