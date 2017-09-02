
namespace Sparkle.WebStatus.Domain
{
    using System;

    public class BasicUserModel
    {
        public Guid Guid { get; set; }

        public Guid Password { get; set; }

        public DateTime DateCreatedUtc { get; set; }
    }
}
