
namespace Sparkle.Services.Networks.Team
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Users;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserTeamMemberModel
    {
        public UserTeamMemberModel(UserModel user, UserProfileFieldModel role, UserProfileFieldModel description, UserProfileFieldModel group, UserProfileFieldModel order)
        {
            this.User = user;
            this.SetNetworkTeamFields(role, description, group, order);
        }

        public string NetworkTeamRole { get; set; }

        public string NetworkTeamDescription { get; set; }

        public int NetworkTeamGroup { get; set; }

        public int NetworkTeamOrder { get; set; }

        public UserModel User { get; set; }

        private void SetNetworkTeamFields(UserProfileFieldModel role, UserProfileFieldModel description, UserProfileFieldModel group, UserProfileFieldModel order)
        {
            this.NetworkTeamGroup = int.Parse(group.Value);
            this.NetworkTeamOrder = int.Parse(order.Value);
            if (role != null)
            {
                this.NetworkTeamRole = role.Value;
            }
            if (description != null)
            {
                this.NetworkTeamDescription = description.Value;
            }
        }
    }
}
