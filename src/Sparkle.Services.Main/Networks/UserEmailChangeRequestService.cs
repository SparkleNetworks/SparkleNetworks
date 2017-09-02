
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Networks.Filters;
    using Sparkle.Services.Networks.Users;
    using Sparkle.Services.Networks.Lang;
    using SrkToolkit.Domain;
    using SrkToolkit.Common.Validation;
    using Sparkle.Services.Authentication;

    public class UserEmailChangeRequestService : ServiceBase, IUserEmailChangeRequestService
    {
        [DebuggerStepThrough]
        internal UserEmailChangeRequestService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public IList<UserEmailChangeRequest> SelectByUserId(int userId)
        {
            return this.Repo.UserEmailChangeRequest
                .NewQuery(UserEmailChangeRequestOptions.User | UserEmailChangeRequestOptions.CreatedByUser)
                .WithUserId(userId)
                .ToList();
        }

        public UserEmailChangeRequest SelectPendingRequestFromUserId(int userId)
        {
            return this.Repo.UserEmailChangeRequest
                .NewQuery(UserEmailChangeRequestOptions.User | UserEmailChangeRequestOptions.CreatedByUser)
                .WithUserId(userId)
                .PendingRequest()
                .SingleOrDefault();
        }

        public void CancelPendingRequest(UserEmailChangeRequest item)
        {
            item.StatusValue = UserEmailChangeRequestStatus.Canceled;
            this.Repo.UserEmailChangeRequest.Update(item);
        }

        public AdminProceduresRequest GetAdminProceduresRequestFromLogin(string login)
        {
            if (string.IsNullOrEmpty(login))
                throw new ArgumentException("The value cannot be empty", "login");

            var model = new AdminProceduresRequest();

            var user = this.Services.People.SelectWithLogin(login);
            if (user != null && user.NetworkAccess != NetworkAccessLevel.Disabled)
            {
                // Change email procedure
                var emailChangeRequest = this.Services.UserEmailChangeRequest.SelectPendingRequestFromUserId(user.Id);
                if (emailChangeRequest != null)
                {
                    model.HasAlreadyPending = true;
                    model.PendingEmail = new EmailAddress(emailChangeRequest.NewEmailAccountPart, emailChangeRequest.NewEmailTagPart, emailChangeRequest.NewEmailDomainPart).Value;
                    model.PendingRemark = emailChangeRequest.EmailChangeRemark;
                }

                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Login = user.Login;
                model.ActualEmail = user.Email;
                model.ActualCompany = user.Company.Name;
            }

            return model;
        }

        public AdminProceduresResult AddUserEmailChangeRequest(AdminProceduresRequest request, int currentUserId)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new AdminProceduresResult(request);

            var user = this.Services.People.SelectWithLogin(request.Login);
            if (user == null)
            {
                result.Errors.Add(AdminProceduresError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return result;
            }

            if (!this.Services.People.IsActive(user))
            {
                result.Errors.Add(AdminProceduresError.UserIsInactive, NetworksEnumMessages.ResourceManager);
                return result;
            }

            var userEmailAddress = new EmailAddress(user.Email);
            var newEmailAddress = new EmailAddress(request.Email);

            var isFreeEmail = this.IsEmailAvailable(newEmailAddress, user.Id);
            if (!isFreeEmail)
            {
                result.Errors.Add(AdminProceduresError.ForbiddenEmail, NetworksEnumMessages.ResourceManager);
                return result;
            }

            var pending = this.Services.UserEmailChangeRequest.SelectPendingRequestFromUserId(user.Id);
            if (pending != null)
            {
                if (request.OverwritePending)
                {
                    this.Services.UserEmailChangeRequest.CancelPendingRequest(pending);
                }
                else
                {
                    result.Errors.Add(AdminProceduresError.AlreadyPending, NetworksEnumMessages.ResourceManager, user.FirstName, user.LastName);
                    return result;
                }
            } 
            
            var item = new UserEmailChangeRequest
            {
                NetworkId = this.Services.NetworkId,
                PreviousEmailAccountPart = userEmailAddress.AccountPart,
                PreviousEmailTagPart = userEmailAddress.TagPart,
                PreviousEmailDomainPart = userEmailAddress.DomainPart,
                NewEmailAccountPart = newEmailAddress.AccountPart,
                NewEmailTagPart = newEmailAddress.TagPart,
                NewEmailDomainPart = newEmailAddress.DomainPart,
                UserId = user.Id,
                CreatedByUserId = currentUserId,
                Status = (int)UserEmailChangeRequestStatus.Pending,
                PreviousEmailForbidden = request.ForbidNewEmail ? 1 : 0,
                EmailChangeRemark = request.Remark,
                CreateDateUtc = DateTime.UtcNow,
            };

            this.Repo.UserEmailChangeRequest.Insert(item);

            ////this.Services.Email.SendEmailChangeRequest(user, email, confirmationLink); // done somewhere else

            result.Succeed = true;
            return result;
        }

        public bool IsEmailAvailable(EmailAddress emailAddress, int userId)
        {
            var isInUse = this.Services.People.IsEmailAddressInUse(emailAddress.AccountPart, emailAddress.DomainPart);
            var isForbiddenOrPending = this.Services.UserEmailChangeRequest.IsEmailForbiddenOrPending(emailAddress, userId);

            if (!isInUse && !isForbiddenOrPending)
                return true;
            return false;
        }

        public bool IsEmailForbiddenOrPending(EmailAddress emailAddress, int userId)
        {
            var isPending = this.Repo.UserEmailChangeRequest
                .Select()
                .WithPendingEmail(emailAddress.AccountPart, emailAddress.DomainPart)
                .Where(o => o.UserId != userId)
                .Count() > 0;

            var isForbidden = this.Repo.UserEmailChangeRequest
                .Select()
                .WithForbiddenEmail(emailAddress.AccountPart, emailAddress.DomainPart)
                .Count() > 0;

            if (isPending || isForbidden)
                return true;
            return false;
        }

        public UserEmailChangeRequest SelectById(int id)
        {
            return this.Repo.UserEmailChangeRequest
                .Select()
                .Where(o => o.Id == id)
                .SingleOrDefault();
        }

        public void ValidatePendingRequest(UserEmailChangeRequest pending)
        {
            if (pending.StatusValue != UserEmailChangeRequestStatus.Pending)
                throw new ArgumentException("You are trying to validate a non-pending request", "pending");

            pending.StatusValue = UserEmailChangeRequestStatus.Succeed;
            pending.ValidateDateUtc = DateTime.UtcNow;

            var user = this.Services.People.SelectWithId(pending.UserId);
            var email = new EmailAddress(pending.NewEmailAccountPart, pending.NewEmailTagPart, pending.NewEmailDomainPart);
            user.Email = email.Value;

            // TODO: do the following operations in a SQL transaction please

            // update dbo.Users
            this.Repo.People.Update(user);

            // update dbo.aspnet_Membership
            this.Services.MembershipService.UpdateUser(user.UserId, email.Value);

            // update request
            this.Repo.UserEmailChangeRequest.Update(pending);
        }

        public IList<UserEmailChangeRequest> GetPendingRequests()
        {
            return this.Repo.UserEmailChangeRequest
                .NewQuery(UserEmailChangeRequestOptions.User | UserEmailChangeRequestOptions.CreatedByUser)
                .PendingRequest()
                .ToList();
        }

        public UserEmailChangeRequestModel GetById(int id)
        {
            var item = this.Repo.UserEmailChangeRequest
                .GetById(id, UserEmailChangeRequestOptions.User | UserEmailChangeRequestOptions.CreatedByUser);
            if (item == null)
                return null;

            return new UserEmailChangeRequestModel(item)
            {
                UserProfileUrl = item.User != null ? this.Services.People.GetProfileUrl(item.User, UriKind.Relative) : null,
                CreatedByUserProfileUrl = item.CreatedByUser != null ? this.Services.People.GetProfileUrl(item.CreatedByUser, UriKind.Relative) : null,
            };
        }

        public IList<UserEmailChangeRequestModel> GetAll()
        {
            var items = this.Repo.UserEmailChangeRequest.GetAll(UserEmailChangeRequestOptions.User | UserEmailChangeRequestOptions.CreatedByUser);
            return items.Select(x => new UserEmailChangeRequestModel(x)).ToList();
        }
    }
}
