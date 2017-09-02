
namespace Sparkle.Services.Networks
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.PartnerResources;
    using Sparkle.Services.Networks.Users;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public interface IPartnerResourcesService
    {
        void FillPartnerResourceEditRequestLists(PartnerResourceEditRequest model);
        PartnerResourceEditRequest GetEditRequest(string alias, string city = null, bool isAdmin = false);
        PartnerResourceEditResult SavePartnerResource(PartnerResourceEditRequest request);

        IList<CityPartnershipsModel> GetCitiesPartnerships();
        IList<PartnerResourceEditRequest> GetPartnersPerCity(int cityId);

        bool ToggleAvailable(string alias, bool enable);
        bool Delete(string alias, int userId, bool sendEmail = false);

        CityPartnershipsModel GetCityPartnerShipsModel(int cityId, bool isAdmin);

        IList<PartnerResourceModel> GetAll(ProfileFieldType[] loadFields = null);

        string GetProfileUrl(PartnerResource partnerResource, UriKind uriKind);
        string GetProfileUrl(string alias, UriKind uriKind);

        SetProfilePictureResult SetPartnerResourcePicture(string alias, Stream stream, string mime);
        SetProfilePictureResult SetPartnerResourcePicture(SetProfilePictureRequest request);

        PartnerResource GetByAlias(string alias);

        ProfilePictureModel GetProfilePicture(PartnerResource item, PictureAccessMode mode);
        ProfilePictureModel GetProfilePictureForCreate(MemoryStream stream, PictureAccessMode mode);

        string GetPictureUrl(string alias, CompanyPictureSize size, UriKind kind);
        string GetPictureUrl(PartnerResource item, CompanyPictureSize size, UriKind kind);

        IList<PartnerResource> GetPendingRequests();

        void ApprovePending(string alias, int userId);
    }
}
