
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class VisibilityModel
    {
        public VisibilityModel()
        {
        }

        public VisibilityModel(short id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public short Id { get; set; }

        public string Name { get; set; }
    }
}
