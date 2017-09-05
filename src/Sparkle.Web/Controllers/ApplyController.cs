
namespace Sparkle.Controllers
{
    using Sparkle.Entities.Networks;
    using Sparkle.Filters;
    using Sparkle.Infrastructure;
    using Sparkle.LinkedInNET;
    using Sparkle.LinkedInNET.OAuth2;
    using Sparkle.Resources;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Tags;
    using Sparkle.Services.Networks.Users;
    using Sparkle.UI;
    using Sparkle.WebBase;
    using SrkToolkit.Domain;
    using SrkToolkit.Web;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// The registration process for all users is here!
    /// This is the Apply feature.
    /// </summary>
    [SubscriberAccess(RequiresActiveSubscription = false)]
    public class ApplyController : LocalSparkleController
    {
        ////private AuthorizationScope linkedInScope = ReadFullProfile | AuthorizationScope.ReadEmailAddress | AuthorizationScope.ReadContactInfo;
        internal static readonly AuthorizationScope linkedInScope = AuthorizationScope.ReadBasicProfile;

        private string applyInviterCode = null;

        private string ApplyInviterCode
        {
            get
            {
                if (string.IsNullOrEmpty(this.applyInviterCode))
                {
                    var cookie = this.Request.Cookies.Get("ApplyInviterCode");
                    if (cookie != null)
                    {
                        this.applyInviterCode = cookie.Value;
                    }
                }

                return this.applyInviterCode;
            }
            set
            {
                this.applyInviterCode = value;

                var cookie = this.Request.Cookies["ApplyInviterCode"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("ApplyInviterCode", this.applyInviterCode);
                    cookie.Expires = DateTime.UtcNow.AddHours(1D);
                    this.Response.Cookies.Add(cookie);
                }
                else
                {
                    cookie.Value = this.applyInviterCode;
                    cookie.Expires = DateTime.UtcNow.AddHours(1D);
                    this.Response.Cookies.Set(cookie);
                }
            }
        }

        private ActionResult RedirectOnWrongApplyState(ApplyRequestModel apply)
        {
            if (apply.Status == ApplyRequestStatus.Accepted)
            {
                this.TempData.AddInfo(Alerts.ApplyRequestAlreadyAccepted);
                return this.RedirectToAction("LogOn", "Account");
            }

            this.TempData.AddError(string.Format(Alerts.ApplyRequestDifferentState, apply.StatusTitle));
            return this.RedirectToAction("Details", new { id = apply.Key, });
        }

        private bool ValidateApplyRequestModelState(ApplyRequestRequest request)
        {
            int joinCompanyId;
            var isInt = int.TryParse(request.CompanyId, out joinCompanyId);
            if (!isInt || (isInt && joinCompanyId != 0) || !this.Services.AppConfiguration.Tree.Features.EnableCompanies)
            {
                foreach (var item in this.ModelState)
                {
                    if (item.Key.StartsWith("CreateCompanyRequest."))
                        continue;
                    if (item.Value.Errors.Count > 0)
                        return false;
                }

                return true;
            }
            else
            {
                return this.ModelState.IsValid;
            }
        }

        public ActionResult Index(Guid? Key, bool? FillWithLinkedIn, string CompanyCategory = null, string InviterCode = null)
        {
            if (this.User.Identity != null && this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("News", "Home");
            }

            var request = this.Services.People.GetApplyRequestRequest(Key, null, this.UserId, CompanyCategory);
            if (!Key.HasValue)
            {
                return this.RedirectToAction("Index", new { Key = request.Key, CompanyCategory = CompanyCategory, InviterCode = InviterCode, });
            }

            if (request.Model != null && request.Model.Status != ApplyRequestStatus.New)
            {
                return this.RedirectOnWrongApplyState(request.Model);
            }

            // verify inviter code
            if (!string.IsNullOrEmpty(InviterCode))
            {
                if (this.Services.People.IsApplyInviterCodeValid(InviterCode))
                {
                    this.ApplyInviterCode = InviterCode;
                }
                else
                {
                    this.TempData.AddError(Alerts.ApplyInviterCodeIsInvalid);
                }
            }

            if (FillWithLinkedIn.HasValue && FillWithLinkedIn.Value)
            {
                if (this.Services.AppConfiguration.Tree.Externals.LinkedIn.AllowLogon)
                {
                    var result = this.Services.People.AppendLinkedInProfileToApplyRequest(request, this.Request.UserHostAddress);
                    if (result.Succeed)
                    {
                        if (result.Errors.Any(o => o.Code == LinkedInPeopleError.CannotRetrieveProfilePicture))
                        {
                            this.TempData.AddError(result.Errors.Single(o => o.Code == LinkedInPeopleError.CannotRetrieveProfilePicture).DisplayMessage);
                        }

                        return this.RedirectToAction("Index", "Apply", new { Key = request.Key, CompanyCategory = CompanyCategory, InviterCode = InviterCode, });
                    }
                    else
                    {
                        var error = result.Errors.First();
                        this.Services.Logger.Error(
                            "/Apply/Index",
                            ErrorLevel.Business,
                            "ApplyController.Index: AppendDataModel failed ({0}: {1})",
                            error.Code.ToString(),
                            error.DisplayMessage);
                        this.TempData.AddError(error.DisplayMessage);
                    }
                }
                else
                {
                    this.TempData.AddError(T("L'inscription via LinkedIn n'est pas possible. "));
                }
            }

            request.InviterCode = this.ApplyInviterCode;

            // perfect open graph for sharing
            this.ViewBag.Description = this.Services.Network.About;

            return this.View(request);
        }

