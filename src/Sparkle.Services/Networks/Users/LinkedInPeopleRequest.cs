
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.Models.Tags;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LinkedInPeopleRequest : BaseRequest
    {
        public LinkedInPeopleRequest(string token)
        {
            this.AccessToken = token;
        }

        public LinkedInPeopleRequest(int userId)
        {
            this.UserId = userId;
        }

        public int UserId { get; set; }

        public string AccessToken { get; set; }

    }

    public class LinkedInPeopleResult : BaseResult<LinkedInPeopleRequest, LinkedInPeopleError>
    {
        public LinkedInPeopleResult(LinkedInPeopleRequest request)
            : base(request)
        {
            this.Changes = new List<ProfileFieldUpdate>();
            this.TagChanges = new List<TagModel>();
            this.Companies = new List<LinkedInCompanyResult>();
        }

        public User UserEntity { get; set; }

        public string JobTitleToCreate { get; set; }

        public IList<ProfileFieldUpdate> Changes { get; set; }

        public IList<TagModel> TagChanges { get; set; }

        public int EntityChanges { get; set; }

        public IList<LinkedInCompanyResult> Companies { get; set; }     // Used for Apply proc


        public GetPictureFromUrlResult PicturesStream { get; set; }

        public bool UserLinkedInId { get; set; }
    }

    public class ProfileFieldUpdate
    {
        private readonly UserProfileFieldModel item;
        private readonly ProfileFieldChange change;

        public ProfileFieldUpdate(UserProfileFieldModel model, ProfileFieldChange change)
        {
            this.item = model;
            this.change = change;
        }

        public ProfileFieldChange Change
        {
            get { return this.change; }
        }

        public UserProfileFieldModel Item
        {
            get { return this.item; }
        }
    }

    public enum ProfileFieldChange
    {
        Create,
        Update,
        Delete,
        CreateOrUpdate,
    }

    public enum LinkedInPeopleError
    {
        LinkedInNotConfigured,
        LinkedInAccountNotLinked,
        ApiCallFailed,
        InvalidApiToken,
        UnauthorizedUser,
        NoSuchUser,
        RequestIsNotNew,
        CannotRetrieveProfilePicture,
        LinkedInImportAlreadyDone,
        LinkedInUpdateIsDisabled,
    }
}
