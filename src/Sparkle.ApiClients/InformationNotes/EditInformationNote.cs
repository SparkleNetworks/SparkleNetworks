
namespace Sparkle.ApiClients.InformationNotes
{
    using Sparkle.ApiClients.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class EditInformationNoteRequest : BaseRequest
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime UserStartDate { get; set; }

        [DataMember]
        public DateTime UserEndDate { get; set; }
    }

    [DataContract(Namespace = Names.PublicNamespace)]
    public class EditInformationNoteResult : BaseResult
    {
        [DataMember]
        public InformationNoteModel Item { get; set; }
    }
}
