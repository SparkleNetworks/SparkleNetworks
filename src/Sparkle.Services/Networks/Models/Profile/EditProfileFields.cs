﻿
namespace Sparkle.Services.Networks.Models.Profile
{
    using Sparkle.Entities.Networks;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EditProfileFieldsRequest : BaseRequest
    {
        public EditProfileFieldsRequest()
        {
        }

        public int ActingUserId { get; set; }

        public string EntityType { get; set; }

        public int EntityId { get; set; }

        public IList<ProfileFieldModel> Fields { get; set; }

        public IList<ProfileFieldValueModel> Values { get; set; }

        public ProfileFieldSource Source { get; set; }
    }

    public class EditProfileFieldsResult : BaseResult<EditProfileFieldsRequest, EditProfileFieldsError>
    {
        public EditProfileFieldsResult(EditProfileFieldsRequest request)
            : base(request)
        {
        }
    }

    public enum EditProfileFieldsError
    {
        NoSuchActingUser,
        NoSuchItem,
        NotAuthorized,
        ValueHasNoMatchingField,
    }
}
