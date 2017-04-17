
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ProfilePictureModel
    {
        public Dictionary<string, PictureAccess> Pictures { get; set; }

        public string Username { get; set; }

        public int UserId { get; set; }
    }
}
