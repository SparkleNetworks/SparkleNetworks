
namespace Sparkle.Services.Objects
{
    using System;

    public class WeeklyMailSubscriber
    {
        public WeeklyMailSubscriber()
        {
        }

        public WeeklyMailSubscriber(Entities.Networks.User person)
        {
            this.Email = person.Email;
            this.UserId = person.Id;
            this.FirstName = person.FirstName;
            this.LastName = person.LastName;
            this.Registered = true;
        }

        public string Email { get; set; }

        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool Registered { get; set; }

        public bool OptedIn { get; set; }

        public int InvitedBy { get; set; }

        public string InvitedByName { get; set; }

        public string InvitedCode { get { return InvitedCodeGuid.ToString(); } }

        public Guid InvitedCodeGuid { get; set; }

        public override string ToString()
        {
            return this.Email + " " + (this.Registered ? ("registered as " + this.FirstName + " " + this.LastName) : ("invited as " + this.InvitedCode));
        }
    }
}
