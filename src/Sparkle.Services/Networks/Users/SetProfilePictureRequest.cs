
namespace Sparkle.Services.Networks.Users
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SrkToolkit.Domain;

    public class SetProfilePictureRequest : BaseRequest
    {
        public SetProfilePictureRequest()
            : base()
        {
        }

        public int UserId { get; set; }

        public Stream PictureStream { get; set; }

        public string PictureMime { get; set; }

        public string PictureName { get; set; }
    }
}
