
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Tags;
    
    /// <summary>
    /// THIS IS PART OF TAGS V1 PACKAGE. THIS HAS TO BE REMOVED.
    /// </summary>
    public class RecreationsService : ServiceBase, IRecreationsService
    {
        /// <summary>
        /// Gets the recreations repository.
        /// </summary>
        protected IRecreationsRepository RecreationsRepository
        {
            get { return this.Repo.Recreations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecreationsService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        internal RecreationsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Recreation Update(Recreation item)
        {
            return this.RecreationsRepository.Update(item);
        }

        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int Insert(Recreation item)
        {
            return this.RecreationsRepository.Insert(item).Id;
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Delete(Recreation item)
        {
            this.RecreationsRepository.Delete(item);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public IList<Recreation> SelectAll()
        {
            return RecreationsRepository
                .Select()
                //.ChildOf(1)
                .OrderBy(o => o.TagName)
                .ToList();
        }

        /// <summary>
        /// Selects the recreations by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public Recreation GetById(int id)
        {
            return RecreationsRepository
                .Select()
                .ById(id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the name of the recreations by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Recreation SelectRecreationsByName(string name)
        {
            return RecreationsRepository
                .Select()
                .ByName(name)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the recreations from user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<Recreation> SelectRecreationsFromUserId(Guid userId)
        {
            return RecreationsRepository
                .Select()
                .ChildOf(1)
                .ToList();
        }

        /// <summary>
        /// Searches the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="take">The take.</param>
        /// <returns></returns>
        public IList<Recreation> Search(string request, int take)
        {
            var list = new Dictionary<int, Recreation>();

            list.AddRange(this.SearchImpl(request, take), s => s.Id, s => s);

            var splitted = request.Split(' ', '\t', '\r', '\n', ',', ';', '/', '&', '\'');
            splitted = splitted
                .Select(s => s.Trim().ToLowerInvariant())
                .Where(s => !string.IsNullOrEmpty(s) && !excludeWords.Contains(s))
                .Select(s => s.EndsWith("ies") ? s.Substring(0, s.Length - 3) : s.EndsWith("s") ? s.Substring(0, s.Length - 1) : s)
                .ToArray();

            for (int i = 0; i < splitted.Length; i++)
            {
                var results = this.SearchImpl(splitted[i], take);
                for (int j = 0; j < results.Length; j++)
                {
                    if (!list.ContainsKey(results[j].Id))
                        list.Add(results[j].Id, results[j]);
                }
            }

            return list.Values.Take(take).ToList();
        }

        private Recreation[] SearchImpl(string request, int take)
        {
            return this.RecreationsRepository
                .Select()
                .Contain(request)
                .OrderBy(o => o.TagName)
                .Take(take)
                .ToArray();
        }

        private static readonly string[] excludeWords = new string[]
        {
            "le", "la", "les", "l",
            "the", "a",
        };

        public Recreation GetByName(string name)
        {
            return this.Repo.Recreations
                .GetByName(name);
        }

        public IDictionary<int, Tag2Model> GetAllForCache()
        {
            return this.Repo.Recreations.GetAll()
                .ToDictionary(t => t.Key, t => new Tag2Model(t.Value));
        }
    }
}
