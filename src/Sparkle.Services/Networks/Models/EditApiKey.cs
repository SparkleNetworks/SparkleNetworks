
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SrkToolkit.Domain;

    public class EditApiKeyRequest : BaseRequest
    {
        public EditApiKeyRequest()
        {
        }

        public string Name { get; set; }

        public int? Id { get; set; }

        public bool IsEnabled { get; set; }

        public string Description { get; set; }

        public string Roles { get; set; }
    }

    public class EditApiKeyResult : BaseResult<EditApiKeyRequest, EditApiKeyError>
    {
        public EditApiKeyResult(EditApiKeyRequest request)
            : base(request)
        {
        }

        public ApiKeyModel Item { get; set; }
    }

    public enum EditApiKeyError
    {
        NoSuchItem,
        InvalidRequest
    }
}
