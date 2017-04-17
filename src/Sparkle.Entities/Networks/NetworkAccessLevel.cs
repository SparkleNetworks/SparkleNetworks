
namespace Sparkle.Entities.Networks
{
    using System;

    [Flags]
    public enum NetworkAccessLevel
    {
        Disabled = 0x00000000,

        /// <summary>
        /// 1 Indicates the user can access the platform.
        /// </summary>
        User = 0x00000001,

        /// <summary>
        /// 2 Indicates the user can create a new company.
        /// </summary>
        AddCompany = 0x00000002,

        /// <summary>
        /// 4 Indicates the user can look at the network's statistics.
        /// </summary>
        ReadNetworkStatistics = 0x00000004,

        /// <summary>
        /// Indicates the user can see and manage connected devices.
        /// </summary>
        ManageDevices = 0x00000008 | 0x00000010,

        /// <summary>
        /// Indicates the user can see connected devices.
        /// </summary>
        ReadDevices =            0x00000010,

        ChangeCompanyFlags =     0x00000020,
        ManageInformationNotes = 0x00000040,
        ManageRegisterRequests = 0x00000080,
        ValidatePublications =   0x00000100,
        ManageCompany =          0x00000200,
        ValidatePendingUsers =   0x00000400,
        ModerateNetwork =        0x00000800,
        ManageClubs =            0x00001000,
        ManageSubscriptions =    0x00002000,
        ManagePartnerResources = 0x00004000,
        NetworkAdmin =           0x00010000,
        ContentManager =         0x00020000,
        ManagePages =            0x00040000,

        /// <summary>
        /// Special value with all rights enabled for SparkleNetworks staff.
        /// </summary>
        SparkleStaff = 0x40000000,


        All = User | AddCompany | ReadNetworkStatistics | ManageDevices | ReadDevices | ChangeCompanyFlags | ManageInformationNotes | ValidatePublications | SparkleStaff | ManageRegisterRequests | ManageCompany | NetworkAdmin | ValidatePendingUsers | ManageClubs | ModerateNetwork | ManagePartnerResources | ManageSubscriptions | ManageClubs | ContentManager | ManagePages,
    }

    public static class EnumExtensions
    {
        public static bool HasFlag(this NetworkAccessLevel value, NetworkAccessLevel flag)
        {
            return (value & flag) == flag;
        }

        public static bool HasAnyFlag(this NetworkAccessLevel value, params NetworkAccessLevel[] flags)
        {
            for (int i = 0; i < flags.Length; i++)
            {
                if (value.HasFlag(flags[i]))
                    return true;
            }

            return false;
        }

        public static bool HasFlag(this CompanyAccessLevel value, CompanyAccessLevel flag)
        {
            return (value & flag) == flag;
        }

        public static bool HasAnyFlag(this CompanyAccessLevel value, params CompanyAccessLevel[] flags)
        {
            for (int i = 0; i < flags.Length; i++)
            {
                if (value.HasFlag(flags[i]))
                    return true;
            }

            return false;
        }
    }
}
