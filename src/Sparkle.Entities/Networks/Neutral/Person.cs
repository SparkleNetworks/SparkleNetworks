
namespace Sparkle.Entities.Networks.Neutral
{
    using System;

    public class Person : IPerson
    {
        public Person()
        {
        }

        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int ShortId { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public int? JobId { get; set; }

        public string JobName { get; set; }

        public string JobAlias { get; set; }

        public string CompanyAlias { get; set; }

        public string Culture { get; set; }

        public string Timezone { get; set; }

        public string PictureName { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public int NetworkAccessLevel { get; set; }

        public int CompanyAccessLevel { get; set; }

        public bool CompanyIsApproved { get; set; }

        public bool CompanyIsEnabled { get; set; }

        public override string ToString()
        {
            return this.Id + " " + this.Email + " " + this.Username;
        }
    }
}
