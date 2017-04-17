
namespace Sparkle.Services.Networks.Users
{
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class ChangeUserCultureRequest : BaseRequest
    {
        public ChangeUserCultureRequest()
            : base()
        {
        }

        public ChangeUserCultureRequest(int? userId, CultureInfo newCulture)
            : base()
        {
            this.UserId = userId;
            this.NewCulture = newCulture;
        }

        public int? UserId { get; set; }

        public CultureInfo NewCulture { get; set; }
    }
}
