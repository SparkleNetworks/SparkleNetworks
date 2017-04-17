
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class EventCategoryModel
    {
        public EventCategoryModel()
        {
        }

        public EventCategoryModel(Entities.Networks.EventCategory item)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.NetworkId = item.NetworkId;
            this.ParentId = item.ParentId;
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int? NetworkId { get; set; }

        [DataMember]
        public int ParentId { get; set; }

        [DataMember]
        public int? EventsCount { get; set; }
    }
}
