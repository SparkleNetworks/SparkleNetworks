
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class JobModel
    {
        public JobModel()
        {
        }

        public JobModel(int id, string name, string alias)
        {
            this.Id = id;
            this.Name = name;
            this.Alias = alias;
        }

        public JobModel(Entities.Networks.Job item)
        {
            this.Id = item.Id;
            this.Name = item.Libelle;
            this.Alias = item.Alias;
            this.GroupName = item.GroupName;
        }

        public static JobModel NoJob
        {
            get { return new JobModel(); }
        }

        public int Id { get; set; }

        public string Alias { get; set; }

        public string Name { get; set; }

        public string GroupName { get; set; }

        public int? UserCount { get; set; }
    }
}
