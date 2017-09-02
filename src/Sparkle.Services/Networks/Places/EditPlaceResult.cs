
namespace Sparkle.Services.Networks.Places
{
    using Sparkle.Services.Networks.Models;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class EditPlaceResult : BaseResult<EditPlaceRequest, AddPlaceError>
    {
        public EditPlaceResult(EditPlaceRequest request)
            : base(request)
        {
        }

        public PlaceModel Place { get; set; }

        public bool? GeocodeSucceed { get; set; }
    }

    public enum AddPlaceError
    {
        MissingCategory,
        NoSuchPlace,
        NoSuchCompany,
        CannotAddPlaceForCompany,
        NoSuchActingUser,
        InternalError,
    }
}
