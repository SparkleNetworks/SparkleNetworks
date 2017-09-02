
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NetworkTypeModel
    {
        public NetworkTypeModel()
        {
        }

        public NetworkTypeModel(int id)
        {
            this.Id = id;
        }

        public NetworkTypeModel(Entities.Networks.NetworkType item)
        {
            this.Id = item.Id;
            this.Name = item.Name;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        internal NetworkTypeModel Clone()
        {
            return new NetworkTypeModel
            {
                Id = this.Id,
                Name = this.Name,
            };
        }
    }
}
