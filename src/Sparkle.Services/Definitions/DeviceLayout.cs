
namespace Sparkle.Services.Definitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DeviceLayout
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool CanBeUsedAsDefault { get; set; }
    }
}
