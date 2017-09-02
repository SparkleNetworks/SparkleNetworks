
namespace Sparkle.Services.Networks.Team
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TeamPageModel
    {
        private bool isAdmin;

        public TeamPageModel(IDictionary<int, IList<UserTeamMemberModel>> usersWithNetworkRole, IList<UserModel> usersWithoutNetworkRole)
        {
            this.UsersWithNetworkRole = new UsersWithNetworkRoleList(usersWithNetworkRole);
            this.UsersWithoutNetworkRole = usersWithoutNetworkRole;
        }

        public UsersWithNetworkRoleList UsersWithNetworkRole { get; set; }

        public IList<UserModel> UsersWithoutNetworkRole { get; set; }

        public bool IsAdmin
        {
            get { return this.isAdmin; }
            set
            {
                this.UsersWithNetworkRole.IsAdmin = value;
                this.isAdmin = value;
            }
        }
    }

    public class UsersWithNetworkRoleList
    {
        public bool IsAdmin { get; set; }

        public bool IsLightMode { get; set; }

        public IDictionary<int, IList<UserTeamMemberModel>> Users { get; set; }

        public UsersWithNetworkRoleList(IDictionary<int, IList<UserTeamMemberModel>> usersWithNetworkRole)
        {
            this.Users = usersWithNetworkRole;
        }
    }
}
