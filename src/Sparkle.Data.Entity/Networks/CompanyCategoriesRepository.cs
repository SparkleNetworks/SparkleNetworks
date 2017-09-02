
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class CompanyCategoriesRepository : BaseNetworkRepository<CompanyCategory, short>, ICompanyCategoriesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public CompanyCategoriesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.CompanyCategories)
        {
        }

        public IList<CompanyCategory> GetAll()
        {
            return this.Set
                .OrderBy(c => c.Name)
                .ToList();
        }

        public IList<CompanyCategory> GetCategoriesUsedInNetwork(int networkId)
        {
            return this.Context.GetCompanyCategoriesUsedInNetwork(networkId).ToList();
        }

        public CompanyCategory GetByName(string name)
        {
            return this.Set
                .Where(o => o.Name == name)
                .SingleOrDefault();
        }

        public IList<CompanyCategory> GetAll(int networkId)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId)
                .ToList();
        }

        protected override short GetEntityId(CompanyCategory item)
        {
            return item.Id;
        }

        protected override CompanyCategory GetById(NetworksEntities model, short id)
        {
            return this.GetSet(model).SingleOrDefault(o => o.Id == id);
        }

        public IDictionary<short, int> GetActiveCompaniesUsingCount()
        {
            return this.Context.Companies
                .Where(o => o.IsApproved && o.IsEnabled)
                .GroupBy(o => o.CategoryId)
                .ToDictionary(o => o.Key, o => o.Count());
        }

        public IDictionary<short, int> GetInactiveCompaniesUsingCount()
        {
            return this.Context.Companies
                .Where(o => !o.IsApproved || !o.IsEnabled)
                .GroupBy(o => o.CategoryId)
                .ToDictionary(o => o.Key, o => o.Count());
        }

        public void SetDefaultCategory(short categoryId, int networkId)
        {
            var newDefault = this.Set.Where(o => o.Id == categoryId && o.NetworkId == networkId).SingleOrDefault();
            if (newDefault != null)
            {
                var oldDefaults = this.Set
                    .Where(o => o.IsDefault && o.NetworkId == networkId)
                    .ToList();
                foreach (var item in oldDefaults)
                {
                    item.IsDefault = false;
                    this.Update(item);
                }

                newDefault.IsDefault = true;
                this.Update(newDefault);
            }
        }

        public IList<CompanyCategory> GetByAliasNull()
        {
            return this.Set
                .Where(o => o.Alias == null || string.IsNullOrEmpty(o.Alias))
                .ToList();
        }

        public CompanyCategory GetByAlias(string alias, int networkId)
        {
            return this.Set
                .Where(o => o.Alias == alias && o.NetworkId == networkId)
                .SingleOrDefault();
        }

        public CompanyCategory GetDefault(int networkId)
        {
            return this.Set
                .Where(o => o.IsDefault && o.NetworkId == networkId)
                .SingleOrDefault();
        }

        public CompanyCategory GetAny(int networkId)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId)
                .FirstOrDefault();
        }

        public IDictionary<short, CompanyCategory> GetById(short[] ids)
        {
            return this.Set
                .Where(o => ids.Contains(o.Id))
                .ToDictionary(c => c.Id, c => c);
        }

        public int Count(int networkId)
        {
            var count = this.Context.Companies
                .ByNetwork(networkId)
                .Count();
            return count;
        }
    }
}
