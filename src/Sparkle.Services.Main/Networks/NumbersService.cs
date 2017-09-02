
namespace Sparkle.Services.Main.Networks
{
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    
    /// <summary>
    /// OBSOLETE.
    /// </summary>
    public class NumbersService : ServiceBase, INumbersService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumbersService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        internal NumbersService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected INumbersRepository numbersRepository
        {
            get { return this.Repo.Numbers; }
        }

        public int Insert(Number item)
        {
            this.SetNetwork(item);

            return this.numbersRepository.Insert(item).Id;
        }

        public Number Update(Number item)
        {
            this.VerifyNetwork(item);

            return this.numbersRepository.Update(item);
        }

        public void Delete(Number item)
        {
            this.VerifyNetwork(item);

            this.numbersRepository.Delete(item);
        }

        public IList<Number> SelectAll()
        {
            return numbersRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .OrderByDescending(o => o.Name)
                .ToList();
        }
    }
}
