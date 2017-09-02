
namespace Sparkle.WebBase
{
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ModelExtensions
    {
        public static string GetPictureUrl(this UserModel user, PersonPictureSize size)
        {
            return string.Format("/Data/PersonPicture/{0}/{1}", user.Username, size);
        }
    }
}
