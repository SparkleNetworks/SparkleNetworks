
namespace Sparkle.Services.Networks.Clubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SrkToolkit.Domain;

    public class EditClubResult : BaseResult<EditClubRequest, EditClubError>
    {
        public EditClubResult(EditClubRequest request)
            : base(request)
        {
        }

        public Entities.Networks.Club Item { get; set; }
    }

    public enum EditClubError
    {
    }
}
