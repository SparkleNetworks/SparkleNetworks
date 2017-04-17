
namespace Sparkle.ApiClients.InformationNotes
{
    using Sparkle.ApiClients.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class InformationNoteModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int NetworkId { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public DateTime StartDateUtc { get; set; }

        [DataMember]
        public DateTime EndDateUtc { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }
    }
}