        [HttpPost]
        public ActionResult Index(ApplyRequestRequest request)
        {
            if (this.User.Identity != null && this.User.Identity.IsAuthenticated)
                return this.RedirectToAction("News", "Home");

            ApplyRequestModel applyModel = null;
            if ((applyModel = this.Services.People.GetApplyRequest(request.Key)) != null && applyModel.Status != ApplyRequestStatus.New)
                return this.RedirectOnWrongApplyState(applyModel);

            ApplyRequestResult result;
            request.UserRemoteAddress = this.Request.UserHostAddress;
            if (this.ValidateApplyRequestModelState(request) && !this.Request.IsAjaxRequest())
            {
                result = this.Services.People.SaveApplyRequest(request, true);

                if (this.ValidateResult(result, MessageDisplayMode.TempData))
                {
                    if (result.EmailErrorMessage != null)
                    {
                        this.TempData.AddError(result.EmailErrorMessage);
                    }

                    if (result.Submitted)
                    {
                        return this.RedirectToAction("Details", new { id = result.Model.Key, });
                    }
                }
            }
            else
            {
                result = this.Services.People.SaveApplyRequest(request, false);
            }

            this.Services.People.GetApplyRequestRequest(request.Key, request, this.UserId, null);

            if (this.Request.IsAjaxRequest())
            {
                if (result.Succeed)
                {
                    return this.ResultService.JsonSuccess(result);
                }
                else
                {
                    var error = result.Errors.First();
                    return this.ResultService.JsonError(error.Code.ToString(), error.DisplayMessage, result);
                }
            }
            else
            {
                this.ViewBag.ApplyRequest = applyModel;
                return this.View(request);
            }
        }

        [HttpOptions]
        public ActionResult ValidateEmailDomain()
        {
            return this.ResultService.NotFound();
        }

        [HttpPost]
        public ActionResult ValidateEmailDomain(string email, string search, Guid? key, string companyId)
        {
            var result = this.Services.Company.VerifyEmailDomainForApply(email, search, key);
            result.CompanyId = companyId;

            return this.ResultService.JsonSuccess(new
            {
                Model = result,
                Html = this.RenderPartialViewToString("~/Views/Ajax/ApplyCompany.cshtml", result),
                CanCreateCompany = result.DomainNameMatch == null,
                HasLinkedInCompanies = result.LinkedInCompanies != null && result.LinkedInCompanies.Count > 0,
            });
        }

        /// <param name="ReturnUrl">The URL of the apply request form.</param>
        public ActionResult LinkedInConnect(string ReturnUrl, Guid? Key)
        {
            ApplyRequestModel applyModel = null;
            if (Key != null && (applyModel = this.Services.People.GetApplyRequest(Key.Value)) != null && applyModel.Status != ApplyRequestStatus.New)
                return this.RedirectOnWrongApplyState(applyModel);

            var returnUrl = UrlTools.Compose(this.Request) + "/Apply/LinkedInConnectReturn?ReturnUrl=" + Uri.EscapeDataString(ReturnUrl);
            var failReturnUrl = UrlTools.Compose(this.Request) + "/Apply/LinkedInConnectReturn?error=IdNotFound&errorDesc=The+redirect+id+was+not+found";

            if (Key != null)
            {
                returnUrl += "&Key=" + Uri.EscapeDataString(Key.Value.ToString());
                failReturnUrl += "&Key=" + Uri.EscapeDataString(Key.Value.ToString());
            }

            // Init LinkedIn
            string goUrl;
            bool success;
            var initResult = this.InitLinkedIn(this.Services, ReturnUrl, returnUrl, (int)linkedInScope, out goUrl, out success);
            if (!success)
                return initResult;

            goUrl += "&failReturnUrl=" + Uri.EscapeDataString(failReturnUrl);
            Trace.WriteLine("ApplyController.LinkedInConnect redirects user to: " + goUrl);
            return this.Redirect(goUrl);
        }

