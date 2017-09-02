
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Lang;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class UserRolesModel
    {
        public IDictionary<NetworkAccessLevel, UserRoleModel> Roles { get; set; }

        public int UsersTotal { get; set; }
    }

    public class UserRoleModel
    {
        public string RoleName { get; set; }

        public string RoleDescription { get; set; }

        public IList<UserModel> Users { get; set; }

        public UserRoleModel(string name, string description, IList<UserModel> users)
        {
            this.RoleName = name;
            this.RoleDescription = description;
            this.Users = users;
        }
    }

    public class UserRoleFormModel
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public bool IsCurrentUserSparkleStaff { get; set; }

        [Display(Name = "NetworkAccessLevel_AddCompany", Description = "NetworkAccessLevel_AddCompany_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool AddCompany { get; set; }

        [Display(Name = "NetworkAccessLevel_ReadNetworkStatistics", Description = "NetworkAccessLevel_ReadNetworkStatistics_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool ReadNetworkStatistics { get; set; }

        [Display(Name = "NetworkAccessLevel_ReadDevices", Description = "NetworkAccessLevel_ReadDevices_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool ReadDevices { get; set; }

        [Display(Name = "NetworkAccessLevel_ManageDevices", Description = "NetworkAccessLevel_ManageDevices_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool ManageDevices { get; set; }

        [Display(Name = "NetworkAccessLevel_ManageInformationNotes", Description = "NetworkAccessLevel_ManageInformationNotes_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool ManageInformationNotes { get; set; }

        [Display(Name = "NetworkAccessLevel_ManageRegisterRequests", Description = "NetworkAccessLevel_ManageRegisterRequests_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool ManageRegisterRequests { get; set; }

        [Display(Name = "NetworkAccessLevel_ValidatePublications", Description = "NetworkAccessLevel_ValidatePublications_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool ValidatePublications { get; set; }

        [Display(Name = "NetworkAccessLevel_ManageCompany", Description = "NetworkAccessLevel_ManageCompany_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool ManageCompany { get; set; }

        [Display(Name = "NetworkAccessLevel_ValidatePendingUsers", Description = "NetworkAccessLevel_ValidatePendingUsers_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool ValidatePendingUsers { get; set; }

        [Display(Name = "NetworkAccessLevel_ModerateNetwork", Description = "NetworkAccessLevel_ModerateNetwork_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool ModerateNetwork { get; set; }

        [Display(Name = "NetworkAccessLevel_ManageClubs", Description = "NetworkAccessLevel_ManageClubs_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool ManageClubs { get; set; }

        [Display(Name = "NetworkAccessLevel_ManageSubscriptions", Description = "NetworkAccessLevel_ManageSubscriptions_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool ManageSubscriptions { get; set; }

        [Display(Name = "NetworkAccessLevel_ManagePartnerResources", Description = "NetworkAccessLevel_ManagePartnerResources_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool ManagePartnerResources { get; set; }

        [Display(Name = "NetworkAccessLevel_NetworkAdmin", Description = "NetworkAccessLevel_NetworkAdmin_Desc", ResourceType = typeof(NetworksEnumMessages))]
        public bool NetworkAdmin { get; set; }

        [Display(Name = "Sparkle Staff")]
        public bool SparkleStaff { get; set; }

    }
}
