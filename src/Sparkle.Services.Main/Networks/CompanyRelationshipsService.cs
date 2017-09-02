
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Companies;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CompanyRelationshipsService : ServiceBase, ICompanyRelationshipsService
    {
        [DebuggerStepThrough]
        public CompanyRelationshipsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public void Initialize()
        {
            var all = this.Repo.CompanyRelationshipTypes.GetAll(this.Services.NetworkId);
            var expecteds = new CompanyRelationshipTypePoco[]
            {
                new CompanyRelationshipTypePoco
                {
                    Name = "Invited",
                    Verb = "Invited",
                    KnownType = (byte)KnownCompanyRelationshipType.Invited,
                    Rules = "{\"IsReadOnly\":true}",
                },
                new CompanyRelationshipTypePoco
                {
                    Name = "Startup accelerator",
                    Verb = "Accelerates",
                    KnownType = (byte)KnownCompanyRelationshipType.StartupAccelerator,
                },
            };

            bool missingKnownType = false;
            bool missingAlias = all.Any(c => c.Alias == null);
            foreach (var expected in expecteds)
            {
                if (!all.Any(c => c.KnownType == expected.KnownType))
                {
                    missingKnownType = true;
                    break;
                }
            }

            if (!missingKnownType && !missingAlias)
            {
                this.Services.Logger.Verbose("CompanyRelationshipsService.Initialize", ErrorLevel.Success, "All CompanyRelationships OK.");
                return;
            }

            var transaction = this.Services.Repositories.NewTransaction();
            using (transaction.BeginTransaction())
            {
                all = transaction.Repositories.CompanyRelationshipTypes.GetAll(this.Services.NetworkId);
                for (int i = 0; i < expecteds.Length; i++)
                {
                    var expected = expecteds[i];
                    if (!all.Any(c => c.KnownType == expected.KnownType))
                    {
                        var item = new CompanyRelationshipType
                        {
                            Name = expected.Name,
                            Verb = expected.Verb,
                            NetworkId = this.Services.NetworkId,
                            Alias = expected.Name.MakeUrlFriendly(true),
                            KnownType = expected.KnownType,
                            Rules = expected.Rules,
                        };
                        transaction.Repositories.CompanyRelationshipTypes.Insert(item);
                        this.Services.Logger.Info("CompanyRelationshipsService.Initialize", ErrorLevel.Success, "Created CompanyRelationshipType " + item.ToString() + ".");
                    }
                }

                foreach (var item in all)
                {
                    if (item.Alias == null)
                    {
                        item.Alias = item.Name.MakeUrlFriendly(true);
                    }
                }

                transaction.CompleteTransaction();
            }
        }

        public IList<CompanyRelationshipTypeModel> GetAllTypes()
        {
            return this.Repo.CompanyRelationshipTypes
                .GetAll(this.Services.NetworkId)
                .Select(o => new CompanyRelationshipTypeModel(o))
                .ToList();
        }

        public IList<CompanyRelationshipTypeModel> GetNonSystemTypes()
        {
            return this.Repo.CompanyRelationshipTypes
                .GetAll(this.Services.NetworkId)
                .Where(t => t.KnownTypeValue != KnownCompanyRelationshipType.Invited)
                .Select(o => new CompanyRelationshipTypeModel(o))
                .ToList();
        }

        public IList<CompanyRelationshipModel> GetByCompanyId(int companyId, CompanyRelationshipOptions options)
        {
            return this.Repo.CompanyRelationships.GetByCompanyId(companyId, options).Select(o => new CompanyRelationshipModel(o)).ToList();
        }

        public IDictionary<CompanyRelationshipTypeModel, IList<CompanyRelationshipModel>> GetByCompanyIdSortedByType(int companyId)
        {
            var values = new Dictionary<CompanyRelationshipTypeModel, IList<CompanyRelationshipModel>>();

            var types = this.GetAllTypes();
            var relationships = this.GetByCompanyId(companyId, CompanyRelationshipOptions.Master | CompanyRelationshipOptions.Slave);
            foreach (var item in relationships.GroupBy(o => o.TypeId))
            {
                CompanyRelationshipTypeModel type = null;
                if ((type = types.SingleOrDefault(o => o.Id == item.Key)) != null)
                    values.Add(type, item.ToList());
            }

            return values;
        }

        public CompanyRelationshipTypeModel GetTypeById(int? id)
        {
            CompanyRelationshipType item = null;
            if (!id.HasValue || (item = this.Repo.CompanyRelationshipTypes.GetById(id.Value)) == null)
                return null;

            return new CompanyRelationshipTypeModel(item);
        }

        public void InsertFromPoco(Entities.Networks.Neutral.CompanyRelationshipPoco item)
        {
            var newItem = new CompanyRelationship
            {
                TypeId = item.TypeId,
                MasterId = item.MasterId,
                SlaveId = item.SlaveId,
                DateCreatedUtc = DateTime.UtcNow,
            };
            this.Repo.CompanyRelationships.Insert(newItem);
        }

        public IList<CompanyRelationshipTypeModel> GetTypesByKnownType(KnownCompanyRelationshipType type)
        {
            return this.Repo.CompanyRelationshipTypes.GetByKnownType(type, this.Services.NetworkId).Select(o => new CompanyRelationshipTypeModel(o)).ToList();
        }

        public void Insert(CompanyRelationship item)
        {
            this.Repo.CompanyRelationships.Insert(item);
        }
    }
}