        public ActionResult LinkedInConnectReturn(string code, Guid? redirId, string error, string errorDesc, string ReturnUrl, string li_error_message, string li_error, Guid? Key)
        {
            if (li_error == "invalid_scope")
            {
                this.TempData.AddError(string.Format(Alerts.LinkedInConnectReturnInvalidScope, error, errorDesc));
            }

            if (string.IsNullOrEmpty(ReturnUrl) && Key.HasValue)
            {
                ReturnUrl = Url.Action("", "Apply", new { Key = Key.Value.ToString(), });
            }

            AuthorizationAccessToken token;
            bool success;
            var result = this.GetLinkedInToken(this.Services, code, redirId, li_error, li_error_message, ReturnUrl, out token, out success);
            if (!success)
                return result;

            // get applyrequest.key from ReturnUrl
            // "/Apply?Key=xxxx-xxx-xxx-xxxx"
            Guid key;
            if (Key != null)
            {
                key = Key.Value;
            }
            else
            {
                var stringKey = ReturnUrl.Split(new char[] { '?', '&', })
                    .Skip(1)
                    .Select(p => p.Split(new char[] { '=', }, 2))
                    .Where(p => p.Length == 2 && p[0] == "Key")
                    .Select(p => p[1])
                    .SingleOrDefault();
                if (stringKey == null || !Guid.TryParse(stringKey, out key))
                {
                    this.Logger.Error(
                        "/Apply/LinkedInConnectReturn",
                        ErrorLevel.Input,
                        "Missing Key from ReturnUrl '" + ReturnUrl + "' error='{0}' errorDesc='{1}' li_error='{2}' li_error_message='{3}'",
                        error, errorDesc,
                        li_error, li_error_message);
                    this.TempData.AddError(string.Format(Alerts.LinkedInConnectReturnError, error, errorDesc));
                    return this.RedirectToLocal(ReturnUrl);
                }
            }

            var request = this.Services.People.GetApplyRequestRequest(key, null, null, null);
            this.Services.People.AppendSocialNetworkConnection(request, SocialNetworkConnectionType.LinkedIn, string.Empty, token.AccessToken, string.Empty, true, token.AuthorizationDateUtc, token.ExpiresIn / 60);

            this.TempData.AddInfo(Alerts.LinkedInConnectSuccess);
            var goUrl = this.Url.SetQueryString(ReturnUrl, "FillWithLinkedIn", "True");
            return this.RedirectToLocal(goUrl);
        }

        public ActionResult Details(Guid? id, bool SendEmailAgain = false, bool EmailSent = false)
        {
            ApplyRequestModel model = null;
            if (!id.HasValue || (model = this.Services.People.GetApplyRequest(id.Value)) == null)
                return this.ResultService.NotFound();

            this.ViewBag.EmailSent = EmailSent;
            this.ViewBag.SendAgainRequest = false;
            if (this.User.Identity != null && this.User.Identity.IsAuthenticated)
                this.ViewBag.SendAgainRequest = true;

            if (model.IsPendingEmailConfirmation && SendEmailAgain)
            {
                var requester = new UserModel(model.UserDataModel.User);
                try
                {
                    this.Services.Email.SendApplyRequestConfirmation(model, requester);
                    this.TempData.AddConfirmation(Alerts.ApplyRequest_EmailConfirmSent);
                    return this.RedirectToAction("Details", new { id = id, EmailSent = true, });
                }
                catch (SparkleServicesException ex)
                {
                    this.TempData.AddError(ex.DisplayMessage ?? NetworksLabels.EmailProviderDefaultDisplayError);
                    return this.RedirectToAction("Details", new { id = id, EmailSent = false, });
                }
                catch (InvalidOperationException ex)
                {
                    this.TempData.AddError(NetworksLabels.EmailProviderDefaultDisplayError);
                    return this.RedirectToAction("Details", new { id = id, EmailSent = false, });
                }
            }

            if (model.IsAccepted)
            {
                return this.Redirect(this.Services.People.GetApplyRequestJoinUrl(model));
            }

            return this.View(model);
        }

        public ActionResult GetApplyCurrentStatus(Guid? id)
        {
            if (!this.Request.IsAjaxRequest())
                return this.ResultService.NotFound();

            ApplyRequestModel model = null;
            if (!id.HasValue || (model = this.Services.People.GetApplyRequest(id.Value)) == null)
                return this.ResultService.JsonError();

            return this.ResultService.JsonSuccess(new { Status = model.Status.ToString() });
        }

