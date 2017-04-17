
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Common;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Users;

    public interface  IInvitedService : IServiceBase
    {
        bool CheckCode(Guid code);
        bool CheckEmail(string email);

        string GetEmail(Guid code);
        int Insert(Invited item);

        [Obsolete("Use specific methods Get*ByEmailDomain() or Get*ByCompany()")]
        IList<Invited> SelectByEmailDomain(string emailDomain);

        IList<Invited> GetPendingByEmailDomain(string emailDomain);
        IList<Invited> GetAcceptedByEmailDomain(string emailDomain);
        IList<Invited> GetAllByEmailDomain(string emailDomain);

        IList<Invited> GetPendingByCompany(int companyId);
        IList<Invited> GetAcceptedByCompany(int companyId);
        IList<Invited> GetAllByCompany(int companyId);

        [Obsolete("Use specific methods Count*ByEmailDomain()")]
        int CountByEmailDomain(string emailDomain);

        int CountPendingByEmailDomain(string emailDomain);
        int CountAcceptedByEmailDomain(string emailDomain);
        int CountAllByEmailDomain(string emailDomain);

        IList<Invited> SelectAll();
        IList<Invited> SelectForNewsletter();
        Invited Update(Invited item);

        /// <summary>
        /// Invites the specified inviter.
        /// </summary>
        /// <param name="inviter">The inviter.</param>
        /// <param name="email">The email.</param>
        /// <param name="companyId">The company id.</param>
        /// <returns></returns>
        InvitePersonResult Invite(User inviter, string email, int companyId = 0, CompanyAccessLevel companyAccess = CompanyAccessLevel.User);
        InvitePersonResult InviteAgain(User inviter, string email);

        Invited SelectById(int invitedId);

        Invited SelectByIdAndEmail(int id, string email);

        Invited MarkAsRegistred(Guid code, int userId);

        Invited GetByInvitationKey(Guid invitationKey);

        IList<Invited> GetRegistered();

        Invited GetByEmail(string email);
        Invited GetByEmail(string email, bool includeUserDetails);

        int CountForNewsletter();

        int CountForNoNewsletter();

        int CountPending();

        IList<Invited> GetPending();

        ValidateInvitationCodeResult ValidateCode(Guid code);

        DeleteInvitationResult Delete(DeleteInvitationRequest request);

        IList<InvitedModel> GetByUserId(int userId, bool loadInviters);
    }
}
