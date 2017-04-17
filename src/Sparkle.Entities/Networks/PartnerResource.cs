
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class PartnerResource : IEntityInt32Id, INetworkEntity
    {
    }

    public enum PartnerResourceOptions
    {
        None =         0x00000,
        Tags =         0x00001,
        TagsCategory = 0X00002,
    }
}