        public ActionResult Confirm(Guid? id, string Secret)
        {
            ApplyRequestModel applyModel = null;
            if (!id.HasValue || (applyModel = this.Services.People.GetApplyRequest(id.Value)) == null)
                return this.ResultService.NotFound();
            if (applyModel.Status != ApplyRequestStatus.PendingEmailConfirmation)
                return this.RedirectOnWrongApplyState(applyModel);

            var request = new ConfirmApplyRequestEmailAddressRequest
            {
                Key = id.Value,
                Secret = Secret,
            };

            var result = this.Services.People.ConfirmApplyRequestEmailAddress(request);
            if (this.ValidateResult(result, MessageDisplayMode.TempData))
            {
                if (result.EmailWasAlreadyConfirmed)
                {
                    this.TempData.AddInfo(Alerts.ApplyRequest_RequestIsPendingAccept);
                }
                else if (result.EmailDomainMatch)
                {
                    this.TempData.AddInfo(Alerts.ApplyRequest_CreatedOnConfirm);
                    return this.Redirect(this.Services.People.GetApplyRequestJoinUrl(id.Value));
                }
                else
                {
                    this.TempData.AddInfo(Alerts.ApplyRequest_EmailAddressConfirmed);
                }
                return this.RedirectToAction("Details", new { id = id, });
            }
            else if (result.Errors.ContainsError(ConfirmApplyRequestEmailAddressError.NoSuchApplyRequest))
            {
                var error = result.Errors.First(e => e.Code == ConfirmApplyRequestEmailAddressError.NoSuchApplyRequest);
                return this.ResultService.NotFound(error.DisplayMessage);
            }
            else if (result.Errors.ContainsError(ConfirmApplyRequestEmailAddressError.NotSubmitted))
            {
                var error = result.Errors.First(e => e.Code == ConfirmApplyRequestEmailAddressError.NotSubmitted);
                this.TempData.AddError(error.DisplayMessage);
                return this.RedirectToAction("Index", new { Key = id, });
            }
            else
            {
                return this.View(result);
            }
        }

        public ActionResult Join(Guid? id, string Secret)
        {
            ApplyRequestModel model = null;
            if (!id.HasValue || (model = this.Services.People.GetApplyRequest(id.Value)) == null)
                return this.ResultService.NotFound();
            if (model.Status != ApplyRequestStatus.Accepted)
                return this.RedirectOnWrongApplyState(model);

            var result = this.Services.People.ValidateApplyRequestToJoin(id.Value, Secret);
            if (this.ValidateResult(result, MessageDisplayMode.TempData))
            {
                if (result.GoUrl != null)
                {
                    return this.Redirect(result.GoUrl);
                }
                else
                {
                    return this.RedirectToAction("News", "Home");
                }
            }

            return this.View(result);
        }

        ////public ActionResult ManageTags(AddOrRemoveTagRequest model)
        ////{
        ////    if (!this.Request.IsAjaxRequest() || model == null)
        ////        return this.ResultService.NotFound();

        ////    var result = this.Services.Tags.AddOrRemoveCompanyTagInApplyRequest(model);
        ////    if (!this.ValidateResult(result, MessageDisplayMode.None))
        ////    {
        ////        var error = result.Errors.First();
        ////        return this.ResultService.JsonError(error.Code.ToString(), error.DisplayMessage);
        ////    }

        ////    return this.ResultService.JsonSuccess(result.AddedTag);
        ////}

        public ActionResult GetCompanyTags(Guid key, string categoryAlias)
        {
            if (!this.Request.IsAjaxRequest())
                return this.ResultService.NotFound();
            if (key == Guid.Empty || string.IsNullOrEmpty(categoryAlias))
                return this.ResultService.JsonError("EmptyArguments", Alerts.EmptyArguments);

            var applyRequest = this.Services.People.GetApplyRequest(key);
            if (applyRequest == null)
                return this.ResultService.JsonError("NoSuchApplyRequest", Alerts.NoSuchApplyRequest);

            var category = this.Services.Tags.GetCategoryByAlias(categoryAlias);
            if (categoryAlias == null)
                return this.ResultService.JsonError("NoSuchTagCategory", Alerts.NoSuchTagCategory);

            var tags = this.Services.Tags.GetCompanyTagsInApplyRequest(applyRequest.Key, category.Id);

            return this.ResultService.JsonSuccess(tags.OrderBy(o => o.Name).ToList());
        }

        public ActionResult Closed()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult SetSingleProfileField(SetSingleProfileFieldRequest request)
        {
            request.UserRemoteAddress = this.Request.UserHostAddress;
            var result = this.Services.People.SetSingleProfileFieldOnApply(request);
            return this.ResultService.JsonSuccess(result);
        }
    }
}
