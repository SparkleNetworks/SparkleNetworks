
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Users;
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RegisterInvitationModel
    {
        public RegisterInvitationModel()
        {
        }

        public RegisterInvitationModel(Invited item)
        {
            this.Id = item.Id;
            this.UserId = item.UserId;
            this.Code = item.Code;
            this.Email = new EmailAddress(item.Email);

            if (item.UserId != null)
            {
                this.User = item.User != null ? new UserModel(item.User) : new UserModel(item.UserId.Value);
            }
        }

        public RegisterInvitationModel(int id)
        {
            this.Id = id;
        }

        public int Id { get; set; }

        public int? UserId { get; set; }

        public bool Accepted
        {
            get { return this.UserId != null; }
        }

        public UserModel User { get; set; }

        public Guid Code { get; set; }

        public EmailAddress Email { get; set; }
    }
}
