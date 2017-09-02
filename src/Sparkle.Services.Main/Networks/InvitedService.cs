
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Common;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Users;
    using SrkToolkit.Common.Validation;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;

    public class InvitedService : ServiceBase, IInvitedService
    {
        [DebuggerStepThrough]
        internal InvitedService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IInvitedRepository InvitedRepository
        {
            get { return this.Repo.Invited; }
        }

        public Invited Update(Invited item)
        {
            return this.InvitedRepository.Update(item);
        }

        public int Insert(Invited item)
        {
            return this.InvitedRepository.Insert(item).Id;
        }

        [Obsolete("Use specific methods Get*ByEmailDomain()")]
        public IList<Invited> SelectByEmailDomain(string emailDomain)
        {
            return this.InvitedRepository
                .Select()
                .EmailDomain(emailDomain)
                .ToList();
        }

        public IList<Invited> GetPendingByEmailDomain(string emailDomain)
        {
            return this.InvitedRepository
                .Select()
                .EmailDomain(emailDomain)
                .Pending()
                .ToList();
        }

        public IList<Invited> GetAcceptedByEmailDomain(string emailDomain)
        {
            return this.InvitedRepository
                .Select()
                .EmailDomain(emailDomain)
                .Registered()
                .ToList();
        }

        public IList<Invited> GetAllByEmailDomain(string emailDomain)
        {
            return this.InvitedRepository
                .Select()
                .EmailDomain(emailDomain)
                .ToList();
        }

        public IList<Invited> GetPendingByCompany(int companyId)
        {
            return this.InvitedRepository
                .Select()
                .ByCompany(companyId)
                .Pending()
                .ToList();
        }

        public IList<Invited> GetAcceptedByCompany(int companyId)
        {
            return this.InvitedRepository
                .Select()
                .ByCompany(companyId)
                .Registered()
                .ToList();
        }

        public IList<Invited> GetAllByCompany(int companyId)
        {
            return this.InvitedRepository
                .Select()
                .ByCompany(companyId)
                .ToList();
        }

        [Obsolete("Use specific methods Count*ByEmailDomain()")]
        public int CountByEmailDomain(string emailDomain)
        {
            return this.InvitedRepository
                .Select()
                .EmailDomain(emailDomain)
                .Count();
        }

        public int CountPendingByEmailDomain(string emailDomain)
        {
            return this.InvitedRepository
                .Select()
                .EmailDomain(emailDomain)
                .Pending()
                .Count();
        }

        public int CountAcceptedByEmailDomain(string emailDomain)
        {
            return this.InvitedRepository
                .Select()
                .EmailDomain(emailDomain)
                .Registered()
                .Count();
        }

        public int CountAllByEmailDomain(string emailDomain)
        {
            return this.InvitedRepository
                .Select()
                .EmailDomain(emailDomain)
                .Count();
        }

        public IList<Invited> SelectAll()
        {
            return this.InvitedRepository.Select()
                    .ToList();
        }

        public IList<Invited> GetRegistered()
        {
            return this.InvitedRepository.Select()
                    .Registered()
                    .ToList();
        }

        public Invited GetByEmail(string email)
        {
            return this.InvitedRepository.GetByEmail(email, false);
        }

        public Invited GetByEmail(string email, bool includeUserDetails)
        {
            return this.InvitedRepository.GetByEmail(email, includeUserDetails);
        }

        public IList<Invited> SelectForNewsletter()
        {
            return this.InvitedRepository.Select()
                .NotUnregistered()
                .ToList();
        }

        public bool CheckCode(Guid code)
        {
            IList<Invited> list = this.InvitedRepository.Select()
                    .CheckCode(code)
                    .ToList();
            return list.Count != 0;
        }

        public bool CheckEmail(string email)
        {
            IList<Invited> list = this.InvitedRepository.Select()
                    .CheckEmail(email)
                    .ToList();
            return list.Count != 0;
        }

        public string GetEmail(Guid code)
        {
            Invited item = this.InvitedRepository.Select()
                    .CheckCode(code)
                    .FirstOrDefault();
            return item.Email;
        }

        public InvitePersonResult Invite(User inviter, string email, int companyId = 0, CompanyAccessLevel companyAccess = CompanyAccessLevel.User)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("The value cannot be empty", "email");

            if (Validate.EmailAddress(email) == null)
                throw new ArgumentException("The email address is not in a correct format", "email");

            this.AclEnforcer.AllowAnonymous().AllowUsers().AllowAdministrators().Check();

            ////if (this.Identity.IsUser && this.Identity.Person.InvitationsLeft < 1)
            ////    return InvitePersonResult.QuotaReached;

            var user = this.Services.People.SelectWithProMail(email);
            Company company;

            if (companyId == 0)
            {
                string domain = email.Substring(email.IndexOf('@') + 1);
                company = GetCompanyByDomain(domain);
            }
            else
            {
                company = this.Services.Company.GetById(companyId);
            }

            if (company == null)
                throw new ArgumentException("Cannot determine company for invitation", "companyId");

            inviter = inviter ?? this.Services.People.SelectWithLogin("kevin.alexandre");
            var invite = this.InvitedRepository.Select().FirstOrDefault(i => i.Email == email);

            if (inviter == null)
                throw new ArgumentNullException("inviter");

            if (user != null)
                return InvitePersonResult.UserExists;

            if (invite != null)
                return InvitePersonResult.AlreadyInvited;

            if (company != null)
            {
                invite = new Invited
                {
                    Email = email,
                    CompanyId = company.ID,
                    Date = DateTime.Now,
                    Code = Guid.NewGuid(),
                    InvitedByUserId = inviter.Id,
                    CompanyAccessLevel = (int)companyAccess,
                };

                this.InvitedRepository.Insert(invite);

                try
                {
                    this.Services.Email.SendInvitation(inviter, invite);
                    return new InvitePersonResult
                    {
                        Code = InvitePersonResult.ResultCode.Done,
                        Company = company,
                        Invitation = invite,
                    };
                }
                catch (InvalidOperationException ex)
                {
                    try
                    {
                        this.InvitedRepository.Delete(invite);
                    }
                    catch (DataException)
                    {
                    }

                    var result = InvitePersonResult.SmtpError(ex);
                    result.Error = ex;
                    return result;
                }
            }

            return InvitePersonResult.NoCompany;
        }

        public InvitePersonResult InviteAgain(User inviter, string email)
        {
            this.AclEnforcer.AllowAnonymous().AllowUsers().AllowAdministrators().Check();

            ////if (this.Identity.IsUser && this.Identity.Person.InvitationsLeft < 1)
            ////    return InvitePersonResult.QuotaReached;

            var domain = email.Substring(email.IndexOf('@') + 1);
            var user = this.Services.People.SelectWithProMail(email);
            var company = GetCompanyByDomain(domain);
            inviter = inviter ?? this.Services.People.SelectWithLogin("kevin.alexandre");
            Invited invite = this.InvitedRepository.Select().FirstOrDefault(i => i.Email == email);

            if (user != null)
            {
                return InvitePersonResult.UserExists;
            }
            else if (invite != null)
            {
                try
                {
                    this.Services.Email.SendInvitation(inviter, invite);
                    return new InvitePersonResult
                    {
                        Code = InvitePersonResult.ResultCode.Done,
                        Company = company,
                    };
                }
                catch (InvalidOperationException ex)
                {
                    return InvitePersonResult.SmtpError(ex);
                }
            }
            else
            {
                return InvitePersonResult.NoCompany;
            }
        }

        public Invited SelectById(int invitedId)
        {
            return this.InvitedRepository.Select()
                .ById(invitedId)
                .FirstOrDefault();
        }

        public Invited GetByInvitationKey(Guid invitationKey)
        {
            return this.InvitedRepository.Select()
                .ByInvitationKey(invitationKey)
                .FirstOrDefault();
        }

        public Invited SelectByIdAndEmail(int invitedId, string invitedEmail)
        {
            return this.InvitedRepository.Select()
               .ById(invitedId)
               .CheckEmail(invitedEmail)
               .FirstOrDefault();
        }

        private Dictionary<string, Company> companyDomains = new Dictionary<string, Company>();

        private Company GetCompanyByDomain(string hostname)
        {
            if (companyDomains.ContainsKey(hostname))
                return companyDomains[hostname];

            return companyDomains[hostname] = this.Services.Company.SelectByDomainName(hostname);
        }

        public Invited MarkAsRegistred(Guid code, int userId)
        {
            Invited invitation = this.Repo.Invited.GetByCode(code);
            invitation.UserId = userId;
            return this.Repo.Invited.Update(invitation);
        }

        public int CountForNewsletter()
        {
            return this.InvitedRepository.Select()
                .NotUnregistered()
                .Count();
        }

        public int CountForNoNewsletter()
        {
            return this.InvitedRepository.Select()
                .Unregistered()
                .Count();
        }

        public int CountPending()
        {
            return this.Repo.Invited.Select()
                .Where(i => i.Company.NetworkId == this.Services.NetworkId && i.UserId == null)
                .Count();
        }

        public IList<Invited> GetPending()
        {
            return this.Repo.Invited.Select()
                .Where(i => i.Company.NetworkId == this.Services.NetworkId && i.UserId == null)
                .OrderByDescending(i => i.Date)
                .ToList();
        }

        public ValidateInvitationCodeResult ValidateCode(Guid code)
        {
            var result = new ValidateInvitationCodeResult();

            var invitation = this.Repo.Invited.GetByCode(code, this.Services.NetworkId);
            if (invitation == null)
            {
                result.Errors.Add(ValidateInvitationCodeError.NoSuchCode, NetworksEnumMessages.ResourceManager);
                return result;
            }

            result.Invitation = new UserInvitationModel(invitation);

            var request = invitation.RegisterRequests.LastOrDefault();
            ////request = this.Repo.RegisterRequests.GetByInvitationId(invitation.Id);
            if (request != null)
            {
                result.Invitation.RegisterRequest = new RegisterRequestModel(request);
            }

            if (invitation.UserId != null)
            {
                result.Errors.Add(ValidateInvitationCodeError.AlreadyUsed, NetworksEnumMessages.ResourceManager);
                return result;
            }

            result.Succeed = result.IsValid = true;
            return result;
        }

        public DeleteInvitationResult Delete(DeleteInvitationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new DeleteInvitationResult(request);

            var item = this.InvitedRepository.GetById(request.InvitationId);
            var actingUser = this.Repo.People.GetById(request.ActingUserId);

            if (item == null)
                result.Errors.Add(DeleteInvitationError.NoSuchInvitation, NetworksEnumMessages.ResourceManager);
            if (actingUser == null)
                result.Errors.Add(DeleteInvitationError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            if (result.Errors.Count > 0)
                return result;

            var company = this.Repo.Companies.GetById(item.CompanyId);
            if (company.NetworkId == actingUser.NetworkId)
            {
                if (actingUser.NetworkAccess.HasAnyFlag(NetworkAccessLevel.SparkleStaff, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.ValidatePendingUsers))
                {
                    // ok
                }
                else if (actingUser.CompanyID == company.ID && actingUser.CompanyAccess == CompanyAccessLevel.Administrator)
                {
                    // ok
                }
                else
                {
                    result.Errors.Add(DeleteInvitationError.Unauthorized, NetworksEnumMessages.ResourceManager);
                }
            }
            else
            {
                if (actingUser.NetworkAccess.HasAnyFlag(NetworkAccessLevel.SparkleStaff))
                {
                    // ok
                }
                else
                {
                    result.Errors.Add(DeleteInvitationError.Unauthorized, NetworksEnumMessages.ResourceManager);
                }
            }

            if (item.UserId != null)
                result.Errors.Add(DeleteInvitationError.InvitationAlreadyUsed, NetworksEnumMessages.ResourceManager);

            if (result.Errors.Count > 0)
                return result;

            if (item.DeletedDateUtc == null)
            {
                item.DeletedByUserId = actingUser.Id;
                item.DeletedDateUtc = DateTime.UtcNow;
                this.Repo.Invited.Update(item);
            }

            result.Succeed = true;
            return result;
        }

        public IList<InvitedModel> GetByUserId(int userId, bool loadInviters)
        {
            var items = this.InvitedRepository.GetByUserId(userId);
            var models = items.Select(x => new InvitedModel(x)).ToList();

            if (loadInviters)
            {
                var userIds = models.Select(x => x.InvitedByUserId).ToArray();
                var users = this.Repo.People.GetUsersViewById(userIds);
                foreach (var item in models)
                {
                    if (users.ContainsKey(item.InvitedByUserId))
                    {
                        item.InvitedByUser = new UserModel(users[item.InvitedByUserId]);
                    }
                }
            }

            return models;
        }
    }
}
