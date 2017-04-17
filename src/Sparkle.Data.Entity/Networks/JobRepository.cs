
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Entity.Networks.Sql;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Data.EntityClient;
    using System.Linq;

    public class JobRepository : BaseNetworkRepositoryInt<Job>, IJobRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public JobRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Jobs)
        {
        }

        public IList<JobWithStats> GetWithStats(int networkId)
        {
            return this.Set
                .Select(j => new JobWithStats
                {
                    Job = j,
                    PeopleCount = j.Users.Where(u => u.NetworkId == networkId).Count(),
                })
                .ToList();
        }

        public int Count()
        {
            return this.Set.Count();
        }

        public IList<Job> GetJobsUsedInNetwork(int networkId)
        {
            return this.Context.GetJobsUsedInNetwork(networkId).ToList();
        }

        public IList<Job> GetAll()
        {
            return this.Set.OrderBy(j => j.Libelle).ToList();
        }

        public IList<Job> GetJobsForNetwork(int networkId)
        {
            return this.Set
                ////.Where(j => j.NetworkId == null || j.NetworkId == networkId)
                .OrderBy(j => j.Libelle)
                .ToList();
        }

        public Job GetByAlias(string alias)
        {
            return this.Set
                .Where(o => o.Alias == alias)
                .SingleOrDefault();
        }

        public void DeleteJob(int deleteJobId, int? targetJobId)
        {
            // Entity Framework fails to execute the SP without failing the transaction
            ////var result = this.Context.DeleteJob(deleteJobId, targetJobId);

            // We call the SP manually to ensure the transaction does not get broken
            DeleteJob_Result result = null;

            var commandText = "EXEC dbo.DeleteJob @deleteJobId = " + deleteJobId
                + ", @targetJobId = " + (targetJobId != null ? targetJobId.Value.ToString() : "NULL");

            var cmd = ((EntityConnection)this.Context.Connection).StoreConnection.EnsureOpen().CreateCommand()
                .SetText(commandText);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    result = new DeleteJob_Result();
                    result.AffectedUsers = reader.GetInt32(reader.GetOrdinal("AffectedUsers"));
                    result.DeletedJobs = reader.GetInt32(reader.GetOrdinal("DeletedJobs"));
                }
            }
        }

        public string[] GetGroupNames()
        {
            var items = this.Set
                .GroupBy(x => x.GroupName)
                .Select(x => x.Key)
                .ToArray();
            return items;
        }

        public IList<Job> GetByName(string name)
        {
            var items = this.Set
                .Where(x => x.Libelle == name)
                .ToArray();
            return items;
        }
    }
}
