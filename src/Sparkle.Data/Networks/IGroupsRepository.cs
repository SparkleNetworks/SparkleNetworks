
namespace Sparkle.Data.Networks
{
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using System.Linq;

    [Repository]
    public interface IGroupsRepository : IBaseNetworkRepository<Group, int>
    {
        IQueryable<Group> CreateQuery(GroupOptions options);

        Group GetActiveById(int id);

        IList<Group> GetById(int[] ids);
        IList<Group> GetActiveById(int[] ids);
        IList<Group> GetById(int[] ids, int networkId);
        IList<Group> GetActiveById(int[] ids, int networkId);

        IList<Group> GetById(int[] ids, GroupOptions options);
        IList<Group> GetActiveById(int[] ids, GroupOptions options);
        Group GetById(int id, int networkId, GroupOptions options);
        IList<Group> GetById(int[] ids, int networkId, GroupOptions options);
        IList<Group> GetActiveById(int[] ids, int networkId, GroupOptions options);

        Group GetByAlias(string alias, GroupOptions options);
        Group GetByAlias(string alias, int networkId, GroupOptions options);
    }
}
