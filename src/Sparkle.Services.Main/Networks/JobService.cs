
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Users;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class JobService : ServiceBase, IJobService
    {
        private static NetworkAccessLevel[] editJobsRoles = new NetworkAccessLevel[] { NetworkAccessLevel.ContentManager, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff, };

        [DebuggerStepThrough]
        internal JobService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IJobRepository JobRepository
        {
            get { return this.Repo.Job; }
        }

        /// <summary>
        /// Adds the job.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Job AddJob(Job item)
        {
            return this.JobRepository.Insert(item);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public IList<Job> SelectAll()
        {
            return this.JobRepository
                .Select()
                .OrderBy(o => o.Libelle)
                .ToList();
        }

        /// <summary>
        /// Selects the with id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public Job SelectWithId(int? id)
        {
            if (id.HasValue)
            {
                return JobRepository
                .Select()
                .WithId(id.Value)
                .FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Selects the with alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public Job SelectWithAlias(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
            {
                return JobRepository
                .Select()
                .WithAlias(alias)
                .FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Searches the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public IList<Job> Search(string request)
        {
            return JobRepository
                .Select()
                .Contain(request)
                .OrderBy(o => o.Libelle)
                .Take(5)
                .ToList();
        }

        public int Count()
        {
            return this.Repo.Job.Count();
        }

        public IList<JobModel> GetAll()
        {
            return this.Repo.Job.GetJobsForNetwork(this.Services.NetworkId)
                .Select(j => new JobModel(j))
                .ToList();
        }

        public IList<JobModel> GetAllWithCounts()
        {
            var items = this.Repo.Job.GetJobsForNetwork(this.Services.NetworkId)
                .Select(j => new JobModel(j))
                .ToList();

            var counts = this.Repo.People.GetJobCounts(this.Services.NetworkId);
            foreach (var item in items)
            {
                if (counts.ContainsKey(item.Id))
                {
                    item.UserCount = counts[item.Id];
                }
            }

            return items;
        }

        public void AddManyJob(string[] jobs)
        {
            foreach (var job in jobs)
            {
                var exist = this.Repo.Job.Select().Where(o => o.Libelle == job).SingleOrDefault();
                if (exist == null)
                {
                    var jo = new Job
                    {
                        Libelle = job,
                        Alias = job.MakeUrlFriendly(true),
                    };
                    this.Repo.Job.Insert(jo);
                }
            }
        }

        public string MakeAlias(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The value cannot be empty", "name");

            var repo = this.Repo.Job;
            string alias = MakeAlias(name, repo);
            return alias;
        }

        public DeleteJobRequest GetDeleteRequest(int? id, DeleteJobRequest request)
        {
            if (request == null)
            {
                request = new DeleteJobRequest();
            }

            if (id != null)
            {
                var job = this.GetById(id.Value);
                if (job == null)
                {
                    return null;
                }
            }

            request.Jobs = this.GetAll();
            request.Jobs.Insert(0, new JobModel(-1, this.Services.Lang.T("Do not migrate"), null));

            return request;
        }

        public DeleteJobResult Delete(DeleteJobRequest request)
        {
            const string path = "JobService.Delete";
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new DeleteJobResult(request);

            if (request.ActingUserId == null)
            {
                result.Errors.Add(DeleteJobError.NoSuchActingUser, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, path);
            }

            var tran = this.Repo.NewTransaction();
            using (tran.BeginTransaction())
            {
                var actingUser = tran.Repositories.People.GetActiveById(request.ActingUserId.Value, Data.Options.PersonOptions.Company);
                if (actingUser != null)
                {
                    var userModel = new UserModel(actingUser);
                    if (userModel.IsActive)
                    {
                        if (!userModel.NetworkAccessLevel.Value.HasAnyFlag(editJobsRoles))
                        {
                            result.Errors.Add(DeleteJobError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                        }
                    }
                    else
                    {
                        result.Errors.Add(DeleteJobError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                    }
                }
                else
                {
                    result.Errors.Add(DeleteJobError.NoSuchActingUser, NetworksEnumMessages.ResourceManager);
                }

                var deleteJob = tran.Repositories.Job.GetById(request.Id);
                if (deleteJob == null)
                {
                    result.Errors.Add(DeleteJobError.NoSuchItem, NetworksEnumMessages.ResourceManager);
                }

                Job targetJob = null;
                if (request.TargetJobId != null && !request.TargetJobId.Value.Equals(-1))
                {
                    targetJob = tran.Repositories.Job.GetById(request.TargetJobId.Value);
                    if (targetJob == null)
                    {
                        result.Errors.Add(DeleteJobError.NoSuchItem, NetworksEnumMessages.ResourceManager);
                    }
                    else if (targetJob.Id == deleteJob.Id)
                    {
                        result.Errors.Add(DeleteJobError.SameTarget, NetworksEnumMessages.ResourceManager);
                    }
                }

                if (result.Errors.Count > 0)
                {
                    return this.LogResult(result, path);
                }

                var userCount = tran.Repositories.People.CountByJobId(deleteJob.Id);

                tran.Repositories.Job.DeleteJob(request.Id, targetJob != null ? targetJob.Id : default(int?));
                tran.ExecuteChanges();
                tran.CompleteTransaction();

                result.Succeed = true;
                return this.LogResult(result, path, "Deleted Job " + deleteJob + " to Job " + (targetJob != null ? targetJob.ToString() : "no job") + ".");
            }
        }

        public JobModel GetById(int id)
        {
            var entity = this.Repo.Job.GetById(id);
            if (entity != null)
            {
                var item = new JobModel(entity);
                return item;
            }
            else
            {
                return null;
            }
        }

        public EditJobRequest GetEditRequest(int? id, EditJobRequest request)
        {
            if (request == null)
            {
                request = new EditJobRequest();
            }

            if (id != null)
            {
                var job = this.GetById(id.Value);
                if (job == null)
                {
                    return null;
                }

                request.Id = job.Id;
                request.Name = job.Name;
                request.GroupName = job.GroupName;

                request.UserCount = this.Repo.People.CountByJobId(job.Id);
            }

            request.GroupNames = this.Repo.Job.GetGroupNames();

            return request;
        }

        public EditJobResult Edit(EditJobRequest request)
        {
            const string path = "JobService.Edit";
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new EditJobResult(request);

            if (request.ActingUserId == null)
            {
                result.Errors.Add(EditJobError.NoSuchActingUser, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, path);
            }

            var tran = this.Repo.NewTransaction();
            using (tran.BeginTransaction())
            {
                var actingUser = tran.Repositories.People.GetActiveById(request.ActingUserId.Value, Data.Options.PersonOptions.Company);
                if (actingUser != null)
                {
                    var userModel = new UserModel(actingUser);
                    if (userModel.IsActive)
                    {
                        if (!userModel.NetworkAccessLevel.Value.HasAnyFlag(editJobsRoles))
                        {
                            result.Errors.Add(EditJobError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                        }
                    }
                    else
                    {
                        result.Errors.Add(EditJobError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                    }
                }
                else
                {
                    result.Errors.Add(EditJobError.NoSuchActingUser, NetworksEnumMessages.ResourceManager);
                }

                var repo = tran.Repositories.Job;

                Job entity;
                if (request.Id > 0)
                {
                    entity = repo.GetById(request.Id);

                    if (entity == null)
                    {
                        result.Errors.Add(EditJobError.NoSuchItem, NetworksEnumMessages.ResourceManager);
                    }
                }
                else
                {
                    entity = new Job();

                    var nameMatches = repo.GetByName(request.Name);
                    if (nameMatches.Count > 0)
                    {
                        result.Errors.Add(EditJobError.NameAlreadyInUse, NetworksEnumMessages.ResourceManager);
                    }

                }

                if (result.Errors.Count > 0)
                {
                    return this.LogErrorResult(result, path);
                }

                entity.Alias = entity.Alias ?? this.MakeAlias(request.Name, repo);
                entity.Libelle = request.Name;
                entity.GroupName = request.GroupName.NullIfEmptyOrWhitespace();

                if (entity.Id > 0)
                {
                    repo.Update(entity);
                }
                else
                {
                    repo.Insert(entity);
                }

                tran.CompleteTransaction();

                result.Succeed = true;
                result.Item = new JobModel(entity);
                return this.LogResult(result, path, "Update Job " + result.Item + ".");
            }
        }

        private string MakeAlias(string name, IJobRepository repo)
        {
            string alias = name.Trim().MakeUrlFriendly(true);
            if (repo.GetByAlias(alias) != null)
            {
                alias = alias.GetIncrementedString(a => this.Repo.Job.GetByAlias(a) == null);
            }

            return alias;
        }
    }
}