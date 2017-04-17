
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;


    public class CompanyRequestModel
    {
        [IgnoreDataMember]
        public CompanyRequest RequestEntity { get; set; }

        [IgnoreDataMember]
        public Company CompanyEntity { get; set; }

        public int PendingMembers { get; set; }
        public int RegisteredMembers { get; set; }

        public bool CanDelete
        {
            get { return this.RequestEntity != null ? /*this.RequestEntity.RejectedTimes > 0 && */this.RequestEntity.Approved != true && this.RequestEntity.CompanyId == null : false; }
        }
    }
}
