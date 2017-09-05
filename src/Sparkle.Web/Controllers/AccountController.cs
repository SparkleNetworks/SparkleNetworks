
namespace Sparkle.Controllers
{
    using LinkedInNET;
    using Sparkle.Common;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Entities.Networks;
    using Sparkle.Filters;
    using Sparkle.Helpers;
    using Sparkle.Infrastructure;
    using Sparkle.Infrastructure.Crypto;
    using Sparkle.LinkedInNET.OAuth2;
    using Sparkle.Models;
    using Sparkle.Models.Account;
    using Sparkle.Resources;
    using Sparkle.Services;
    using Sparkle.Services.Authentication;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Users;
    using Sparkle.UI;
    using Sparkle.WebBase;
    using SrkToolkit.Common.Validation;
    using SrkToolkit.Web;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using Thought.vCards;
    using Twitterizer;
    using UserModel = Sparkle.Services.Networks.Models.UserModel;

    /*
     * Here is a diagram of the OLD registration process.
     * Everything described here now redirects the user the the /Apply process.
     * 
     *                    REGISTRATION PROCESS
     *                    ________|__________
     *                   |                   |
     *                   | Companies.Enabled |
     *                 / |___________________| \
     *            YES /                         \ NO
     *               /                           \
     *    -------------------                  -----------
     *    | RegisterRequest |                  | Join    |
     *    -------------------                  -----------
     *          |    (goes in RegisterRequests    |    (goes in Users table
     *          |     table)                      |     With IsEmailConfirmed=0)
     *    -------------------                  -----------
     *    | Register        |                  | Confirm |
     *    -------------------                  -----------
     *          |    (User line created)          |   
     *          |                                 |
     *          |________                   ______|
     *                 __\_________________/__
     *                 |                     |
     *                 | Requires validation |
     *                 |_____________________|
     *                   /                 \
     *              YES /                   \ NO
     *                 /                     \
     *            Redirect home           QuickStart
     *            with info message
     *            
     * 
     */

    [SubscriberAccess(RequiresActiveSubscription = false)]
    public class AccountController : LocalSparkleController
    {
        internal static readonly AuthorizationScope linkedInScope = AuthorizationScope.ReadBasicProfile;
        const string RememberMeChoiceCookieName = "SpkAuthRemember";

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService
        {
            get { return this.Services.MembershipService; }
        }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null)
                FormsService = new FormsAuthenticationService();

            base.Initialize(requestContext);

