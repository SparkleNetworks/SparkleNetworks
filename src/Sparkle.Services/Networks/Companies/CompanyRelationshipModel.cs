
namespace Sparkle.Services.Networks.Companies
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CompanyRelationshipModel
    {
        public void Set(CompanyRelationship item)
        {
            this.Id = item.Id;
            this.TypeId = item.TypeId;
            this.MasterId = item.MasterId;
            this.SlaveId = item.SlaveId;
            this.DateCreatedUtc = item.DateCreatedUtc;

            if (item.RelationshipTypeReference.IsLoaded)
            {
                this.Type = new CompanyRelationshipTypeModel(item.RelationshipType);
            }
            else
            {
                this.Type = new CompanyRelationshipTypeModel(item.TypeId);
            }

            if (item.MasterReference.IsLoaded)
            {
                this.Master = new CompanyModel(item.Master);
            }

            if (item.SlaveReference.IsLoaded)
            {
                this.Slave = new CompanyModel(item.Slave);
            }
        }

        public CompanyRelationshipModel(CompanyRelationship item)
        {
            this.Set(item);
        }

        public int Id { get; set; }

        public int TypeId { get; set; }

        public CompanyRelationshipTypeModel Type { get; set; }

        public int MasterId { get; set; }

        public CompanyModel Master { get; set; }

        public int SlaveId { get; set; }

        public CompanyModel Slave { get; set; }

        public DateTime DateCreatedUtc { get; set; }
    }
}
