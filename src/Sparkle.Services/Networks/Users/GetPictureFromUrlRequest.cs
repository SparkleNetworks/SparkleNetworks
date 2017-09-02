
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Services.Networks.Definitions;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class GetPictureFromUrlRequest : BaseRequest
    {
        private bool? makeResize;

        public string PictureUrl { get; set; }

        public bool MakeResize
        {
            get
            {
                if (!this.makeResize.HasValue)
                    this.makeResize = true;

                return this.makeResize.Value;
            }
            set { this.makeResize = value; }
        }
    }

    public class GetPictureFromUrlResult : BaseResult<GetPictureFromUrlRequest, GetPictureFromUrlError>
    {
        public GetPictureFromUrlResult(GetPictureFromUrlRequest request)
            : base(request)
        {
            this.ResizedPictures = new Dictionary<PictureFormat, MemoryStream>();
        }

        public MemoryStream OriginalPicture { get; set; }

        public IDictionary<PictureFormat, MemoryStream> ResizedPictures { get; set; }
    }

    public enum GetPictureFromUrlError
    {
        HttpRequestFailed,
        FileIsNotPicture
    }
}