            this.Services.People.OptionsList = new List<string> { "Job", "Company" };
        }

        public ActionResult GetPath()
        {
            string r = Request.QueryString["ReturnUrl"];
            if (r == "/")
            {
                return RedirectToRoute("Welcome");
            }
            return RedirectToAction("LogOn", new { ReturnUrl = r });
        }

        public ActionResult Index()
        {
            if (this.UserId == null)
                return this.RedirectToAction("LogOn");

            var mbsUser = this.MembershipService.GetUser(this.SessionService.User.Login);
            this.ViewBag.MbsUser = mbsUser;

            var states = this.Services.SocialNetworkStates.GetAllIncludingUnconfigured();
            this.ViewBag.IsSocialAvailable = states != null && states.Any(s =>
                (s.Entity != null && s.Entity.IsConfigured)
                && ((s.Type == SocialNetworkConnectionType.Twitter && this.Services.AppConfiguration.Tree.Features.Users.SocialPull.IsEnabled)
                    || (s.Type != SocialNetworkConnectionType.Twitter && s.Type != SocialNetworkConnectionType.LinkedIn)));

            return this.View();
        }

        public ActionResult LogIn()
        {
            return this.RedirectToActionPermanent("LogOn");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pc">indicates the password was just changed</param>
        /// <returns></returns>
        public ActionResult LogOn(string pc, string ReturnUrl)
        {
            // already logged
            if (this.User.Identity != null && this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("News", "Home");
            }

            var model = new LogOnModel
            {
                ReturnUrl = ReturnUrl,
            };
            if (!string.IsNullOrEmpty(pc))
            {
                model.PasswordChanged = pc == "1";
            }

            string username = this.Request.QueryString["username"];
            if (username != null)
            {
                model.UserName = username;
            }

            {
                // read the cookie that remembers the "remember me" choice for future logins
                // when you often change account, it is boring to uncheck the option every time
                var rememberMeCookie = this.Request.Cookies[RememberMeChoiceCookieName];
                if (rememberMeCookie != null)
                {
                    bool rememberMeCookieValue;
                    if (rememberMeCookie.Value != null && bool.TryParse(rememberMeCookie.Value, out rememberMeCookieValue))
                    {
                        model.RememberMe = rememberMeCookieValue;
                    }
                }
            }

            return this.View(model);
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model)
        {
            // chuck norris (403 forbidden) any authenticated user
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("News", "Home");
            }

            {
                // save a cookie that remembers the "remember me" choice for future logins
                // when you often change account, it is boring to uncheck the option every time
                var rememberMeCookie = this.Request.Cookies[RememberMeChoiceCookieName];
                if (rememberMeCookie != null)
                {
                    rememberMeCookie.Values.Clear();
                    rememberMeCookie.Value = model.RememberMe.ToString();
                    this.Response.SetCookie(rememberMeCookie);
                }
                else
                {
                    rememberMeCookie = new HttpCookie(RememberMeChoiceCookieName, model.RememberMe.ToString());
                    rememberMeCookie.Expires = DateTime.UtcNow.AddYears(1);
                    this.Response.SetCookie(rememberMeCookie);
                }
            }

            if (this.ModelState.IsValid)
            {
                // this is the definite membership user
                Sparkle.Services.Authentication.MembershipUser mbsUser = null;
                IList<LogOnError> logError = new List<LogOnError>();


                // Gets last login date
                mbsUser = this.MembershipService.GetUser(model.UserName);
                if (mbsUser == null)
                    logError.Add(LogOnError.InvalidLogin);
                else if (mbsUser != null && mbsUser.IsLockedOut)
                    logError.Add(LogOnError.PasswordBlocked);


                bool isValid = false;
                // check by USERNAME
                if (mbsUser != null && !logError.Contains(LogOnError.PasswordBlocked))
                {
                    isValid = this.MembershipService.ValidateUser(model.UserName, model.Password) == ValidateUserStatus.Ok;
                    if (isValid)
                        mbsUser = this.MembershipService.GetUser(model.UserName);
                    else
                        logError.Add(LogOnError.InvalidPassword);
                }

                // check by EMAIL ADDRESS
                if (!isValid && model.UserName.Contains('@') && !logError.Contains(LogOnError.InvalidPassword) && !logError.Contains(LogOnError.PasswordBlocked))
                {
                    mbsUser = this.MembershipService.GetUserByEmail(model.UserName);
                    if (mbsUser != null && mbsUser.IsLockedOut)
                        logError.Add(LogOnError.PasswordBlocked);
                    else if (mbsUser != null && !string.IsNullOrEmpty(mbsUser.UserName))
                    {
                        isValid = MembershipService.ValidateUser(mbsUser.UserName, model.Password) == ValidateUserStatus.Ok;
                        if (!isValid)
                            logError.Add(LogOnError.InvalidPassword);
                    }
                    else
                        logError.Add(LogOnError.InvalidEmail);
                }

                isValid &= mbsUser != null;

                // auth successfull
                if (isValid)
                {
                    //
                    // CRITICAL BEGINS
                    // permits to fill the username in the logs
                    using (this.Logger.OverrideIdentity(ServiceIdentity.Anonymous))
                    {
                        // CRITICAL ENDS
                        //

                        // fetch person associated to the user account
                        Guid uid = mbsUser.ProviderUserKey;
                        //User me = this.Services.People.GetByGuid(uid);
                        User me = this.Services.People.GetForSessionById(uid);

                        // verify network
                        if (me.NetworkId != this.Services.NetworkId)
                        {
                            if (me.NetworkAccess.HasFlag(NetworkAccessLevel.SparkleStaff))
                            {
                                this.TempData.AddError("Ceci n'est pas votre réseau !");
                            }
                            else
                            {
                                this.Logger.Info("Account/Login", ErrorLevel.Authz, string.Format("try to login ( {0} ) but NetworkId is " + me.NetworkId, model.UserName));
                                model.ValidateMessage = "La connexion à un autre réseau n'est pas autorisée... :'(";
                                ModelState.AddModelError("", "Vous ne pouvez pas vous connecter à ce réseau.");
                                return View(model);
                            }
                        }

                        // exclude disabled users
                        if (me.NetworkAccessLevel < 1)
                        {
                            this.Logger.Info("Account/Login", ErrorLevel.Authz, string.Format("try to login ( {0} ) but NetworkAccessLevel is " + me.NetworkAccessLevel, model.UserName));

                            model.ValidateMessage = "Vous ne pouvez pas vous connecter aujourd'hui... :'(";
                            ModelState.AddModelError("", "Votre compte ne peut pas se connecter au site actuellement.");
                            ////ModelState.AddModelError("", "Vous ne travaillez plus à " + Lang.T("CurrentPlaceAlone"));
                            ////ModelState.AddModelError("", "Vous n'avez pas respecté les conditions d'utilisations");
                            ////ModelState.AddModelError("", "Une régularisation/contractualisation avec votre entreprise est en cours");
                            return View(model);
                        }

                        if (me.CompanyAccessLevel < 1)
                        {
                            this.Logger.Info("Account/Login", ErrorLevel.Authz, string.Format("try to login ( {0} ) but CompanyAccessLevel is " + me.NetworkAccessLevel, model.UserName));

                            model.ValidateMessage = "Vous ne pouvez pas vous connecter aujourd'hui... :'(";
                            ModelState.AddModelError("", "Votre compte est actuellement désactivé par un responsable de votre entreprise.");
                            return View(model);
                        }

                        // exclude account not validated by company admin
                        if (me.AccountClosed.HasValue && me.AccountClosed.Value)
                        {
                            this.Logger.Info("Account/Login", ErrorLevel.Authz, string.Format("try to login ( {0} ) but account not validated by company admin", model.UserName));
                            model.ValidateMessage = "Vous ne pouvez pas vous connecter aujourd'hui... :'(";
                            ModelState.AddModelError("", "Votre compte doit être validé par un responsable de l'entreprise.");
                            return View(model);
                        }

                        // email not confirmed
                        if (!me.IsEmailConfirmed)
                        {
                            this.Logger.Info("Account/Login", ErrorLevel.Authz, string.Format("try to login ( {0} ) but account email address is not confirmed", model.UserName));
                            model.EmailActivationAction = this.Services.UserActionKeys.GetLatestAction(me.Id, UserActionKey.UserEmailConfirmActionKey);
                            model.User = me;
                            model.ValidateMessage = "Vous n'avez pas confirmé votre adresse email... :'(";

                            return View(model);
                        }

                        // company disabled
                        if (!me.Company.IsEnabled)
                        {
                            this.Logger.Info("Account/Login", ErrorLevel.Authz, string.Format("try to login ( {0} ) but company is disabled", model.UserName));

                            model.ValidateMessage = "Vous ne pouvez pas vous connecter aujourd'hui... :'(";
                            ModelState.AddModelError("", "Votre entreprise est actuellement désactivé par un administrateur du réseau.");
                            return View(model);
                        }

                        // GO !!! give cookies to user (macadamia cookies)
                        this.Logger.Info("MembershipService.ValidateUser", ErrorLevel.Success, string.Format("On LogOn: login: {0}", model.UserName));
                        FormsService.SignIn(mbsUser.UserName, model.RememberMe);

                        // populate session
                        /////this.SessionService.User = me;
                        var newUserInSession = this.SessionService.User;

                        // Check Notifications
                        this.Services.Notifications.InitializeNotifications(me);

                        var lInMatchId = this.SessionService.LinkedInMatchId;
                        var lInMatchEmail = this.SessionService.LinkedInMatchEmail;
                        if (!string.IsNullOrEmpty(lInMatchId) && !string.IsNullOrEmpty(lInMatchEmail))
                        {
                            this.Services.People.TryUpdateLinkedInId(me, lInMatchId, lInMatchEmail);
                        }

                        // redirect to request page 
                        if (Url.IsLocalUrl(model.ReturnUrl) && !model.ReturnUrl.ToLowerInvariant().Contains("account/logo"))
                        {
                            return Redirect(model.ReturnUrl);
                        }

                        // or redirect to home
                        return RedirectToAction("News", "Home");
                    }
                }

                this.Logger.Info("Account/Login", ErrorLevel.Authn, string.Format("login: {0}, error: {1}", model.UserName, string.Join("; ", logError)));

                switch (logError.Last())
                {
                    case LogOnError.InvalidLogin:
                        ModelState.AddModelError("", Lang.T("L'identifiant indiqué n'est pas bon."));
                        break;

                    case LogOnError.InvalidEmail:
                        ModelState.AddModelError("", Lang.T("L'adresse email est invalide."));
                        break;

                    case LogOnError.InvalidPassword:
                        ModelState.AddModelError("", Lang.T("Le mot de passe incorrect."));
                        break;

                    case LogOnError.PasswordBlocked:
                        this.TempData.AddWarning(Lang.T("Votre mot de passe n'est plus valide."));
                        return RedirectToAction("Recover", "Account", new { id = mbsUser != null ? mbsUser.Email : model.UserName });

                    default:
                        break;
                }
            }
            else
            {
                // log any wrong attempt
                this.Logger.Info("Account/Login", ErrorLevel.Input, string.Format("login: {0}, \r\nstate:\r\n{1}", model.UserName, ModelState.Summary()));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AuthorizeUser]
        public ActionResult LogOff()
        {
            this.Logger.Info("Account/Logoff", ErrorLevel.Success, User.Identity.Name);
            this.SessionService.Clear();
            FormsService.SignOut();

            return this.RedirectToAction("Welcome", "Home");
        }

        public ActionResult LinkedInLogOn(string ReturnUrl, bool RememberMe = false)
        {
            if (this.User.Identity != null && this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("News", "Home");
            }

            if (string.IsNullOrEmpty(ReturnUrl))
            {
                ReturnUrl = "/Account/LogOn";
            }

            var returnUrl = UrlTools.Compose(this.Request) + "/Account/LinkedInLogOnReturn?ReturnUrl=" + Uri.EscapeDataString(ReturnUrl) + "&RememberMe=" + RememberMe;
            var failReturnUrl = UrlTools.Compose(this.Request) + "/Account/LinkedInLogOnReturn?error=IdNotFound&errorDesc=The+redirect+id+was+not+found";
            string goUrl;

            // Init LinkedIn
            bool success;
            var initResult = this.InitLinkedIn(this.Services, ReturnUrl, returnUrl, (int)linkedInScope, out goUrl, out success);
            if (success)
            {
                return this.Redirect(goUrl + "&failReturnUrl=" + Uri.EscapeDataString(failReturnUrl));
            }
            else
            {
                return initResult;
            }
        }

        public ActionResult LinkedInLogOnReturn(string code, Guid? redirId, string error, string errorDesc, string ReturnUrl, bool RememberMe)
        {
            AuthorizationAccessToken token;
            bool success;
            var resultToken = this.GetLinkedInToken(this.Services, code, redirId, error, errorDesc, ReturnUrl, out token, out success);
            if (!success)
                return resultToken;

            // make api call to get linkedin userid & give credentials
            var result = this.Services.People.GetUserFromLinkedInId(new ConnectWithLinkedInRequest { AccessToken = token, });
            if (this.ValidateResult(result, MessageDisplayMode.TempData))
            {
                //
                // CRITICAL BEGINS
                // permits to fill the username in the logs
                using (this.Logger.OverrideIdentity(ServiceIdentity.Anonymous))
                // CRITICAL ENDS
                //
                {
                    // GO !!! give cookies to user (macadamia cookies)
                    this.Logger.Info("MembershipService.ValidateUser", ErrorLevel.Success, string.Format("On LogOn: login: {0}", result.UserMatch.Username));
                    FormsService.SignIn(result.UserMatch.Username, RememberMe);

                    // populate session
                    /////this.SessionService.User = me;
                    var newUserInSession = this.SessionService.User;

                    // Check Notifications
                    this.Services.Notifications.InitializeNotifications(result.UserMatch);

                    // redirect to request page (if it is not a logon/logoff page)
                    if (Url.IsLocalUrl(ReturnUrl) && !ReturnUrl.ToLowerInvariant().Contains("account/logo"))
                    {
                        return Redirect(ReturnUrl);
                    }

                    // or redirect to home
                    return RedirectToAction("News", "Home");
                }
            }

            if (result.PartialMatch)
            {
                this.SessionService.LinkedInMatchId = result.LinkedInUserId;
                this.SessionService.LinkedInMatchEmail = result.LinkedInEmail;
            }

            return this.RedirectToAction("LogOn", new { ReturnUrl = ReturnUrl, });
        }

        public ActionResult Register(string id)
        {
            Guid code;
            var error = this.CheckInvitationCode(id, out code);
            if (error != null)
                return error;

            var model = this.Services.People.GetCreateEmailPassordAccountModel(default(CreateEmailPassordAccountRequest), invitationCode: code);
            return this.View(model);
        }

        [HttpPost]
        public ActionResult Register(CreateEmailPassordAccountRequest model, string returnUrl)
        {
            var error = this.CheckInvitationCode(model.InvitationCode);
            if (error != null)
                return error;

            model = this.Services.People.GetCreateEmailPassordAccountModel(model, null);

            if (this.ModelState.IsValid)
            {
                var result = this.Services.People.CreateEmailPasswordAccount(model);
                if (this.ValidateResult(result))
                {
                    this.MarkAuthenticated(result.User);
                    return this.RedirectToAction("MyPicture", "QuickStart");
                }
            }

            return this.View(model);
        }

        private void MarkAuthenticated(Sparkle.Services.Networks.Models.UserModel user)
        {
            FormsService.SignIn(user.Username, true);
            this.SessionService.Clear();
            this.SessionService.ReviveIfDead(() => this.Services, this.HttpContext, user.Username);
            var me = this.SessionService.User;
        }

        private ActionResult CheckInvitationCode(string id, out Guid code)
        {
            if (id == null || !Guid.TryParse(id, out code))
            {
                this.TempData.AddError(Alerts.AccountRegister_InvalidInvitationCode);
                this.ViewBag.Message = Alerts.AccountRegister_InvalidInvitationCode;
                code = Guid.Empty;
                return this.View("RegisterError");
            }

            return this.CheckInvitationCode(code);
        }

        private ActionResult CheckInvitationCode(Guid? code)
        {
            if (code == null)
            {
                this.TempData.AddError(Alerts.AccountRegister_InvalidInvitationCode);
                this.ViewBag.Message = Alerts.AccountRegister_InvalidInvitationCode;
                return this.View("RegisterError");
            }


            var validate = this.Services.Invited.ValidateCode(code.Value);
            if (!validate.IsValid)
            {
                foreach (var error in validate.Errors)
                {
                    this.TempData.AddError(error.DisplayMessage ?? error.Code.ToString());
                    this.ViewBag.Message = error.DisplayMessage ?? error.Code.ToString();
                }

                return this.View("RegisterError");
            }

            return null;
        }

        public ActionResult Unregister(Guid id)
        {
            var invitation = this.GetUnregisterModel(id);

            this.ViewBag.Invitation = invitation;

            if (invitation == null)
            {
                return this.View();
            }

            if (invitation.UserId.HasValue)
            {
                return this.RedirectToAction("Settings", "Account");
            }

            this.ViewBag.CanConfirm = true;

            return this.View();
        }

        [HttpPost]
        public ActionResult Unregister(Guid id, string email, FormCollection form)
        {
            var invitation = this.GetUnregisterModel(id);

            if (invitation == null)
            {
                this.ViewBag.ErrorMessage = "Adresse email inconnue ou jeton invalide.";
                return this.View();
            }

            if (invitation.UserId.HasValue)
            {
                this.ViewBag.CanConfirm = false;
                return this.RedirectToAction("Settings", "Account");
            }

            this.ViewBag.CanConfirm = true;

            ////this.Services.Invited.Update(invited);

            if (invitation.Unregistred && form["register"] != null)
            {
                invitation.Unregistred = false;
                this.Services.Invited.Update(invitation);
                this.Logger.Info("Account/Unregister", ErrorLevel.Success, "Invited '" + invitation.Email + "' registered to newsletter");
            }
            else if (!invitation.Unregistred && form["unregister"] != null)
            {
                invitation.Unregistred = true;
                this.Services.Invited.Update(invitation);
                this.Logger.Info("Account/Unregister", ErrorLevel.Success, "Invited '" + invitation.Email + "' unregistered from newsletter");
            }

            return this.View();
        }

        private Invited GetUnregisterModel(Guid id)
        {
            var invitation = this.Services.Invited.GetByInvitationKey(id);

            this.ViewBag.id = id;
            this.ViewBag.email = invitation.Email;
            this.ViewBag.Invitation = invitation;
            this.ViewBag.CanConfirm = false;
            this.ViewBag.Confirmed = false;

            if (invitation == null)
            {
                this.ViewBag.ErrorMessage = "Adresse email inconnue.";
            }
            else
            {
                this.ViewBag.ErrorMessage = null;
            }

            return invitation;
        }

        public ActionResult RegisterRequest(bool UseApply = true)
        {
            // this code was for networks without companies. use apply now.
            ////bool usersRegisterInCompany = this.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany != null;
            ////if (usersRegisterInCompany)
            ////{
            ////    return this.RedirectToAction("Join");
            ////}

            bool allowUseRegisterRequest = false; // create a configuration entry if this if desired
            this.ViewBag.UseApply = UseApply;
            if (UseApply && allowUseRegisterRequest)
            {
                return this.RedirectToAction("Index", "Apply");
            }

            var model = new RegisterRequestModel();

            var companies = CompaniesController.GetCompaniesFromCache(this.Services);

            var vis = new List<SelectListItem>();
            foreach (var company in companies)
            {
                vis.Add(new SelectListItem()
                {
                    Value = company.Id.ToString(),
                    Text = company.Name,
                    Selected = model.CompanyId == company.Id,
                });
            }

            this.ViewBag.Companies = vis;

            return this.View(model);
        }

        [HttpPost]
        public ActionResult RegisterRequest(RegisterRequestModel model, string email, bool UseApply = true)
        {
            bool allowUseRegisterRequest = false; // create a configuration entry if this if desired
            this.ViewBag.UseApply = UseApply;
            if (UseApply && allowUseRegisterRequest)
            {
                return this.RedirectToAction("Index", "Apply");
            }

            EmailAddress address = EmailAddress.TryCreate(email);

            // Companies
            bool usersRegisterInCompany = this.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany != null;
            if (usersRegisterInCompany)
                return this.RedirectToAction("Join");

            Company defaultCompany = null;
            if (usersRegisterInCompany)
            {
                defaultCompany = this.Services.Company.GetById(this.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany.Value);
            }
            else
            {
                var companies = CompaniesController.GetCompaniesFromCache(this.Services);

                var vis = new List<SelectListItem>();
                foreach (var company in companies)
                {
                    vis.Add(new SelectListItem()
                    {
                        Value = company.Id.ToString(),
                        Text = company.Name,
                        Selected = model.CompanyId == company.Id,
                    });
                }

                ViewBag.Companies = vis;
            }

            if (!this.ModelState.IsValid || address == null)
            {
                return this.View(model);
            }

            model.HttpPost = true;

            var invited = this.Services.Invited.GetByEmail(address.Value);
            var user = this.Services.People.SelectWithProMail(address.Value);
            User invitedUser = (invited != null && invited.UserId != null) ? this.Services.People.SelectWithId(invited.UserId.Value) : null;

            // check already user or invited (using Invited table)
            if (invited != null || user != null)
            {
                ////if (invited != null && user == null && invitedUser == null)
                ////{
                ////    // already a invited
                ////    this.TempData.AddInfo("Vous avez reçu un email d'invitation. Suivez le lien qu'il contient pour confirmer votre inscription.");
                ////    return this.View(model);
                ////}
                ////else
                if (user != null && invitedUser != null && user.Id != invitedUser.Id)
                {
                    // already a user, redirect to login form
                    this.TempData.AddInfo("Vous êtes déjà inscrit !");
                    return this.RedirectToAction("LogOn", new { username = address.Value, });
                }
                else if (user != null || invited != null && invited.UserId.HasValue)
                {
                    // already a user, redirect to login form
                    this.TempData.AddInfo("Vous êtes déjà inscrit !");
                    if (user != null)
                        return this.RedirectToAction("LogOn", new { username = user.Email, });
                    else
                        return this.RedirectToAction("LogOn", new { username = address.Value, });
                }
                else
                {
                    // already invited, send invitation again
                    model.Invited = true;
                    model.ValideCompany = true;

                    var result = this.Services.Invited.InviteAgain(null, email);
                    switch (result.Code)
                    {
                        case InvitePersonResult.ResultCode.Done:
                        case InvitePersonResult.ResultCode.AlreadyInvited:
                            // great, that's what's expected
                            this.TempData.AddInfo(Alerts.AlreadyInvitedEmailSentAgain);
                            break;

                        case InvitePersonResult.ResultCode.SmtpError:
                        case InvitePersonResult.ResultCode.Error:
                            // too bad
                            this.TempData.AddInfo("Une erreur est survenue lors de l'envoi de l'email. L'équipe de support en a été informée.");
                            var message = "AccountController/RegisterRequest: invited!=null && invited.UserId.HasValue -> InviteAgain('" + email + "') failed";
                            Trace.WriteLine(message);
                            HttpErrorReport.Do(
                                this.HttpContext, true, result.Error, this.Application.AppStartTimeUtc, this.Application.GetType().Assembly,
                                message);
                            break;

                        case InvitePersonResult.ResultCode.InvalidAddress:
                        case InvitePersonResult.ResultCode.UserExists:
                        case InvitePersonResult.ResultCode.NoCompany:
                        case InvitePersonResult.ResultCode.NotAuthorized:
                        case InvitePersonResult.ResultCode.QuotaReached:
                        case InvitePersonResult.ResultCode.CreateCompanyRequestSend:
                        default:
                            // should not happen
                            break;
                    }

                    model.InviteCode = result.Code;
                }
            }
            else
            {
                // no invitation/user match
                // search for a matching company
                var companyByDomain = this.Services.Company.SelectByDomainName(address.DomainPart);

                if (companyByDomain != null)
                {
                    // found company match
                    model.ValideCompany = true;

                    ////this.Services.People.CreateEmailPasswordAccount();

                    InvitePersonResult result;
                    try
                    {
                        result = this.Services.Invited.Invite(null, address.Value, companyByDomain.ID);
                    }
                    catch (Exception ex)
                    {
                        result = InvitePersonResult.OtherError(ex);
                    }

                    switch (result.Code)
                    {
                        case InvitePersonResult.ResultCode.Error:
                        case InvitePersonResult.ResultCode.SmtpError:
                            var message = "AccountController/RegisterRequest: invited==null && company!=null -> Invite('" + email + "', '" + companyByDomain.ID + "') failed";
                            Trace.WriteLine(message);
                            HttpErrorReport.Do(
                                this.HttpContext, true, result.Error, this.Application.AppStartTimeUtc, this.Application.GetType().Assembly,
                                message);
                            break;
                    }

                    model.InviteCode = result.Code;
                }
                else if (model.CompanyId > 0)
                {
                    var company = this.Services.Company.GetById(model.CompanyId);
                    if (company == null)
                    {
                        model.HttpPost = false;
                        this.ModelState.AddModelError("CompanyId", "Ce champ est nécessaire.");
                        return this.View(model);
                    }

                    var result = this.Services.RegisterRequests.EmitRegisterRequest(model.Email, company.ID);

                    switch (result.Code)
                    {
                        case EmitRegisterRequestCode.RequestEmitted:
                        case EmitRegisterRequestCode.RequestExists:
                            model.HttpPost = true;
                            return this.RedirectToAction("RegisterRequestResult", new { id = result.Entity.Code.ToString(), });

                        default:
                            this.ModelState.AddModelError("", "Oops, une erreurs est survenue.");
                            break;
                    }
                }
                else
                {
                    model.HttpPost = false;
                }
            }

            return this.View(model);
        }

        public ActionResult RegisterRequestResult(Guid id)
        {
            var request = this.Services.RegisterRequests.GetByCode(id, RegisterRequestOptions.All);
            if (request == null)
                return this.ResultService.NotFound();

            return this.View(request);
        }

        public ActionResult Join(bool UseApply = true)
        {
            bool usersRegisterInCompany = this.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany != null;
            if (!usersRegisterInCompany)
            {
                return this.RedirectToAction("RegisterRequest");
            }

            bool allowUseJoinRequest = false; // create a configuration entry if this if desired
            this.ViewBag.UseApply = UseApply;
            if (UseApply && allowUseJoinRequest)
            {
                return this.RedirectToAction("Index", "Apply");
            }

            var model = new JoinModel();
            model.Request = model.Request ?? new CreateEmailPassordAccountRequest
            {
                Gender = this.Services.AppConfiguration.Tree.Features.Users.DefaultGender == "Male" ? NetworkUserGender.Male : NetworkUserGender.Female,
            };

            return this.View(model);
        }

        [HttpPost]
        public ActionResult Join(JoinModel model, bool UseApply = true)
        {
            bool usersRegisterInCompany = this.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany != null;
            if (!usersRegisterInCompany)
            {
                return this.RedirectToAction("RegisterRequest");
            }

            bool allowUseJoinRequest = false; // create a configuration entry if this if desired
            this.ViewBag.UseApply = UseApply;
            if (UseApply && allowUseJoinRequest)
            {
                return this.RedirectToAction("Index", "Apply");
            }

            model = model ?? new JoinModel();
            model.Request = model.Request ?? new CreateEmailPassordAccountRequest
            {
                Gender = this.Services.AppConfiguration.Tree.Features.Users.DefaultGender == "Male" ? NetworkUserGender.Male : NetworkUserGender.Female,
            };

            if (this.Request.HttpMethod == "POST" && this.ModelState.IsValid)
            {
                var result = this.Services.People.CreateEmailPasswordAccount(model.Request);
                if (this.ValidateResult(result, MessageDisplayMode.ModelState, "Request"))
                {
                    if (result.EmailSent)
                    {
                        this.TempData.AddInfo(Lang.T("Nous vous avons envoyé un email pour vérifier votre adresse email."));
                    }
                    else
                    {
                        this.TempData.AddWarning(Lang.T("Nous vous avons envoyé un email pour vérifier votre adresse email."));
                    }

                    return this.RedirectToAction("Welcome", "Home");
                }
                else
                {
                    if (result.Errors.Any(e => e.Code == CreateEmailPassordAccountError.UserEmailAlreadyExists))
                    {
                        var user = this.Services.People.SelectWithProMail(model.Request.Email);
                        if (user != null)
                        {
                            if (!user.IsEmailConfirmed)
                            {
                                var action = this.Services.Repositories.UserActionKeys.GetLatestAction(user.Id, UserActionKey.UserEmailConfirmActionKey);
                                if (action != null)
                                {
                                    model.ConfirmEmailAction = action;
                                }
                            }
                        }
                    }
                }
            }

            return this.View(model);
        }

        public ActionResult SendActivationEmail(string email, string returnUrl)
        {
            var result = this.Services.People.SendActivationEmail(email);

            if (this.ValidateResult(result, MessageDisplayMode.TempData))
            {
                this.TempData.AddConfirmation(Lang.T("Un email de confirmation vous a été envoyé."));
            }

            this.TempData.AddInfo("Si vous rencontrez des difficultés pour vous identifier, <a href=\"" + Urls.SupportPage + "\">vous pouvez nous contacter directement</a>. ", true);

            return this.RedirectToLocal(returnUrl, this.Url.Action("LogOn"));
        }

        public ActionResult Confirm(int id, string secret)
        {
            var result = this.Services.People.ConfirmEmail(id, secret);

            if (this.ValidateResult(result, MessageDisplayMode.TempData))
            {
                this.TempData.AddConfirmation(Lang.T("Parfait ! Votre adresse email est confirmée."));

                this.SessionService.ClearUser();

                if (this.UserId == null)
                {
                    FormsAuthentication.SetAuthCookie(result.User.Login.ToString(), true);
                    this.SessionService.ReviveIfDead(() => this.Services, this.HttpContext);
                }

                return this.RedirectToAction("Index", "QuickStart");
            }

            return this.View(result);
        }

        [AuthorizeUser]
        public ActionResult Welcome()
        {
            return this.ResultService.Gone();
        }

        [AuthorizeUser]
        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordModel
            {
                ReturnUrl = this.Request.UrlReferrer != null ? this.Request.UrlReferrer.PathAndQuery : null,
            };
            this.ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return this.View(model);
        }

        [AuthorizeUser]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    this.Logger.Info("Account/ChangePassword", ErrorLevel.Success);
                    this.TempData.AddConfirmation("Votre mot de passe est maintenant changé !");
                    return this.RedirectToLocal(model.ReturnUrl, Url.Action("Index"));
                }
                else
                {
                    this.ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            this.ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return this.View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return this.ResultService.NotFound();
        }

        [AuthorizeUser]
        public new ActionResult Profile(bool? LinkedInConnect, string login = null)
        {
            UserModel user;

            if (string.IsNullOrEmpty(login))
            {
                user = this.Services.People.GetById(this.UserId.Value, Data.Options.PersonOptions.Company);
            }
            else
            {
                if (!this.SessionService.User.NetworkAccess.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff))
                {
                    return this.ResultService.Forbidden();
                }

                user = this.Services.People.GetByUsername(login, Data.Options.PersonOptions.Company);
                if (user == null)
                    return this.ResultService.NotFound();
            }

            if (LinkedInConnect.HasValue && LinkedInConnect.Value)
            {
                var result = this.Services.People.UpdateFromLinkedIn(new LinkedInPeopleRequest(this.UserId.Value));
                if (result.Succeed)
                {
                    this.TempData.AddInfo(Alerts.LinkedInUpdateProfileSuccess);
                    if (result.UserLinkedInId)
                        this.TempData.AddInfo(Alerts.LinkedInUpdateProfileSuccessIdFound);
                    return this.RedirectToLocal("/Person/" + user.Login);
                }
                else
                {
                    if (result.Errors.Any(e => e.Code == LinkedInPeopleError.InvalidApiToken))
                    {
                    }

                    var error = result.Errors.First();
                    if (error.Code == LinkedInPeopleError.InvalidApiToken)
                    {
                        var cssClass = "accentColor";
                        var encodedUrl = Uri.EscapeDataString("/Account/Profile?LinkedInConnect=True");

                        var leftSide = string.Format("<a href=\"/Account/LinkedInConnect?ReturnUrl={0}\" class=\"{1}\">", encodedUrl, cssClass);
                        var rightSide = "</a>";

                        var message = this.Services.Lang.T("Votre session LinkedIn a expiré. Essayez à nouveau {0}maintenant{1} ;)", leftSide, rightSide);
                        this.TempData.AddError(message, isMarkup: true);
                    }
                    else
                    {
                        this.TempData.AddError(error.DisplayMessage);
                    }

                    this.Logger.Error(
                        "/Account/Profile",
                        ErrorLevel.ThirdParty,
                        "Error when getting the profile (user {0}) from LinkedIn with error: {1} ({2})",
                        this.UserId.Value,
                        error.DisplayMessage,
                        error.Code.ToString());
                }

                return this.RedirectToAction("Profile", new { login = login, });
            }

            var model = this.Services.People.GetProfileEditRequest(user.Id, null);
            return View(model);
        }

        [HttpPost]
        [AuthorizeUser]
        public new ActionResult Profile(/*[ModelBinder(typeof(ProfileEditRequestBinder))]*/ ProfileEditRequest model)
        {
            if (model.Id != this.SessionService.User.Id && !this.SessionService.User.NetworkAccess.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff))
            {
                return this.ResultService.Forbidden();
            }

            UserModel user = this.Services.People.GetByUsername(model.Login, Data.Options.PersonOptions.Company);
            if (user == null)
            {
                return this.ResultService.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                var result = this.Services.People.UpdateUserProfile(model);
                if (!result.Succeed)
                {
                    var error = result.Errors.FirstOrDefault();
                    if (error == null)
                    {
                        this.TempData.AddError(Alerts.UnknownError);
                    }
                    else
                    {
                        this.TempData.AddError(error.DisplayMessage);
                    }
                }
                else
                {
                    this.TempData.AddInfo(Alerts.EditRequestSuccess);
                    return this.RedirectToAction("People", "Peoples", new { id = user.Login, });
                }
            }

            model = this.Services.People.GetProfileEditRequest(user.Id, model);

            return View(model);
        }

        [AuthorizeUser]
        public ActionResult Visits(short id = 1)
        {
            VisitsModel model = new VisitsModel();

            var items = this.Services.PeopleVisits.GetByProfile(this.UserId.Value).OrderByDescending(v => v.Date);

            foreach (var item in items)
            {
                User user = this.Services.People.SelectWithId(item.UserId);

                if (user != null)
                {
                    model.Visits.Add(new VisitModel()
                    {
                        Date = item.Date,
                        Count = item.ViewCount,
                        Name = user.FirstName + " " + user.LastName,
                        Login = user.Login,
                        ////Picture = this.GetSimpleUrl("Data", "PersonPicture", "DataPicture", new { id = user.Login, size = "Medium", }),
                        Picture = this.Services.People.GetProfilePictureUrl(user.Login, UserProfilePictureSize.Medium, UriKind.Relative),
                        JobName = (user.Job != null ? user.Job.Libelle : null),
                        JobAlias = (user.Job != null ? user.Job.Alias : null),
                        CompanyName = user.Company.Name,
                        CompanyAlias = user.Company.Alias
                    });
                }
            }

            return View(model);
        }

        [HttpPost]
        [AuthorizeUser]
        public ActionResult Permissions(string user, byte value)
        {
            User contact = this.Services.People.SelectWithLogin(user);

            // Take most recent values instead of session
            User me = this.Services.People.SelectWithId(this.SessionService.User.Id);

            if (contact == null || me == null)
            {
                return new ResultService(this.HttpContext)
                    .JsonError("bad request", (contact == null ? "Contact not found. " : "") + (me == null ? "Self not found. " : ""));
            }

            if (this.Services.People.CanChangeUserCompanyAccess(contact, me, (CompanyAccessLevel)value))
            {
                contact.AccountRight = value; // obsolete
                contact.CompanyAccessLevel = value;
                this.Services.People.Update(contact);
                return this.ResultService.JsonSuccess();
            }
            else
            {
                return this.ResultService.JsonError("unauthorized", Alerts.ChangeCompanyPermissionUnauthorized);
            }

        }

        [AuthorizeUser]
        public ActionResult ContactRequests()
        {
            ContactRequetsModel model = new ContactRequetsModel();

            var items = this.Services.SeekFriends.SelectSeekFriendsByTargetId(this.UserId.Value, SeekFriendOptions.SeekerCompany | SeekFriendOptions.TargetCompany);
            model.ContactRequests = items
                .Select(u => new PeopleModel
                {
                    FirstName = u.Seeker.FirstName,
                    LastName = u.Seeker.LastName,
                    Login = u.Seeker.Username,
                    JobName = (u.Seeker.Job != null ? u.Seeker.Job.Libelle : null),
                    CompanyName = u.Seeker.Company.Name,
                    UserId = u.Seeker.Id,
                })
                .ToList();

            return View(model);
        }

        public ActionResult Settings(string Type, string User)
        {
            var userId = this.UserId;
            if (!string.IsNullOrEmpty(User)
                && (userId = SimpleSecrets.VerifyUsersEmailNotificationActionHash(this.Services.NetworkId, User)).HasValue)
            {
                Thread.CurrentThread.CurrentCulture =
                    Thread.CurrentThread.CurrentUICulture =
                    this.Services.People.GetCulture(userId.Value);

                if (!string.IsNullOrEmpty(Type))
                {
                    var result = this.Services.Notifications.Unsubscribe(new UnsubscribeFromNotificationRequest { UserId = userId.Value, Type = Type, });
                    if (this.ValidateResult(result, MessageDisplayMode.TempData))
                    {
                        this.TempData.AddInfo(Alerts.UnsubscribeUniqueNotificationSuccess);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(User))
                    this.TempData.AddError(Alerts.UnsubscribeUniqueNotificationInvalidKey);
                if (!(userId = this.UserId).HasValue)
                    return this.RedirectToAction("LogOn", new { ReturnUrl = "/Account/Settings", });
            }

            var notifs = this.Services.Notifications.GetNotifications(userId.Value);
            var userEntity = this.SessionService.User ?? this.Services.People.SelectWithId(userId.Value);
            var model = new SettingsModel();
            model.UserKey = User;
            model.PopulateValues(userEntity, notifs);
            model.PopulateStatus(userEntity, notifs);
            return this.View(model);
        }

        [HttpPost]
        public ActionResult Settings(SettingsModel model)
        {
            var userId = this.UserId;
            if (!string.IsNullOrEmpty(model.UserKey)
                && (userId = SimpleSecrets.VerifyUsersEmailNotificationActionHash(this.Services.NetworkId, model.UserKey)).HasValue)
            {
                Thread.CurrentThread.CurrentCulture =
                    Thread.CurrentThread.CurrentUICulture =
                    this.Services.People.GetCulture(userId.Value);
            }
            else
            {
                if (!string.IsNullOrEmpty(model.UserKey))
                    this.TempData.AddError(Alerts.UnsubscribeUniqueNotificationInvalidKey);
                if (!(userId = this.UserId).HasValue)
                    return this.RedirectToAction("LogOn", new { ReturnUrl = "/Account/Settings", });
            }

            var notifs = this.Services.Notifications.GetNotifications(userId.Value);
            var userEntity = this.SessionService.User ?? this.Services.People.SelectWithId(userId.Value);
            model.PopulateStatus(userEntity, notifs);
            if (this.ModelState.IsValid)
            {
                Notification choices = this.Services.Notifications.SelectNotifications(userId.Value);
                choices.ContactRequest = model.OnContactRequest;
                choices.PrivateMessage = model.OnMessage;
                choices.Comment = model.OnComment;
                choices.Publication = model.OnPublication;
                choices.EventInvitation = model.OnEventInvitation;
                choices.PrivateGroupJoinRequest = model.OnPrivateGroupJoinRequest;
                choices.Newsletter = model.onNewsletter;
                choices.DailyNewsletter = model.OnDailyNewsletter;
                if (choices.MailChimp != model.MailChimp)
                {
                    choices.MailChimp = model.MailChimp;
                    choices.MailChimpStatus = null;
                    choices.MailChimpStatusDateUtc = null;
                }

                if (choices.MainTimelineItems == null)
                {
                    if (model.MainTimelineItems != this.Services.Notifications.GetDefaultNotificationFronConfig("MainTimelineItems"))
                        choices.MainTimelineItems = model.MainTimelineItems;
                    else { /* User still have default configuration choice */ }
                }
                else
                    choices.MainTimelineItems = model.MainTimelineItems;

                if (choices.MainTimelineComments == null)
                {
                    if (model.MainTimelineComments != this.Services.Notifications.GetDefaultNotificationFronConfig("MainTimelineComments"))
                        choices.MainTimelineComments = model.MainTimelineComments;
                    else { /* User still have default configuration choice */ }
                }
                else
                    choices.MainTimelineComments = model.MainTimelineComments;

                if (this.Services.AppConfiguration.Tree.Features.EnableCompanies)
                {
                    if (choices.CompanyTimelineItems == null)
                    {
                        if (model.CompanyTimelineItems != this.Services.Notifications.GetDefaultNotificationFronConfig("CompanyTimelineItems"))
                            choices.CompanyTimelineItems = model.CompanyTimelineItems;
                        else { }
                    }
                    else
                        choices.CompanyTimelineItems = model.CompanyTimelineItems;

                    if (choices.CompanyTimelineComments == null)
                    {
                        if (model.CompanyTimelineComments != this.Services.Notifications.GetDefaultNotificationFronConfig("CompanyTimelineComments"))
                            choices.CompanyTimelineComments = model.CompanyTimelineComments;
                        else { }
                    }
                    else
                        choices.CompanyTimelineComments = model.CompanyTimelineComments;
                }

                this.Services.Notifications.Update(choices);

                this.TempData.AddInfo("Vos préférences sont enregistrées.");
            }

            return this.View(model);
        }

        [AuthorizeUser]
        public ActionResult SaveSettings(SettingsModel model)
        {
            return this.ResultService.NotFound();
        }

        [AuthorizeUser]
        public ActionResult Choices()
        {
            return RedirectToActionPermanent("Settings");
        }

        public ActionResult GetSkills(string MemberName, int maxResults)
        {
            if (!string.IsNullOrEmpty(MemberName))
            {
                MemberName = MemberName.Trim();
                MemberName = new Regex(@"[ ]{2,}", RegexOptions.None).Replace(MemberName, @" ");
                var items = this.Services.Skills.Search(MemberName, maxResults).ToList();

                // Vérifier le match parfait
                bool match = items.Any(skill => skill.TagName.ToUpperInvariant() == MemberName.ToUpperInvariant());
                if (!match)
                {
                    items.AddRange(this.GetAddTagSuggestions<Skill>(MemberName));
                }

                var model = items.Select(o => new { Name = o.TagName, Id = o.Id });
                return this.ResultService.JsonSuccess(model);
            }

            return this.ResultService.JsonError();
        }

        public ActionResult GetInterests(string MemberName, int maxResults)
        {
            if (!string.IsNullOrEmpty(MemberName))
            {
                MemberName = MemberName.Trim();
                MemberName = new Regex(@"[ ]{2,}", RegexOptions.None).Replace(MemberName, @" ");
                var items = this.Services.Interests.Search(MemberName, maxResults).ToList();

                // Vérifier le match parfait
                bool match = items.Any(item => item.TagName.ToUpperInvariant() == MemberName.ToUpperInvariant());
                if (!match)
                {
                    items.AddRange(this.GetAddTagSuggestions<Interest>(MemberName));
                }

                var model = items.Select(o => new { Name = o.TagName, Id = o.Id });
                return this.ResultService.JsonSuccess(model);
            }

            return this.ResultService.JsonError();
        }

        public ActionResult GetRecreations(string MemberName, int maxResults)
        {
            if (!string.IsNullOrEmpty(MemberName))
            {
                MemberName = MemberName.Trim();
                MemberName = new Regex(@"[ ]{2,}", RegexOptions.None).Replace(MemberName, @" ");
                var items = this.Services.Recreations.Search(MemberName, maxResults).ToList();

                // Vérifier le match parfait
                bool match = items.Any(item => item.TagName.ToUpperInvariant() == MemberName.ToUpperInvariant());
                if (!match)
                {
                    items.AddRange(this.GetAddTagSuggestions<Recreation>(MemberName));
                }

                var model = items.Select(o => new { Name = o.TagName, Id = o.Id });
                return this.ResultService.JsonSuccess(model);
            }

            return this.ResultService.JsonError();
        }

        private IEnumerable<T> GetAddTagSuggestions<T>(string name)
            where T : ITag, new()
        {
            var splitted = name.Split(',', ';', '/', '&');
            splitted = splitted
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            foreach (var split in splitted)
            {
                yield return new T
                {
                    TagName = this.GetAddTagName(split),
                };
            }
        }

        private string GetAddTagName(string name)
        {
            name = name.Trim();
            var splitted = name.Split(',', ';', '&', '/');
            return "Ajouter " + splitted[0].TrimToLength(50);
        }

        [HttpPost]
        [AuthorizeUser]
        public ActionResult UploadProfilePicture(HttpPostedFileBase image)
        {
            ResultModel model = new ResultModel();
            if (image != null)
            {
                var stream = image.InputStream;
                var mime = image.ContentType;
                var name = image.FileName;
                SetProfilePictureResult result = this.Services.People.SetProfilePicture(new SetProfilePictureRequest
                {
                    UserId = this.UserId.Value,
                    PictureStream = stream,
                    PictureMime = mime,
                    PictureName = name,
                });
                if (result.Succeed)
                {
                    ////var picture1 = this.Services.People.GetProfilePicture(this.UserId);
                    ////var picture2 = this.Services.People.GetProfilePicture(this.SessionService.User);

                    //model.Result = FileHelper.GetProfilePicture(result.User ?? this.SessionService.User); // THIS IS WRONG
                    ////model.Result = this.GetSimpleUrl("Data", "PersonPicture", "DataPicture", new { id = this.SessionService.User.Login, });
                    model.Result = this.Services.People.GetProfilePictureUrl(this.SessionService.User, UserProfilePictureSize.Medium, UriKind.Relative);

                    ////DataController.ClearUserCache(this.SessionService.User.Login);
                    this.GetCacheAccessor().PersonPictures.Invalidate(this.SessionService.User.Login);

                    this.SessionService.ClearUser();
                    this.SessionService.ReviveIfDead(() => this.Services, this.HttpContext);

                    var actionResult = (JsonNetResult)this.ResultService.JsonSuccess(new
                    {
                        Url = model.Result,
                        RefreshUrl = model.Result + "?refresh=" + DateTime.UtcNow.Ticks,
                    });
                    actionResult.ContentType = "text/html"; // ie does not like json in iframes
                    return actionResult;
                }
                else if (result.Errors.Count > 0)
                {
                    var error = result.Errors.First();
                    return this.ResultService.JsonError(error.Code.ToString(), error.DisplayMessage);
                }
                else
                {
                    return this.ResultService.JsonError();
                }
            }
            else
            {
                return this.ResultService.JsonError("ImageNull", Alerts.UploadPictureNullStream);
            }
        }

        [AuthorizeUser]
        public ActionResult AcceptedContactByMail(string id)
        {
            int UserId;
            int CurrentId = this.UserId.Value;
            if (int.TryParse(id, out UserId))
            {
                Contact contact = new Contact()
                {
                    IsDisplayed = true,
                    UserId = UserId,
                    ContactId = CurrentId,
                };

                this.Services.Friend.InsertFriends(contact);
                SeekFriend seekfriend = this.Services.SeekFriends.SelectSeekFriendsByTargetIdAndSeekerId(CurrentId, UserId);
                if (seekfriend != null)
                {
                    this.Services.SeekFriends.Delete(seekfriend);
                }

                TempData["Accepted"] = Lang.T("Invitation acceptée !");
            }

            return RedirectToAction("Settings");
        }

        [AuthorizeUser]
        public ActionResult Reload()
        {
            this.SessionService.Clear();
            return View();
        }

        public ActionResult Recover(string id, string email, bool autoRecover = false)
        {
            // kick authenticated users
            if (this.User != null && this.User.Identity != null && this.User.Identity.IsAuthenticated)
                return RedirectToAction("Forbidden", "Error");

            var accountToRecover = email ?? id;
            if (autoRecover && !string.IsNullOrEmpty(accountToRecover))
            {
                if (this.User != null && this.User.Identity != null && this.User.Identity.IsAuthenticated)
                {
                    this.TempData.AddError(Alerts.RecoverPassword_MustNotBeLoggedIn);
                    return RedirectToAction("Forbidden", "Error");
                }
                var result = this.Services.People.SendPasswordRecoveryEmailOnAutoRecover(accountToRecover);
                if (this.ValidateResult(result, MessageDisplayMode.TempData))
                {
                    return this.RedirectToAction("RecoverSuccess");
                }
            }

            var model = new AccountRecoveryModel
            {
                Email = accountToRecover,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Recover(AccountRecoveryModel model)
        {
            // kick authenticated users
            if (this.User != null && this.User.Identity != null && this.User.Identity.IsAuthenticated)
            {
                this.TempData.AddError(Alerts.RecoverPassword_MustNotBeLoggedIn);
                return RedirectToAction("Forbidden", "Error");
            }

            if (ModelState.IsValid)
            {
                var result = this.Services.People.SendPasswordRecoveryEmail(model.Email);
                this.ViewBag.Result = result;
                if (this.ValidateResult(result, MessageDisplayMode.None))
                {
                    return this.RedirectToAction("RecoverSuccess");
                }
            }

            return this.View(model);
        }

        public ActionResult RecoverSuccess()
        {
            if (Request.QueryString["e"] != null)
            {
                if (Request.QueryString["e"] == "ftse")
                {
                    ViewBag.ErrorMessage = Lang.T("Sorry, could not send the recovery email. We have been informed, please try again later.");
                }
            }

            return View();
        }

        public ActionResult EmailChangeConfirmed(string id, string key)
        {
            // find pending or error
            var pending = this.Services.UserEmailChangeRequest.SelectById(int.Parse(id));
            if (pending == null)
                goto fail;

            // find person or error
            User person = this.Services.People.SelectWithId(pending.UserId);
            if (person == null)
                goto fail;

            // find user or error
            var user = this.MembershipService.GetUser(person.Username);
            if (user == null)
                goto fail;

            string realkey = Keys.ComputeForAccount(user.ProviderUserKey, user.LastLoginDate);

            // compare key or error
            if (key != realkey)
                goto fail;

            // prepare model
            var oldEmail = new EmailAddress(pending.PreviousEmailAccountPart, pending.PreviousEmailTagPart, pending.PreviousEmailDomainPart);
            var newEmail = new EmailAddress(pending.NewEmailAccountPart, pending.NewEmailTagPart, pending.NewEmailDomainPart);

            var model = new EmailChangeConfirmedModel
                {
                    CreateDateUtc = this.Services.Context.Timezone.ConvertFromUtc(pending.CreateDateUtc).ToShortDateString(),
                    ValidateDateUtc = pending.ValidateDateUtc.HasValue ? this.Services.Context.Timezone.ConvertFromUtc(pending.ValidateDateUtc.Value).ToShortDateString() : null,
                    Status = pending.StatusValue,
                    OldEmail = oldEmail.Value,
                    NewEmail = newEmail.Value,
                };
            this.ViewData.Model = model;

            if (pending.StatusValue == UserEmailChangeRequestStatus.Pending)
                this.Services.UserEmailChangeRequest.ValidatePendingRequest(pending);

            model.IsValid = true;
            return View(model);

        fail:
            this.ModelState.AddModelError(string.Empty, "Jeton invalide ou expiré.");
            return this.View(new EmailChangeConfirmedModel());
        }

        public ActionResult Recovery(string id, string key)
        {
            // kick authenticated users
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                return this.ResultService.Forbidden();
            }

            // prepare model
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            this.ViewBag.IsPasswordReset = true;

            var model = new RecoverPasswordModel
            {
                Username = id,
                Key = key,
            };
            this.ViewData.Model = model;

            // find user or error
            var user = this.MembershipService.GetUser(id);
            if (user == null)
                goto fail;

            // find person or error
            User person = this.Services.People.GetByGuid(user.ProviderUserKey);
            if (person == null)
                goto fail;

            string realkey = Keys.ComputeForAccount(user.ProviderUserKey, user.LastLoginDate);

            // compare key or error
            if (key != realkey)
                goto fail;

            model.IsValid = true;
            this.ViewBag.IsPasswordReset = !user.IsLockedOut;
            this.ViewBag.Email = user.Email;

            return View(model);

        fail:
            this.ModelState.AddModelError(string.Empty, "Jeton invalide ou expiré.");
            return this.View(model);
        }

        [HttpPost]
        public ActionResult Recovery(RecoverPasswordModel model)
        {
            // kick authenticated users
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Forbidden", "Error");

            // verify model
            model.IsValid = false;
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            // find user or error
            var user = this.MembershipService.GetUser(model.Username);
            if (user == null)
                goto inconsistent;

            // find person or error
            User person = this.Services.People.GetByGuid(user.ProviderUserKey);
            if (person == null)
                goto inconsistent;

            string realkey = Keys.ComputeForAccount(user.ProviderUserKey, user.LastLoginDate);

            // compare key or error
            if (model.Key != realkey)
                goto expired;

            model.IsValid = true;
            this.ViewBag.IsPasswordReset = !user.IsLockedOut;
            this.ViewBag.Email = user.Email;

            if (this.ModelState.IsValid)
            {
                // un-lock-out
                if (user.IsLockedOut)
                    this.MembershipService.Unlock(user.UserName);

                // confirm email 
                if (!person.IsEmailConfirmed)
                {
                    person.IsEmailConfirmed = true;
                    this.Services.People.Update(person);
                }

                // change password
                if (this.MembershipService.ChangePassword(user.UserName, model.NewPassword))
                {
                    this.Logger.Info("Account/Recovery", ErrorLevel.Success, string.Format("User {0} recovered its password", person.Login));
                    this.TempData["InformationMessage"] = "Mot de passe changé !";
                    this.TempData["Email"] = user.Email;

                    this.Logger.Info("MembershipService.ValidateUser", ErrorLevel.Success, string.Format("On PasswordRecovery: login: {0}", user.UserName));
                    FormsService.SignIn(user.UserName, model.RememberMe);
                    return this.RedirectToLocal("/");
                }
                else
                {
                    this.Logger.Error("Account/Recovery", ErrorLevel.Internal, string.Format("User {0} recovered its password", person.Login));
                    this.ModelState.AddModelError(string.Empty, "Oops, le changement n'a pas réussit :(");
                }
            }

            return View(model);

        inconsistent:
            this.ModelState.AddModelError(string.Empty, "Le jeton est invalide, expiré ou déjà utilisé.");
            return this.RedirectToAction("Forbidden", "Error");

        expired:
            this.ModelState.AddModelError(string.Empty, "Le jeton est invalide, expiré ou déjà utilisé.");
            return this.View(model);
        }

        [HttpPost]
        [AuthorizeUser]
        public ActionResult Invitation(string emails, bool? noNeedApproval, string companyCategoryAlias, int? companyRelationshipTypeId, string ReturnUrl)
        {
            BoxInviteStep2Model model = new BoxInviteStep2Model { MyCompanyId = this.SessionService.User.CompanyID };
            bool noCompany = false;

            // Check emails
            var valideEmails = new List<string>(Validate.ManyEmailAddresses(emails));
            valideEmails = valideEmails.Distinct().ToList();
            var companyCategory = this.Services.Company.GetCategoryByAlias(companyCategoryAlias);
            var companyRelationshipType = this.Services.CompanyRelationships.GetTypeById(companyRelationshipTypeId);
            var currentUser = this.SessionService.User;
            model.Results = new List<InviteWithApplyResult>(valideEmails.Count);

            foreach (string mail in valideEmails)
            {
                ////InvitationModel invitationModel = new InvitationModel()
                ////{
                ////    people = new PeopleModel(this.Services)
                ////    {
                ////        Email = mail,
                ////    },
                ////    ErrorMessages = string.Empty,
                ////};
                var request = new InviteWithApplyRequest();
                request.ActingUserId = currentUser.Id;
                request.Email = mail;
                request.SkipApproval = noNeedApproval ?? false;
                request.CompanyCategoryId = companyCategory != null ? companyCategory.Id : default(short?);
                request.CompanyRelationshipTypeId = companyRelationshipTypeId;

                var result = this.Services.People.InviteWithApply(request);
                model.Results.Add(result);
                ////string emailDomain = mail.Substring(mail.IndexOf("@") + 1);
                ////Company company = this.Services.Company.SelectByDomainName(emailDomain);

                ////if (company != null || !this.Services.AppConfiguration.Tree.Features.EnableCompanies)
                ////{
                ////    try
                ////    {
                ////        InvitePersonResult inviteResult;
                ////        if (!this.Services.AppConfiguration.Tree.Features.EnableCompanies)
                ////            inviteResult = this.Services.Invited.Invite(this.SessionService.User, mail, this.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany ?? 0);
                ////        else
                ////           inviteResult = this.Services.Invited.Invite(this.SessionService.User, mail, company.ID);
                ////        invitationModel.Code = inviteResult.Code;
                ////        invitationModel.Success = inviteResult.Code == InvitePersonResult.ResultCode.Done;
                ////    }
                ////    catch (Exception ex)
                ////    {
                ////        invitationModel.Code = InvitePersonResult.ResultCode.Error;
                ////        HttpErrorReport.Do(
                ////            this.HttpContext, true, ex, this.Application.AppStartTimeUtc, this.Application.GetType().Assembly,
                ////            "AccountController.Invite: error inviting by email address '" + mail + "'");
                ////    }
                ////}
                ////else
                ////{
                ////    invitationModel.Code = InvitePersonResult.ResultCode.NoCompany;
                ////    noCompany = true;
                ////}
                ////model.Items.Add(invitationModel);
            }

            if (noCompany)
                model.Companies = this.Services.Company.SelectAll();

            if (this.Request.IsAjaxRequest())
            {
                return PartialView("Invitation", model);
            }
            else
            {
                var ok = model.Results.Where(x => x.Succeed).Select(x => x.Request.Email).ToList();
                if (ok.Count == 1)
                {
                    this.TempData.AddConfirmation(Lang.T("Une invitation a été envoyée à {0}.", ok[0]));
                }
                else if (ok.Count > 1)
                {
                    this.TempData.AddConfirmation(Lang.T("Des invitations ont été envoyées à {0}.", string.Join(", ", ok)));
                }

                var errors = model.Results.Where(x => x.Errors != null && x.Errors.Count > 0).ToList();
                foreach (var error in errors)
                {
                    this.TempData.AddError(Lang.T("Impossible d'inviter {0}. {1}", error.Request.Email, error.Errors[0].DisplayMessage));
                }

                return this.RedirectToLocal(ReturnUrl, this.Url.Action("Invitations"));
            }
        }

        [HttpPost]
        [AuthorizeUser]
        public ActionResult ParseContactsVCard(HttpPostedFileBase file)
        {
            if (file != null)
            {
                try
                {
                    var text = new System.IO.StreamReader(file.InputStream, System.Text.Encoding.GetEncoding("iso-8859-1"));
                    var reader = new vCardStandardReader();

                    var model = new Sparkle.Services.Networks.Models.LightContactsModel();
                    vCard vcard;
                    while ((vcard = reader.Read(text)) != null && !string.IsNullOrEmpty(vcard.FormattedName))
                    {
                        var contact = new Sparkle.Services.Networks.Models.LightContactModel();
                        contact.Firstname = vcard.GivenName;
                        contact.Lastname = vcard.FamilyName;
                        contact.Title = vcard.Title;
                        contact.Place = vcard.Organization;
                        contact.Emails = vcard.EmailAddresses.Select(o => o.Address).ToArray();
                        contact.IsOnNetwork = contact.Emails.Any(o => this.Services.People.IsEmailAddressInUse(o));

                        model.Contacts.Add(contact);
                    }

                    this.SessionService.LinkedInContactsImport = model;
                    return this.ResultService.JsonSuccess(model);
                }
                catch (Exception ex)
                {
                    this.Services.Logger.Error(
                        "AccountController/ParseContactsVCard",
                        ErrorLevel.ThirdParty,
                        ex);
                    return this.ResultService.JsonError("Thought.vCard_Error", Alerts.Invitations_vCardReadingFailed);
                }
            }

            return this.ResultService.JsonError();
        }

        [HttpPost]
        [AuthorizeUser]
        public ActionResult Invitations(Guid[] id)
        {
            var returnUrl = "/Account/Invitations#Tab-LinkedIn";

            if (id.Length == 0)
            {
                this.TempData.AddError(Alerts.Invitations_NoContactsSelected);
                return this.RedirectToLocal(returnUrl);
            }

            if (this.SessionService.LinkedInContactsImport == null || this.SessionService.LinkedInContactsImport.Contacts.Count == 0)
            {
                this.TempData.AddError(Alerts.Invitations_SessionDataExpired);
                return this.RedirectToLocal(returnUrl);
            }

            var emails = new List<Sparkle.Services.Networks.Models.UserModel>();
            var users = new List<Sparkle.Services.Networks.Models.UserModel>();
            foreach (var item in this.SessionService.LinkedInContactsImport.Contacts.Where(o => id.Contains(o.UniqueId)))
            {
                var user = item.Emails.Select(o => this.Services.People.WhoUsesThisEmail(o)).FirstOrDefault();
                if (user != null)
                {
                    users.Add(user);
                }
                else
                {
                    emails.Add(new Services.Networks.Models.UserModel { FirstName = item.Firstname, LastName = item.Lastname, Email = item.Emails.First(), });
                }
            }

            var me = this.Services.People.GetActiveById(this.UserId.Value, Data.Options.PersonOptions.None);
            if (emails.Count > 0)
            {
                foreach (var user in emails)
                {
                    this.Services.Email.SendInvitationWithApply(me, user, null);
                }
                this.TempData.AddInfo(Alerts.Invitations_InvitationsSent);
            }

            if (users.Count > 0)
            {
                foreach (var user in users)
                {
                    if (!this.Services.Friend.CheckIfBothAreFriends(me.Id, user.Id))
                        this.Services.SeekFriends.Insert(new SeekFriend
                        {
                            CreateDate = DateTime.UtcNow,
                            ExpirationDate = DateTime.UtcNow.AddMonths(1),
                            SeekerId = this.UserId.Value,
                            TargetId = user.Id,
                        });
                }
                this.TempData.AddInfo(Alerts.Invitations_FriendRequestsSent);
            }

            return this.RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [AuthorizeUser]
        public ActionResult InvitationWithCompany(string emails, string id)
        {
            AccountResult result = new AccountResult();
            if (string.IsNullOrEmpty(emails) || string.IsNullOrEmpty(id))
                return Json(result);

            int companyId;
            if (!int.TryParse(id, out companyId))
                return this.ResultService.JsonError("NoSuchCompany");

            List<string> valideEmails = new List<string>(Validate.ManyEmailAddresses(emails));
            foreach (string mail in valideEmails)
            {
                Company company = this.Services.Company.GetById(companyId);
                if (company != null)
                {
                    try
                    {
                        var inviteResult = this.Services.Invited.Invite(this.SessionService.User, mail, company.ID);
                        result.Success = inviteResult.Code == InvitePersonResult.ResultCode.Done;
                        result.InviteCode = inviteResult.Code;
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.InviteCode = InvitePersonResult.ResultCode.Error;
                        HttpErrorReport.Do(
                            this.HttpContext, true, ex, this.Application.AppStartTimeUtc, this.Application.GetType().Assembly,
                            "AccountController.Invite: error inviting by email address '" + mail + "'");
                    }
                }
            }

            return this.ResultService.JsonSuccess(result);
        }

        [HttpPost]
        [AuthorizeUser]
        public JsonResult ValidateAccount(string user)
        {
            if (string.IsNullOrEmpty(user))
                return new JsonResult();

            if (this.SessionService.User.NetworkAccessLevel < 3)
                return new JsonResult();

            var contact = this.Services.People.SelectWithLogin(user);
            if (contact == null)
                return new JsonResult();

            if (contact.CompanyID != this.SessionService.User.CompanyID)
                return new JsonResult();

            contact.AccountClosed = false;
            this.Services.People.Update(contact);

            return new JsonResult();
        }

        [AuthorizeUser]
        public ActionResult ConfigureTwitter(string oauth_token, string oauth_verifier, string ReturnUrl)
        {
            if (!this.Services.AppConfiguration.Tree.Features.Users.SocialPull.IsEnabled)
                return this.ResultService.NotFound();

            // verify configuration
            var socialState = this.Services.SocialNetworkStates.GetState(SocialNetworkConnectionType.Twitter);
            if (socialState.Entity == null || !socialState.Entity.IsConfigured)
            {
                // twitter is not configured!
            }

            if (string.IsNullOrEmpty(oauth_token) || string.IsNullOrEmpty(oauth_verifier))
            {
                UriBuilder builder = new UriBuilder(this.Request.Url);
                builder.Query = string.Concat(
                    builder.Query,
                    string.IsNullOrEmpty(builder.Query) ? string.Empty : "&",
                    "ReturnUrl=",
                    ReturnUrl);

                try
                {
                    string token = OAuthUtility.GetRequestToken(
                                this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerKey,
                                this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerSecret,
                                builder.ToString()).Token;

                    return this.Redirect(OAuthUtility.BuildAuthorizationUri(token, true).ToString());
                }
                catch (Exception ex)
                {
                    this.TempData.AddError(Lang.T("Une erreur de communication avec le service Twitter est survenue. Ce problème est peut-être temporaire, réessayez-donc plus tard."));
                    this.ReportError(ex, "AccountController/ConfigureTwitter/no-tokens error");
                    return this.RedirectToAction("SocialNetworks");
                }
            }

            OAuthTokenResponse tokens;
            try
            {
                tokens = OAuthUtility.GetAccessToken(
                    this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerKey,
                    this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerSecret,
                    oauth_token,
                    oauth_verifier);
            }
            catch (Exception ex)
            {
                this.TempData.AddError(Lang.T("Une erreur de communication avec le service Twitter est survenue. Ce problème est peut-être temporaire, réessayez-donc plus tard."));
                this.ReportError(ex, "AccountController/ConfigureTwitter/tokens-received error");
                return this.RedirectToAction("SocialNetworks");
            }

            if (tokens != null)
            {
                // Save Social Network Connection
                SocialNetworkConnection connection = new SocialNetworkConnection();
                connection.CreatedByUserId = this.UserId.Value;
                connection.Type = (byte)SocialNetworkConnectionType.Twitter;
                connection.Username = tokens.ScreenName;
                connection.OAuthToken = tokens.Token;
                connection.OAuthVerifier = tokens.TokenSecret;
                connection.IsActive = true;

                int connectionId = this.Services.SocialNetworkConnections.Insert(connection);

                // Save social network subscription
                SocialNetworkUserSubscription subscription = new SocialNetworkUserSubscription();
                subscription.UserId = this.UserId.Value;
                subscription.AutoPublish = true;
                subscription.SocialNetworkConnectionsId = connectionId;

                this.Services.SocialNetworkUserSubscriptions.Insert(subscription);

                // add user to network's twitter list
                if (socialState.Entity != null && socialState.Entity.IsConfigured)
                {
                    var twitterCredentials = new LinqToTwitter.InMemoryCredentials
                    {
                        ConsumerKey = this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerKey,
                        ConsumerSecret = this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerSecret,
                        OAuthToken = socialState.Entity.OAuthAccessToken,
                        AccessToken = socialState.Entity.OAuthAccessSecret,
                    };
                    var authorizer = new LinqToTwitter.SingleUserAuthorizer
                    {
                        Credentials = twitterCredentials,
                    };
                    var twitterContext = new LinqToTwitter.TwitterContext(authorizer);
                    try
                    {
                        // find list
                        var listName = this.Services.SocialNetworkStates.GetTwitterFollowListName(this.Services.Network);
                        var lists = twitterContext.List
                            .Where(l => l.Type == LinqToTwitter.ListType.Lists && l.ScreenName == socialState.Entity.Username)
                            .ToList();
                        var list = lists.SingleOrDefault(l => l.Name == listName);

                        // create list if it does not exist
                        if (list == null)
                        {
                            list = LinqToTwitter.ListExtensions.CreateList(twitterContext, listName, "private", "Network:" + this.Services.NetworkId);
                        }

                        int tentatives = 0, maxTentatives = 2;
                        do
                        {
                            tentatives++;
                            // add user to list
                            var result = LinqToTwitter.ListExtensions.AddMemberToList(twitterContext, tokens.UserId.ToString(), null, null, listName, null, socialState.Entity.Username);

                            // check list
                            var listsMembers = twitterContext.List
                                .Where(l => l.Type == LinqToTwitter.ListType.Members
                                         && l.Slug == listName && l.OwnerScreenName == socialState.Entity.Username)
                                .ToList();
                            var listMembers = listsMembers.Single();
                            var listMember = listMembers.Users.SingleOrDefault(m => m.Identifier.ID == tokens.UserId.ToString());
                            if (listMember != null)
                                break;
                        } while (tentatives <= maxTentatives);

                        this.TempData.AddInfo("Votre compte Twitter est maintenant configuré.");
                    }
                    catch (Exception ex)
                    {
                        this.TempData.AddError(Lang.T("Une erreur de communication avec le service Twitter est survenue. Ce problème est peut-être temporaire, réessayez-donc plus tard."));
                        this.ReportError(ex, "AccountController/ConfigureTwitter/add-to-twitter-network-list error");
                        return this.RedirectToAction("SocialNetworks");
                    }
                }
            }

            return RedirectToAction("SocialNetworks");
        }

        [AuthorizeUser]
        public ActionResult SocialNetworks(int? id)
        {
            if ((id.HasValue && this.SessionService.User.NetworkAccess.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff)) || !id.HasValue)
            {
                Sparkle.Services.Networks.Models.UserModel user = null;
                if (id.HasValue && (user = this.Services.People.GetActiveById(id.Value, Data.Options.PersonOptions.None)) == null)
                    return this.ResultService.NotFound();

                SocialNetworksModel model = new SocialNetworksModel();

                var states = this.Services.SocialNetworkStates.GetAllIncludingUnconfigured();
                var twitterState = states.SingleOrDefault(s => s.Type == SocialNetworkConnectionType.Twitter);
                model.IsTwitterAvailable = twitterState != null && twitterState.Entity != null && twitterState.Entity.IsConfigured;
                var facebookState = states.SingleOrDefault(s => s.Type == SocialNetworkConnectionType.Facebook);
                model.IsFacebookAvailable = facebookState != null && facebookState.Entity != null && facebookState.Entity.IsConfigured;

                var subscriptions = this.Services.SocialNetworkUserSubscriptions.GetByUserId(user != null ? user.Id : this.UserId.Value);

                foreach (var subscription in subscriptions)
                {
                    switch (subscription.SocialNetworkConnection.Type)
                    {
                        case (byte)SocialNetworkConnectionType.Twitter:

                            model.Twitter.IsConnected = true;
                            model.Twitter.Username = subscription.SocialNetworkConnection.Username;
                            model.Twitter.Hashtag = subscription.ContentContainsFilter;

                            break;
                        case (byte)SocialNetworkConnectionType.Facebook:

                            model.Facebook.IsConnected = true;

                            break;
                        default:
                            break;
                    }
                }

                this.ViewBag.UserFullname = user != null ? user.DisplayName : "";
                model.UserId = id;
                return this.View(model);
            }

            return this.ResultService.Forbidden();
        }

        [HttpPost]
        [AuthorizeUser]
        public ActionResult SocialNetworks(SocialNetworksModel model)
        {
            if ((model.UserId.HasValue && this.SessionService.User.NetworkAccess.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff)) || !model.UserId.HasValue)
            {
                Sparkle.Services.Networks.Models.UserModel user = null;
                if (model.UserId.HasValue && (user = this.Services.People.GetActiveById(model.UserId.Value, Data.Options.PersonOptions.None)) == null)
                    return this.ResultService.NotFound();

                if (!string.IsNullOrEmpty(model.Twitter.AdminInput))
                    this.AddUserTwitterLink(user.Id, model.Twitter.AdminInput);

                var subscriptions = this.Services.SocialNetworkUserSubscriptions.GetByUserId(user != null ? user.Id : this.UserId.Value);

                foreach (var subscription in subscriptions)
                {
                    switch (subscription.SocialNetworkConnection.Type)
                    {
                        case (byte)SocialNetworkConnectionType.Twitter:

                            if (model.Twitter.ToDelete)
                            {
                                this.DeleteUserTwitterLink(subscription);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(model.Twitter.Hashtag) && !string.IsNullOrEmpty(subscription.ContentContainsFilter))
                                {
                                    // Remove hashtag
                                    subscription.ContentContainsFilter = null;
                                    this.Services.SocialNetworkUserSubscriptions.Update(subscription);
                                }
                                else if (!string.IsNullOrEmpty(model.Twitter.Hashtag) && string.IsNullOrEmpty(subscription.ContentContainsFilter))
                                {
                                    // Add hashtag
                                    subscription.ContentContainsFilter = model.Twitter.Hashtag.Replace("#", "");
                                    this.Services.SocialNetworkUserSubscriptions.Update(subscription);
                                }

                                model.Twitter.Username = subscription.SocialNetworkConnection.Username;
                                model.Twitter.IsConnected = true;
                            }

                            break;
                        case (byte)SocialNetworkConnectionType.Facebook:

                            model.Facebook.IsConnected = true;

                            break;
                        default:
                            break;
                    }
                }

                this.ViewBag.UserFullname = user != null ? user.DisplayName : "";
                if (model.UserId.HasValue)
                    return this.RedirectToAction("SocialNetworks", new { id = model.UserId.Value, });
                return this.RedirectToAction("SocialNetworks");
            }

            return this.ResultService.Forbidden();
        }

        private void AddUserTwitterLink(int userId, string username)
        {
            // verify configuration
            var socialState = this.Services.SocialNetworkStates.GetState(SocialNetworkConnectionType.Twitter);
            if (socialState.Entity == null || !socialState.Entity.IsConfigured)
                return;

            var twitterCredentials = new LinqToTwitter.InMemoryCredentials
            {
                ConsumerKey = this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerKey,
                ConsumerSecret = this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerSecret,
                OAuthToken = socialState.Entity.OAuthAccessToken,
                AccessToken = socialState.Entity.OAuthAccessSecret,
            };
            var authorizer = new LinqToTwitter.SingleUserAuthorizer
            {
                Credentials = twitterCredentials,
            };
            var twitterContext = new LinqToTwitter.TwitterContext(authorizer);

            try
            {
                // find list
                var listName = this.Services.SocialNetworkStates.GetTwitterFollowListName(this.Services.Network);
                var lists = twitterContext.List
                    .Where(l => l.Type == LinqToTwitter.ListType.Lists && l.ScreenName == socialState.Entity.Username)
                    .ToList();
                var list = lists.SingleOrDefault(l => l.Name == listName);

                // create list if it does not exist
                if (list == null)
                {
                    list = LinqToTwitter.ListExtensions.CreateList(twitterContext, listName, "private", "Network:" + this.Services.NetworkId);
                }

                // add user to list
                var result = LinqToTwitter.ListExtensions.AddMemberToList(twitterContext, null, username, null, listName, null, socialState.Entity.Username);
                this.TempData.AddInfo(Lang.T("Votre compte Twitter est maintenant configuré."));
            }
            catch (Exception ex)
            {
                if (ex is LinqToTwitter.TwitterQueryException)
                {
                    var twitterEx = (LinqToTwitter.TwitterQueryException)ex;

                    // can't find user
                    if (twitterEx.ErrorCode == 108)
                        this.TempData.AddError(Lang.T("Le compte twitter que vous avez renseigné est introuvable, veuillez vérifier qu'il s'agit d'un compte valide et public et réessayez."));
                    return;
                }
                this.TempData.AddError(Lang.T("Une erreur de communication avec le service Twitter est survenue. Ce problème est peut-être temporaire, réessayez-donc plus tard."));
                this.ReportError(ex, "AccountController/SocialNetworks/add-to-twitter-network-list error");
                return;
            }

            this.Services.SocialNetworkUserSubscriptions.CreateConnection(this.SessionService.User.Id, userId, SocialNetworkConnectionType.Twitter, username);
        }

        private void DeleteUserTwitterLink(SocialNetworkUserSubscription subscription)
        {
            var stillInUse = this.Services.SocialNetworkConnections.CountByUsernameAndType(subscription.SocialNetworkConnection.Username, SocialNetworkConnectionType.Twitter) > 1;

            // delete from twitter list
            if (!stillInUse)
            {
                // verify configuration
                var socialState = this.Services.SocialNetworkStates.GetState(SocialNetworkConnectionType.Twitter);
                if (socialState.Entity == null || !socialState.Entity.IsConfigured)
                    return;

                var twitterCredentials = new LinqToTwitter.InMemoryCredentials
                {
                    ConsumerKey = this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerKey,
                    ConsumerSecret = this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerSecret,
                    OAuthToken = socialState.Entity.OAuthAccessToken,
                    AccessToken = socialState.Entity.OAuthAccessSecret,
                };
                var authorizer = new LinqToTwitter.SingleUserAuthorizer
                {
                    Credentials = twitterCredentials,
                };
                var twitterContext = new LinqToTwitter.TwitterContext(authorizer);

                try
                {
                    // find list
                    var listName = this.Services.SocialNetworkStates.GetTwitterFollowListName(this.Services.Network);
                    var lists = twitterContext.List
                        .Where(l => l.Type == LinqToTwitter.ListType.Lists && l.ScreenName == socialState.Entity.Username)
                        .ToList();
                    var list = lists.SingleOrDefault(l => l.Name == listName);

                    var membersList = twitterContext.List
                        .Where(l => l.Type == LinqToTwitter.ListType.Members && l.Slug == listName && l.OwnerScreenName == socialState.Entity.Username)
                        .ToList();
                    var members = membersList.SingleOrDefault(l => l.Slug == listName);

                    // if list does not exist, abort
                    if (list == null)
                        return;

                    if (members.Users.Any(o => o.Identifier.ScreenName == subscription.SocialNetworkConnection.Username))
                    {
                        // delete user from list
                        var result = LinqToTwitter.ListExtensions.DeleteMemberFromList(twitterContext, null, subscription.SocialNetworkConnection.Username, null, listName, null, socialState.Entity.Username);
                    }
                }
                catch (Exception ex)
                {
                    this.TempData.AddError(Lang.T("Une erreur de communication avec le service Twitter est survenue. Ce problème est peut-être temporaire, réessayez-donc plus tard."));
                    this.ReportError(ex, "AccountController/SocialNetworks/remove-from-twitter-network-list error");
                    return;
                }
            }

            // delete entities
            var connection = subscription.SocialNetworkConnection;
            this.Services.Logger.Info("AccountController.SocialNetworks[POST]", ErrorLevel.Success, "Deleting of SocialNetworkUserSubscriptions #" + subscription.Id + " and SocialNetworkConnection #" + connection.Id + " from user #" + subscription.UserId);
            this.Services.SocialNetworkUserSubscriptions.Delete(subscription);
            this.Services.SocialNetworkConnections.Delete(connection);
        }

        [AuthorizeUser]
        public ActionResult Twitter()
        {
            var model = new AccountTwitterModel();
            var settings = this.Services.UserSettings.GetByUserIdAndKey(this.UserId.Value, "Twitter");
            if (settings != null)
            {
                var twitterSetting = settings.Value.Split(';');
                if (twitterSetting.Length == 3)
                {
                    model.Username = twitterSetting[0];
                    model.OAuthToken = twitterSetting[1];
                    model.OAuthVerifier = twitterSetting[2];
                    model.IsConnected = true;
                }

            }

            return View(model);
        }

        [AuthorizeUser]
        public ActionResult NeedToCompleteYourProfile()
        {
            this.TempData.AddWarning("Soyez Fairplay ! Complétez votre profil avant de parcourir ceux des autres :)");

            return RedirectToAction("Profile");
        }

        [AuthorizeUser]
        public ActionResult UserMenu()
        {
            UserMenu model = new UserMenu();
            var user = this.SessionService.User;

            // user stuff
            model.MessagesCount = this.Services.PrivateMessage.CountUnread(user.Id);
            model.MessagesText = Lang.M("{0} message non lu", "{0} messages non lus", model.MessagesCount);

            model.RequestsCount = this.Services.SeekFriends.CountPendingRequests(this.UserId.Value);
            model.RequestsText = Lang.M("{0} demande de contact", "{0} demandes de contact", model.RequestsCount);

            var items = this.Services.Activities.GetUsersNotifications(this.UserId.Value, false, int.MaxValue, 0);
            model.NotificationsCount = items.Where(n => n.Displayed == false).Count();
            model.NotificationsText = Lang.M("{0} notification", "{0} notifications", model.NotificationsCount);

            // company stuff
            model.CompanyManagementCount = this.Services.People.CountMustBeValidateUsersByCompanyId(user.CompanyID)
                + this.Services.RegisterRequests.CountPendingByCompany(user.CompanyID);
            model.CompanyManagementText = Lang.M("{0} demande d'inscription", "{0} demandes d'inscription", model.CompanyManagementCount);

            model.NewAdsCount = this.Services.Ads.CountNewAdsForUser(user.Id);
            model.NewAdsText = Lang.M("{0} new ad", "{0} new ads", model.NewAdsCount);

            return this.ResultService.JsonSuccess(model);
        }

        [AuthorizeUser]
        public ActionResult Invitations(string id, int? CompanyRelationshipTypeId)
        {
            var model = new InvitationsModel
            {
                ////MyCompanyId = this.SessionService.User.CompanyID,
                ////Companies = this.Services.Company.SelectAll()
                ApplyInvitationLink = this.Services.People.GetInviteWithApplyUrl(this.UserId.Value, this.Services.NetworkId),
                LinkedInContactsImport = this.SessionService.LinkedInContactsImport,
                CompanyCategories = this.Services.Company.GetAllCategories(),
                CompanyRelationshipTypes = this.Services.CompanyRelationships.GetNonSystemTypes(),
            };

            var currentUser = this.SessionService.User;
            var companyName = this.SessionService.User.Company.Name;
            var companyCategory = this.Services.Company.GetCategoryById(this.SessionService.User.Company.CategoryId);
            bool isCompanyAccelerator = companyCategory != null && companyCategory.KnownCategory == Sparkle.Entities.Networks.KnownCompanyCategory.CompanyAccelerator;
            bool isAdmin = this.SessionService.User.NetworkAccess.HasAnyFlag(NetworkAccessLevel.AddCompany, NetworkAccessLevel.ManageCompany, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff);

            var title = Lang.T("Inviter des personnes");
            SparkleAccentColor accentColor = SparkleAccentColor.Account;

            if (!string.IsNullOrEmpty(id))
            {
                switch (id.ToLower())
                {
                    case "people":
                        title = Lang.T("Inviter des personnes");
                        break;
                    case "companies":
                        title = Lang.T("Inviter des entreprises");
                        accentColor = SparkleAccentColor.Companies;
                        if (isCompanyAccelerator)
                        {
                            model.DisplayCompanyCategoriesChoice = true;
                        }
                        break;
                    case "team":
                        title = string.Format(Lang.T("Inviter des collègues de {0}"), companyName);
                        model.DisplayPersonalizedInvitationLink = false;
                        model.PreselectedNoNeedApproval = true;
                        break;
                    case "ecosystem":
                        if (isCompanyAccelerator)
                        {
                            title = string.Format(Lang.T("Inviter les entreprises de {0}"), companyName);
                            accentColor = SparkleAccentColor.Companies;
                            model.DisplayPersonalizedInvitationLink = false;
                            model.PreselectedNoNeedApproval = true;

                            // TODO: this is a custom check for spark
                            var defaultRelationship = Lang.T("AccelerateCompanyRelationshipTypeId");
                            if (!string.IsNullOrWhiteSpace(defaultRelationship))
                            {
                                int preselectedCompanyRelationshipTypeId = 0;
                                if (int.TryParse(defaultRelationship, out preselectedCompanyRelationshipTypeId))
                                {
                                    model.PreselectedCompanyRelationshipTypeId = preselectedCompanyRelationshipTypeId;
                                }
                            }
                        }
                        break;
                    case "others":
                        if (isCompanyAccelerator)
                        {
                            title = string.Format(Lang.T("Inviter des entreprises hors de {0}"), companyName);
                            accentColor = SparkleAccentColor.Companies;
                        }
                        break;
                    case "admin":
                        if (isAdmin)
                        {
                            title = Lang.T("Inviter des entreprises");
                            accentColor = SparkleAccentColor.Companies;
                            model.DisplayAdminControls = true;
                        }
                        break;
                    default:
                        break;
                }
            }

            this.ViewBag.IsAdmin = isAdmin;
            if (CompanyRelationshipTypeId.HasValue && model.CompanyRelationshipTypes.Any(o => o.Id == CompanyRelationshipTypeId.Value))
                this.ViewBag.RelationshipSelected = CompanyRelationshipTypeId.Value;
            this.ViewBag.Title = title;
            this.ViewBag.AccentColor = accentColor;
            this.ViewBag.CompanyName = companyName;
            this.ViewBag.IsCompanyAccelerator = isCompanyAccelerator;
            this.ViewBag.Category = companyCategory;

            return View(model);
        }

        [AuthorizeByNetworkAccess(NetworkAccessLevel.SparkleStaff | NetworkAccessLevel.NetworkAdmin)] // this does not work :(
        public ActionResult Roles(string id)
        {
            if (id == null)
                return this.RedirectToAction("Roles", new { id = this.SessionService.User.Login, });

            var user = this.Services.People.SelectWithLogin(id);
            if (user == null)
            {
                return this.ResultService.NotFound();
            }
            else if (user.Id != this.UserId.Value)
            {
                if (!this.SessionService.User.NetworkAccess.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff))
                {
                    return this.ResultService.Forbidden();
                }
            }

            var model = this.Services.People.GetRolesFormModel(user, this.UserId);

            return this.View(model);
        }

        [HttpPost]
        [AuthorizeByNetworkAccess(NetworkAccessLevel.SparkleStaff | NetworkAccessLevel.NetworkAdmin)]
        public ActionResult Roles(Sparkle.Services.Networks.Models.UserRoleFormModel model, string returnUrl)
        {
            model.User = this.Services.People.SelectWithId(model.UserId);
            if (model.User == null)
            {
                return this.ResultService.NotFound();
            }
            else if (model.User.Id != this.UserId.Value)
            {
                if (!this.SessionService.User.NetworkAccess.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff))
                {
                    return this.ResultService.Forbidden();
                }
            }

            if (ModelState.IsValid)
            {
                var currentUser = this.Services.People.SelectWithId(this.UserId.Value);
                if ((currentUser.NetworkAccess & NetworkAccessLevel.SparkleStaff) != NetworkAccessLevel.SparkleStaff)
                    model.SparkleStaff = false;
                this.Services.People.UpdateUserRolesFromModel(model);
                this.TempData.AddInfo("Les changements sont enregistrés !");
            }

            model = this.Services.People.GetRolesFormModel(model.User, this.UserId);

            return this.View(model);
        }

        [AuthorizeUser]
        public ActionResult ChangeCulture(string CultureName, string ReturnUrl)
        {
            var availiableCultures = this.Services.AppConfiguration.Tree.Features.I18N.AvailableCultures.Split(';');
            if (!availiableCultures.Contains(CultureName))
            {
                this.TempData.AddError(Alerts.CultureNotAvailiable);
                return this.RedirectToLocal(ReturnUrl);
            }

            var culture = new CultureInfo(CultureName);
            var result = this.Services.People.ChangeUserCulture(new ChangeUserCultureRequest(this.UserId, culture));
            if (!result.Succeed)
            {
                var errorMessage = result.Errors.First() != null ? result.Errors.First().DisplayMessage : Lang.T("Une erreur inconnue est survenue.");
                this.TempData.AddError(errorMessage);

                return this.RedirectToLocal(ReturnUrl);
            }

            this.SessionService.Clear();
            this.SessionService.ReviveIfDead(() => this.Services, this.HttpContext);
            return this.RedirectToLocal(ReturnUrl);
        }

        public ActionResult ChangeCultureAnonymous(string CultureName, string ReturnUrl)
        {
            var availiableCultures = this.Services.AppConfiguration.Tree.Features.I18N.AvailableCultures.Split(';');
            if (!availiableCultures.Contains(CultureName))
            {
                this.TempData.AddError(Alerts.CultureNotAvailiable);
                return this.RedirectToLocal(ReturnUrl);
            }

            var culture = new CultureInfo(CultureName);
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            CultureTools.SaveClientCulture(this.HttpContext.ApplicationInstance.Context, culture);

            return this.RedirectToLocal(ReturnUrl);
        }

        [AuthorizeUser]
        public ActionResult LinkedInConnect(string ReturnUrl)
        {
            var returnUrl = UrlTools.Compose(this.Request) + "/Account/LinkedInConnectReturn?ReturnUrl=" + Uri.EscapeDataString(ReturnUrl);
            var failReturnUrl = UrlTools.Compose(this.Request) + "/Account/LinkedInConnectReturn?error=IdNotFound&errorDesc=The+redirect+id+was+not+found";
            string goUrl;

            // Init LinkedIn
            bool success;
            var initResult = this.InitLinkedIn(this.Services, ReturnUrl, returnUrl, (int)ApplyController.linkedInScope, out goUrl, out success);
            if (!success)
                return initResult;

            var userConnection = this.Services.SocialNetworkConnections.GetByUserIdAndConnectionType(this.UserId.Value, SocialNetworkConnectionType.LinkedIn);
            if (userConnection != null &&
                !string.IsNullOrEmpty(userConnection.OAuthToken) &&
                userConnection.OAuthTokenDateUtc.HasValue &&
                userConnection.OAuthTokenDurationMinutes.HasValue)
            {
                var expireDate = userConnection.OAuthTokenDateUtc.Value.AddMinutes(userConnection.OAuthTokenDurationMinutes.Value);
                if (expireDate > DateTime.UtcNow)
                    return this.RedirectToLocal(ReturnUrl);
            }

            return this.Redirect(goUrl + "&failReturnUrl=" + Uri.EscapeDataString(failReturnUrl));
        }

        [AuthorizeUser]
        public ActionResult LinkedInConnectReturn(string code, Guid? redirId, string error, string errorDesc, string ReturnUrl)
        {
            AuthorizationAccessToken token;
            bool success;
            var result = this.GetLinkedInToken(this.Services, code, redirId, error, errorDesc, ReturnUrl, out token, out success);
            if (!success)
                return result;

            var userCo = this.Services.SocialNetworkConnections.GetByUserIdAndConnectionType(this.UserId.Value, SocialNetworkConnectionType.LinkedIn);
            if (userCo != null)
            {
                userCo.OAuthVerifier = code;
                userCo.OAuthToken = token.AccessToken;
                userCo.OAuthTokenDateUtc = DateTime.UtcNow;
                userCo.OAuthTokenDurationMinutes = token.ExpiresIn.Value / 60;
                userCo.CreatedByUserId = this.UserId.Value;
                userCo.SocialNetworkConnectionType = SocialNetworkConnectionType.LinkedIn;
                userCo.Username = "";

                this.Services.SocialNetworkConnections.Update(userCo);
            }
            else
            {
                userCo = new SocialNetworkConnection();
                userCo.OAuthVerifier = code;
                userCo.OAuthToken = token.AccessToken;
                userCo.OAuthTokenDateUtc = DateTime.UtcNow;
                userCo.OAuthTokenDurationMinutes = token.ExpiresIn.Value / 60;
                userCo.CreatedByUserId = this.UserId.Value;
                userCo.SocialNetworkConnectionType = SocialNetworkConnectionType.LinkedIn;
                userCo.Username = "";

                this.Services.SocialNetworkConnections.Insert(userCo);
            }

            this.TempData.AddInfo(Alerts.LinkedInConnectSuccess);
            return this.RedirectToLocal(ReturnUrl);
        }

        [AuthorizeUser]
        public ActionResult Subscribe()
        {
            this.ViewBag.Title = Lang.T("S'abonner");

            var available = this.Services.SubscriptionTemplates.GetUserSubscribable();

            return this.View("~/Views/Account/Subscribe.cshtml", available);
        }

        [AuthorizeUser]
        public ActionResult Subscriptions()
        {
            var items = this.Services.Subscriptions.GetByAppliedUser(this.UserId.Value);
            return this.View(items);
        }

        [AuthorizeUser]
        public ActionResult Region(string ReturnUrl)
        {
            var model = this.Services.People.GetRegionSettingsRequest(this.UserId.Value);
            model.ReturnUrl = ReturnUrl;

            return this.View(model);
        }

        [HttpPost]
        [AuthorizeUser]
        public ActionResult Region(RegionSettingsRequest model)
        {
            if (this.ModelState.IsValid)
            {
                var result = this.Services.People.SaveRegionSettings(model, this.UserId.Value);
                if (!this.ValidateResult(result, MessageDisplayMode.TempData))
                {
                    this.Services.People.FillRegionSettingsRequestLists(model);
                    return this.View(model);
                }

                this.SessionService.Clear();
                this.SessionService.ReviveIfDead(() => this.Services, this.HttpContext);

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return this.Redirect(model.ReturnUrl);
                return this.RedirectToAction("Index");
            }

            this.Services.People.FillRegionSettingsRequestLists(model);
            return this.View(model);
        }

        public ActionResult Me()
        {
            var user = this.Services.People.GetById(this.UserId.Value, Data.Options.PersonOptions.Company);
            
            if (user == null)
            {
                return this.ResultService.NotFound();
            }

            return this.RedirectToAction("People", "Peoples", new { id = user.Login, });
        }
    }

    public class AccountResult
    {
        public bool Success { get; set; }

        public InvitePersonResult.ResultCode? InviteCode { get; set; }
    }

    public class UserMenu
    {
        public int MessagesCount { get; set; }
        public string MessagesText { get; set; }

        public int RequestsCount { get; set; }
        public string RequestsText { get; set; }

        public int NotificationsCount { get; set; }
        public string NotificationsText { get; set; }

        public int CompanyManagementCount { get; set; }

        public string CompanyManagementText { get; set; }

        public int NewAdsCount { get; set; }
        public string NewAdsText { get; set; }
    }
}
