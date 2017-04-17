
namespace Sparkle.Entities.Networks
{
    using System;

    partial class Network : IEntityInt32Id
    {
        public NetworkType Type { get; set; }

        public override string ToString()
        {
            return this.Id + " (" + this.Name + ")";
        }
    }

    partial class NetworkType : IEntityInt32Id
    {
    }

    [Flags]
    public enum NetworkOptions
    {
        None = 0,
        Type = 0x1,
    }
}
