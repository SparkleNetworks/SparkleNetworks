
namespace Sparkle.Data.Networks.Objects
{
    using System;
    using Sparkle.Entities.Networks;

    public class PeopleCountByInviter
    {
        public int Inviter { get; set; }

        public string InviterFirstName { get; set; }

        public string InviterPictureUrl { get; set; }

        public int InvitedCount { get; set; }

        public int PeopleCount { get; set; }

        public int FullPeopleCount { get; set; }

        public int Score { get; set; }
    }
}
