
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
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.EmailModels;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Users;
    using SrkToolkit.Common.Validation;
    using SrkToolkit.Domain;
    
    /// <summary>
    /// OBSOLETE.
    /// </summary>
    public class RegisterRequestsService : ServiceBase, IRegisterRequestsService
    {
        internal RegisterRequestsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public EmitRegisterRequestResult EmitRegisterRequest(EmailAddress emailAddress, int companyId)
        {
            if (emailAddress == null)
                throw new ArgumentNullException("emailAddress");

            var result = new EmitRegisterRequestResult
            {
                EmailAddress = emailAddress,
            };

            if (this.Services.People.IsEmailAddressInUse(emailAddress.Value))
            {
                result.Code = EmitRegisterRequestCode.EmailAddressAlreadyInUse;
                return result;
            }

            var company = this.Services.Company.GetById(companyId);
            if (company == null)
            {
                result.Code = EmitRegisterRequestCode.NoSuchCompany;
                return result;
            }

            var match = this.Repo.RegisterRequests
                .GetByEmailAddress(emailAddress.AccountPart, emailAddress.TagPart, emailAddress.DomainPart, this.Services.NetworkId);

            if (match != null)
            {
                if (match.Code == null)
                {
                    match.Code = Guid.NewGuid();
                    this.Repo.RegisterRequests.Update(match);
                }

                result.Entity = match;
                result.Code = EmitRegisterRequestCode.RequestExists;
            }
            else
            {
                result.Code = EmitRegisterRequestCode.RequestEmitted;
                var entity = new RegisterRequest
                {
                    CompanyId = company.ID,
                    DateCreatedUtc = DateTime.UtcNow,
                    DateUpdatedUtc = DateTime.UtcNow,
                    EmailDomain = emailAddress.DomainPart,
                    EmailTagPart = emailAddress.TagPart,
                    EmailAccountPart = emailAddress.AccountPart,
                    Status = 0,
                    Code = Guid.NewGuid(),
                };

                this.SetNetwork(entity);

                entity = this.Repo.RegisterRequests.Insert(entity);
                result.Entity = entity = this.Repo.RegisterRequests.GetById(entity.Id);

                // find recipients
                bool companyHasNoAdmins = false;
                var recipients = new List<SimpleContact>();
                var companyAdmins = this.Repo.Companies.GetAdministrators(company.ID, this.Services.NetworkId);
                if (this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnabled && this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced)
                {
                    var subscribedUserIds = this.Repo.Subscriptions.GetUserIdsSubscribedAmongIds(
                        this.Services.NetworkId,
                        companyAdmins.Select(o => o.Id).ToArray(),
                        DateTime.UtcNow);

                    companyAdmins = companyAdmins.Where(o => subscribedUserIds.Contains(o.Id)).ToList();
                }

                if (companyAdmins.Count > 0)
                {
                    recipients.AddRange(companyAdmins.Select(u => new SimpleContact(u)));
                }
                else
                {
                    companyHasNoAdmins = true;
                    var networkAdmins = this.Services.People.GetByNetworkAccessLevel(NetworkAccessLevel.NetworkAdmin);
                    if (networkAdmins.Count > 0)
                    {
                        recipients.AddRange(networkAdmins.Select(u => new SimpleContact(u)));
                    }
                    else
                    {
                        var sparkleStaff = this.Services.People.GetByNetworkAccessLevel(NetworkAccessLevel.SparkleStaff);
                        if (sparkleStaff.Count > 0)
                        {
                            recipients.AddRange(sparkleStaff.Select(u => new SimpleContact(u)));
                        }
                    }
                }

                var pendingRequests = this.Services.RegisterRequests.CountPendingByCompany(companyId);

                try
                {
                    var model = new RegisterRequestEmailModel(emailAddress.Value, Sparkle.UI.Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                    {
                        Company = new CompanyModel(company),
                        Request = new RegisterRequestModel(entity),
                        CompanyHasNoAdmin = companyHasNoAdmins,
                        OtherRequestCount = pendingRequests - 1,
                    };
                    this.Services.Email.SendRegisterRequestConfirmation(model);
                }
                catch (Exception ex)
                {
                    // this is already logged by the emailservice
                    ////this.Services.Logger.Error("InvitedService.EmitRegisterRequest", ErrorLevel.ThirdParty, ex);
                } 

                foreach (var recipient in recipients)
                {
                    try
                    {
                        var model = new RegisterRequestEmailModel(recipient, Sparkle.UI.Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                        {
                            Company = new CompanyModel(company),
                            Request = new RegisterRequestModel(entity),
                            CompanyHasNoAdmin = companyHasNoAdmins,
                            OtherRequestCount = pendingRequests - 1,
                        };
                        this.Services.Email.SendRegisterRequest(model);
                    }
                    catch (Exception ex)
                    {
                        // this is already logged by the emailservice
                        ////this.Services.Logger.Error("InvitedService.EmitRegisterRequest", ErrorLevel.ThirdParty, ex);
                    } 
                }
            }

            return result;
        }

        public IList<RegisterRequest> GetPendingRegisterRequests(RegisterRequestOptions options)
        {
            return this.Repo.RegisterRequests
                .GetPendingRequests(this.Services.NetworkId, options);
        }

        public RegisterRequest GetRegisterRequestById(int id)
        {
            return this.Repo.RegisterRequests.GetById(id, this.Services.NetworkId);
        }

        public bool CanAccept(RegisterRequest request)
        {
            switch (request.StatusCode)
            {
                case RegisterRequestStatus.New:
                case RegisterRequestStatus.ExternalCommunication:
                    return true;

                case RegisterRequestStatus.Refused:
                case RegisterRequestStatus.Accepted:
                default:
                    return false;
            }
        }

        public IList<RegisterRequest> GetRegisterRequests(RegisterRequestStatus status)
        {
            return this.Repo.RegisterRequests.GetByStatus(status, this.Services.NetworkId);
        }

        /// <summary>
        /// Marks the request accepted by the specified user.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public RegisterRequest MarkAccepted(RegisterRequest item, int userId, int? invitedId)
        {
            this.VerifyNetwork(item);

            item.StatusCode = RegisterRequestStatus.Accepted;
            item.ValidatedByUserId = userId;
            return this.Repo.RegisterRequests.Update(item);
        }

        /// <summary>
        /// Marks the request as being handled by email by the specified user.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public RegisterRequest MarkInCommunication(RegisterRequest item, int userId)
        {
            this.VerifyNetwork(item);

            item.StatusCode = RegisterRequestStatus.ExternalCommunication;
            item.ReplyUserId = userId;
            return this.Repo.RegisterRequests.Update(item);
        }

        /// <summary>
        /// Marks the request denied by the specified user.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public RegisterRequest MarkDenied(RegisterRequest item, int userId)
        {
            this.VerifyNetwork(item);

            item.StatusCode = RegisterRequestStatus.Refused;
            item.ValidatedByUserId = userId;
            return this.Repo.RegisterRequests.Update(item);
        }

        public int CountPending()
        {
            return this.Repo.RegisterRequests.CountPendingRequests(this.Services.NetworkId);
        }

        public int CountPendingByCompany(int companyId)
        {
            return this.Repo.RegisterRequests.CountPendingRequestsByCompany(companyId);
        }

        public RegisterRequestModel GetByCode(Guid id)
        {
            var item = this.Repo.RegisterRequests.GetByCode(id, this.Services.NetworkId);
            if (item != null)
                return new RegisterRequestModel(item);
            return null;
        }

        public RegisterRequestModel GetByCode(Guid id, RegisterRequestOptions options)
        {
            var item = this.Repo.RegisterRequests.GetByCode(id, this.Services.NetworkId, options);
            if (item != null)
                return new RegisterRequestModel(item);
            return null;
        }

        public IList<RegisterRequestModel> GetAllByCompany(int companyId)
        {
            return this.Repo.RegisterRequests.GetAllByCompany(companyId, this.Services.NetworkId)
                .Select(r => new RegisterRequestModel(r))
                .ToList();
        }

        public IList<UpdateRegisterRequestResult> Update(IList<UpdateRegisterRequestRequest> requests, int invitingUserId, int companyId)
        {
            if (requests == null)
                throw new ArgumentNullException("requests");

            var inviter = this.Repo.People.GetById(invitingUserId);

            var company = this.Services.Company.GetById(companyId);

            if (!inviter.NetworkAccess.HasAnyFlag(NetworkAccessLevel.SparkleStaff, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.ManageRegisterRequests)
             && inviter.CompanyAccess != CompanyAccessLevel.Administrator)
            {
                var errorResult = new UpdateRegisterRequestResult(null);
                errorResult.Errors.Add(UpdateRegisterRequestCode.Unauthorized, NetworksEnumMessages.ResourceManager);
                return new List<UpdateRegisterRequestResult>()
                {
                    errorResult,
                };
            }

            var results = new List<UpdateRegisterRequestResult>(requests.Count);
            foreach (var request in requests)
            {
                var result = new UpdateRegisterRequestResult(request);
                results.Add(result);
                var item = this.Repo.RegisterRequests.GetById(request.Id);
                if (item == null)
                {
                    result.Errors.Add(UpdateRegisterRequestCode.NoSuchRequest, NetworksEnumMessages.ResourceManager);
                    continue;
                }

                result.ItemBefore = new RegisterRequestModel(item);

                if (request.NewStatus == RegisterRequestStatus.Accepted)
                {
                    if (item.StatusCode == RegisterRequestStatus.New || item.StatusCode == RegisterRequestStatus.ExternalCommunication)
                    {
                        if (item.CompanyId != null && item.CompanyId.Value != companyId)
                            throw new InvalidOperationException("RegisterRequest " + item.Id + " for company " + item.CompanyId + " cannot be accepted into company " + companyId);

                        result.InviteResult = this.Services.Invited.Invite(inviter, item.EmailAddress, companyId);
                        if (result.InviteResult.Code == InvitePersonResult.ResultCode.Done)
                        {
                            item.AcceptedInvitationId = result.InviteResult.Invitation.Id;
                            item.StatusCode = RegisterRequestStatus.Accepted;
                            this.Repo.RegisterRequests.Update(item);
                            result.Succeed = true;
                        }
                        else
                        {
                            result.Errors.Add(UpdateRegisterRequestCode.InviteError, NetworksEnumMessages.ResourceManager);
                        }
                    }
                    else
                    {
                        result.Errors.Add(UpdateRegisterRequestCode.RequestAlreadyHandled, NetworksEnumMessages.ResourceManager);
                    }
                }
                else if (request.NewStatus == RegisterRequestStatus.Refused)
                {
                    if (item.StatusCode == RegisterRequestStatus.New || item.StatusCode == RegisterRequestStatus.ExternalCommunication)
                    {
                        var model = new RegisterRequestEmailModel(item.EmailAddress, Sparkle.UI.Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                        {
                            Company = new CompanyModel(company),
                            Request = new RegisterRequestModel(item),
                        };
                        item.StatusCode = RegisterRequestStatus.Refused;
                        this.Repo.RegisterRequests.Update(item);
                        result.Succeed = true;
                        try
                        {
                            this.Services.Email.SendRegisterRequestDenied(model);
                        }
                        catch (Exception)
                        {
                            // ignore send error
                        }
                    }
                    else
                    {
                        result.Errors.Add(UpdateRegisterRequestCode.RequestAlreadyHandled, NetworksEnumMessages.ResourceManager);
                    }
                }
                else
                {
                    result.Errors.Add(UpdateRegisterRequestCode.InvalidNewStatus, NetworksEnumMessages.ResourceManager);
                }

                result.ItemAfter = new RegisterRequestModel(item);
            }

            return results;
        }

        public int? GetCompanyIdFromRequestsIds(int[] registerRequestsIds)
        {
            int? companyId = null;
            foreach (var id in registerRequestsIds)
            {
                var request = this.GetRegisterRequestById(id);
                if (!companyId.HasValue)
                    companyId = request.CompanyId;
                else if (companyId.Value != request.CompanyId)
                    return null;
            }

            return companyId;
        }

        public RegisterRequestModel GetById(int id)
        {
            var item = this.Repo.RegisterRequests.GetById(id);
            if (item == null)
                return null;

            return new RegisterRequestModel(item)
            {
                RepliedByProfileUrl = item.RepliedBy != null ? this.Services.People.GetProfileUrl(item.RepliedBy, UriKind.Relative) : null,
                ValidatedByProfileUrl = item.RepliedBy != null ? this.Services.People.GetProfileUrl(item.ValidatedBy, UriKind.Relative) : null,
            };
        }
    }
}
