
namespace Sparkle.Data.Networks
{
    using System.Collections.Generic;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IJobRepository : IBaseNetworkRepository<Job, int>
    {
        IList<JobWithStats> GetWithStats(int networkId);

        int Count();

        IList<Job> GetJobsUsedInNetwork(int networkId);

        IList<Job> GetAll();

        IList<Job> GetJobsForNetwork(int networkId);

        Job GetByAlias(string alias);

        void DeleteJob(int deleteJobId, int? targetJobId);

        string[] GetGroupNames();

        IList<Job> GetByName(string name);
    }
}
