
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Sparkle.Common;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;

    public class ResumesService : ServiceBase, IResumesService
    {
        internal ResumesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IResumesRepository resumeRepository
        {
            get { return this.Repo.Resumes; }

        }

        public Resume Insert(Resume item)
        {
            this.SetNetwork(item);

            return this.resumeRepository.Insert(item);
        }

        public Resume Update(Resume item)
        {
            this.VerifyNetwork(item);

            return this.resumeRepository.Update(item);
        }

        public void Delete(Resume item)
        {
            this.VerifyNetwork(item);

            this.resumeRepository.Delete(item);
        }

        public IList<Resume> SelectApproved(bool allNetworks = false)
        {
            if (allNetworks)
            {
                return this.resumeRepository.Select()
                .Where(r => r.IsApproved).ToList();
            }

            return this.resumeRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(r => r.IsApproved).ToList();
        }

        public IList<Resume> SelectUnapproved(bool allNetworks = false)
        {
            if (allNetworks)
            {
                return this.resumeRepository.Select()
                .Where(r => !r.IsApproved).ToList();
            }

            return this.resumeRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(r => !r.IsApproved).ToList();
        }

        public IList<Resume> SelectAll(bool allNetworks = false)
        {
            if (allNetworks)
            {
                return this.resumeRepository.Select().ToList();
            }

            return this.resumeRepository.Select()
                .ByNetwork(this.Services.NetworkId).ToList();
        }

        public Resume GetById(int resumeId)
        {
            return this.resumeRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(r => r.Id == resumeId)
                .SingleOrDefault();
        }

        public Resume GetByEmail(string email)
        {
            return this.resumeRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(r => r.Email == email)
                .SingleOrDefault();
        }

        public int CountApproved(bool allNetworks = false)
        {
            if (allNetworks)
            {
                return this.resumeRepository.Select()
                .Where(r => r.IsApproved).Count();
            }

            return this.resumeRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(r => r.IsApproved).Count();
        }

        public int CountUnapproved(bool allNetworks = false)
        {
            if (allNetworks)
            {
                return this.resumeRepository.Select()
                .Where(r => !r.IsApproved).Count();
            }

            return this.resumeRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(r => !r.IsApproved).Count();
        }

        public int CountPending(bool allNetworks = false)
        {
            if (allNetworks)
            {
                return this.resumeRepository.Select()
                    .Where(r => !r.IsApproved).Count();
            }

            return this.resumeRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(r => !r.IsApproved).Count();
        }
    }
}
