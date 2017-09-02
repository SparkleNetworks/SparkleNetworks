
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class HintsService : ServiceBase, IHintsService
    {
        [DebuggerStepThrough]
        public HintsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public void Initialize()
        {
            var logPath = "HintsService.Initialize";
            var defaultAliases = new List<HintModel>()
            {
                KnownHints.MaxAdSeenDate,
            };

            var all = this.Repo.Hints.GetAll(this.Services.NetworkId);
            foreach (var item in defaultAliases)
            {
                if (!all.Any(x => x.Alias.Equals(item.Alias)))
                {
                    var entity = new Hint();
                    entity.Alias = item.Alias;
                    entity.Description = item.Description;
                    entity.HintTypeId = (int)item.HintType;
                    entity.IsEnabled = item.IsEnabled;
                    entity.Location = item.Location;
                    entity.NetworkId = item.NetworkId != null ? this.Services.NetworkId : default(int?);
                    entity.Title = item.Title;
                    this.Repo.Hints.Insert(entity);
                    this.Services.Logger.Info(logPath, ErrorLevel.Success, "Created default hint: " + entity);
                }
            }
        }

        public HintToUserModel GetUserRelation(string alias, int userId)
        {
            var hint = this.GetByAlias(alias);
            if (hint == null)
                throw new ArgumentException("Invalid hint alias '" + alias + "'", "alias");

            var item = this.Repo.HintsToUsers.Get(hint.Id, userId);
            if (item == null)
            {
                return new HintToUserModel(hint.Id, userId);
            }
            else
            {
                return new HintToUserModel(item);
            }
        }

        public HintToUserModel SetUserRelation(int hintId, int userId, DateTime dateDismissedUtc)
        {
            var tran = this.Repo.NewTransaction();
            using (tran.BeginTransaction())
            {
                var item = tran.Repositories.HintsToUsers.Get(hintId, userId);
                if (item != null)
                {
                    item.DateDismissedUtc = dateDismissedUtc;
                }
                else
                {
                    item = new HintsToUser();
                    item.DateDismissedUtc = dateDismissedUtc;
                    item.UserId = userId;
                    item.HintId = hintId;
                    tran.Repositories.HintsToUsers.Attach(item);
                }

                tran.CompleteTransaction();
                return new HintToUserModel(item);
            }
        }

        public IDictionary<int, HintModel> GetAllForCache()
        {
            var items = this.Repo.Hints.GetAll(this.Services.NetworkId);
            var models = new Dictionary<int, HintModel>(items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                models.Add(items[i].Id, new HintModel(items[i]));
            }

            return models;
        }

        public HintModel GetByAlias(string alias)
        {
            return this.Services.Cache.GetHintByAlias(alias);
        }

        public HintModel GetByAliasNoCache(string alias)
        {
            var item = this.Repo.Hints.GetByAlias(this.Services.NetworkId, alias);
            return item != null ? new HintModel(item) : null;
        }


    }
}
