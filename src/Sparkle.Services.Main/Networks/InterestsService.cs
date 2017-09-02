
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data;
    using Sparkle.Data.Filters;
    using Sparkle.Entities;
    using System.Diagnostics;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Tags;
    
    /// <summary>
    /// THIS IS PART OF TAGS V1 PACKAGE. THIS HAS TO BE REMOVED.
    /// </summary>
    public class InterestsService : ServiceBase, IInterestsService
    {
        protected IInterestsRepository InterestsRepository
        {
            get { return this.Repo.Interests; }
        }

        [DebuggerStepThrough]
        internal InterestsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Interest Update(Interest item)
        {
            return this.InterestsRepository.Update(item);
        }

        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Interest Insert(Interest item)
        {
            return this.InterestsRepository.Insert(item);
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Delete(Interest item)
        {
            this.InterestsRepository.Delete(item);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public IList<Interest> SelectAll()
        {
            return InterestsRepository
                .Select()
                //.ChildOf(1)
                .OrderBy(o => o.TagName)
                .ToList();
        }

        /// <summary>
        /// Selects the interests by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public Interest GetById(int id)
        {
            return InterestsRepository
                .Select()
                .ById(id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the name of the interests by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Interest SelectInterestsByName(string name)
        {
            return InterestsRepository
                .Select()
                .ByName(name)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the interests from user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<Interest> SelectInterestsFromUserId(int userId)
        {
            return InterestsRepository
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
        public IList<Interest> Search(string request, int take)
        {
            var list = new Dictionary<int, Interest>();

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

        private Interest[] SearchImpl(string request, int take)
        {
            return this.InterestsRepository
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

        public IDictionary<string, int> GetByNames(string[] names)
        {
            return this.Repo.Interests
                .GetByNames(names)
                .ToDictionary(o => o.TagName.RemoveDiacritics().ToLowerInvariant(), o => o.Id);
        }

        public Interest GetByName(string name)
        {
            return this.Repo.Interests
                .GetByName(name);
        }

        public IDictionary<int, Tag2Model> GetAllForCache()
        {
            return this.Repo.Interests.GetAll()
                .ToDictionary(t => t.Key, t => new Tag2Model(t.Value));
        }
    }
}
