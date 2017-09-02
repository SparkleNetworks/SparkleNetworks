
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface IApplyRequestsRepository : IBaseNetworkRepository<ApplyRequest, int>
    {
        ApplyRequest GetByKey(Guid key, int networkId);
        ApplyRequest GetById(int id, int networkId);

        IList<ApplyRequest> GetPending(int networkId);
        IList<ApplyRequest> GetPendingWithCompanyCreate(int networkId);
        IList<ApplyRequest> GetPendingWithCompanyJoin(int networkId);

        IList<ApplyRequest> GetAll(int networkId, int offset, int pageSize);
        int CountAll(int networkId);

        IList<ApplyRequest> GetByJoinCompanyId(int networkId, int companyId);
        int CountByJoinCompanyId(int networkId, int companyId);

        void DeleteAllNotSubmitted(int networkId);

        IList<ApplyRequest> GetByUserId(int userId);

        ////IList<ApplyRequest> GetByEmailAddress(string account, string tag, string domain); // TODO: store email address in this table

        IList<ApplyRequest> FindByImportedId(int networkId, string likeQuery);


    }
}
