
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Sparkle.Services.Networks.Clubs;

    public class ClubsService : ServiceBase, IClubsService
    {
        [DebuggerStepThrough]
        internal ClubsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public ClubModel GetById(int clubId)
        {
            Club item = this.Repo.Clubs
                .Select()
                .Where(c => c.Id == clubId)
                .SingleOrDefault();

            return item != null ? new ClubModel(item) : null;
        }

        public IList<ClubModel> SelectAll(bool allNetworks = false)
        {
            var items = this.Repo.Clubs
                .Select()
                .Where(c => c.NetworkId == null || c.NetworkId == this.Services.NetworkId)
                .ToArray();

            return items.Select(c => new ClubModel(c)).ToList();
        }

        public ClubModel GetByAlias(string alias)
        {
            Club item = this.Repo.Clubs
                .Select()
                .Where(c => c.Alias == alias)
                .SingleOrDefault();

            return item != null ? new ClubModel(item) : null;
        }

        public int Count()
        {
            return this.Repo.Clubs
                .Select()
                .Count();
        }

        public string MakeAlias(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The value cannot be empty", "name");

            string alias = name.Trim().MakeUrlFriendly(true);
            if (this.Repo.Clubs.GetIdByAlias(alias) != null)
            {
                alias = alias.GetIncrementedString(a => this.Repo.Clubs.GetIdByAlias(a) == null);
            }

            return alias;
        }

        public EditClubResult Create(EditClubRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new EditClubResult(request);
            if (!request.IsValid)
                return result;

            var item = new Sparkle.Entities.Networks.Club();
            item.Name = request.Name;
            item.Alias = this.Services.Clubs.MakeAlias(request.Name);
            item.Baseline = request.Baseline;
            item.About = request.About;
            item.Website = request.Website;
            item.Phone = request.Phone;
            item.Email = request.Email;
            item.CreatedByUserId = request.CreatedByUserId;
            item.CreatedDateUtc = DateTime.UtcNow;
            item.NetworkId = this.Services.NetworkId;

            item = this.Repo.Clubs.Insert(item);
            result.Item = item;
            result.Succeed = true;

            return result;
        }

        public ClubModel Update(ClubModel model)
        {
            var item = this.Repo.Clubs.GetById(model.Id);

            if (item.Name != model.Name)
            {
                item.Name = model.Name;
                item.Alias = this.Services.Clubs.MakeAlias(model.Name);
            }

            item.Baseline = model.Baseline;
            item.About = model.About;
            item.Website = model.Website;
            item.Phone = model.Phone;
            item.Email = model.Email;

            item = this.Repo.Clubs.Update(item);

            return new ClubModel(item);
        }

        public EditClubRequest GetEditRequest(int id)
        {
            var item = this.Repo.Clubs.GetById(id);
            return item != null ? new EditClubRequest(item) : null;
        }

        public EditClubResult Edit(EditClubRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var item = this.Repo.Clubs.GetById(request.Id);
            var result = new EditClubResult(request);

            if (!request.IsValid || item == null)
                return result;

            if (item.Name != request.Name)
            {
                item.Name = request.Name;
                item.Alias = this.Services.Clubs.MakeAlias(request.Name);
            }

            item.Baseline = request.Baseline;
            item.About = request.About;
            item.Website = request.Website;
            item.Phone = request.Phone;
            item.Email = request.Email;
            item = this.Repo.Clubs.Update(item);

            result.Succeed = true;
            result.Item = item;

            return result;
        }

        public void Delete(int id)
        {
            var item = this.Repo.Clubs.GetById(id);
            if (item != null)
            {
                this.Repo.Clubs.Delete(item);
            }
        }
    }
}
