
namespace Sparkle.Controllers
{
    using Sparkle.Entities.Networks;
    using Sparkle.Filters;
    using Sparkle.Models;
    using Sparkle.Resources;
    using Sparkle.Services.Networks.Ads;
    using Sparkle.Services.Networks.Models;
    using Sparkle.WebBase;
    using SrkToolkit.Web;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// The Ads feature.
    /// </summary>
    [AuthorizeUser]
    public class AdsController : LocalSparkleController
    {
        private bool? isModerator;
        private bool? isAdmin;

        public ActionResult Index()
        {
            return this.RedirectToAction("List");

            return this.View();
        }

        public ActionResult List(bool? ShowAll, int offset = 0, int pageSize = 100)
        {
            if (pageSize > 100)
                pageSize = 100;

            var model = new AdsModel();

            var user = this.SessionService.User;

            bool openOnly = !ShowAll ?? true;
            model.Items = this.Services.Ads.GetList(Ad.Columns.UpdateDateUtc, true, openOnly, offset, pageSize, AdOptions.Owner);
            this.Services.People.Refresh(model.Items.Select(x => x.Owner), refreshPictureUrl: true);
            model.ItemsTotal = this.Services.Ads.Count(openOnly);
            model.Offset = offset;
            model.PageSize = pageSize;

            model.ShowAll = !openOnly;
            model.Types = this.Services.Ads.GetTypes();

            var maxSeenDateHint = this.Services.Hints.GetUserRelation(KnownHints.MaxAdSeenDateKey, user.Id);
            var maxSeenDate = maxSeenDateHint != null ? maxSeenDateHint.DateDismissedUtc : default(DateTime?);
            if (maxSeenDate != null)
            {
                foreach (var item in model.Items)
                {
                    var itemDate = item.UpdateDateUtc ?? item.Date;
                    item.IsNewForUser = itemDate > maxSeenDate.Value;
                }
            }

            if (maxSeenDateHint != null)
            {
                if (maxSeenDate != null)
                {
                    if (DateTime.UtcNow.Subtract(maxSeenDate.Value).TotalMinutes > 3)
                    {
                        this.Services.Hints.SetUserRelation(maxSeenDateHint.HintId, user.Id, DateTime.UtcNow);
                    }
                }
                else
                {
                    this.Services.Hints.SetUserRelation(maxSeenDateHint.HintId, user.Id, DateTime.UtcNow);
                }
            }

            return this.View(model);
        }

        public ActionResult ById(int id)
        {
            var item = this.Services.Ads.GetById(id, AdOptions.None);
            if (item != null)
                return this.RedirectToAction("Details", new { id = item.Alias, });
            else
                return this.ResultService.NotFound();
        }

        public ActionResult Details(string id)
        {
            var item = this.Services.Ads.GetByAlias(id, AdOptions.None);
            if (item == null)
                return this.ResultService.NotFound();

            var user = this.SessionService.User;
            var owner = this.Services.People.GetById(item.UserId, Data.Options.PersonOptions.Company);
            this.Services.People.Refresh(owner, refreshPictureUrl: true);
            item.Owner = owner;

            if (item.IsValidated == false)
            {
                if (!this.IsModerator() && owner.Id != user.Id)
                {
                    return this.ResultService.Forbidden(this.Services.Lang.T("This ad has been refused."));
                }
            }

            var requiresValidationConfig = this.Services.AppConfiguration.Tree.Features.Ads.RequireValidation;
            var requiresValidation = item.IsValidated == null && requiresValidationConfig;
            if (requiresValidation)
            {
                this.ViewBag.IsPendingValidation = true;
                if (this.IsModerator() || item.UserId == user.Id)
                {
                }
                else
                {
                    return this.ResultService.Forbidden(this.Services.Lang.T("This ad is pending validation."));
                }
            }
            else
            {
                this.ViewBag.IsPendingValidation = false;
            }

            bool isPendingEdit = item.PendingEditDate != null;
            this.ViewBag.IsPendingEdit = isPendingEdit;

            bool canEdit = (this.IsModerator() || item.UserId == user.Id) && item.IsValidated != false;
            this.ViewBag.CanEdit = canEdit;

            return this.View(item);
        }

        public ActionResult Edit(int? id, int? TypeId)
        {
            var request = this.Services.Ads.GetEditRequest(id, null);

            if (id != null)
            {
                var item = this.Services.Ads.GetById(id.Value, AdOptions.None);
                if (item == null)
                {
                    return this.ResultService.NotFound();
                }
                else if (this.IsAdmin() || this.IsModerator())
                {
                }
                else if (item.UserId == this.SessionService.User.Id)
                {
                }
                else
                {
                    return this.ResultService.Forbidden();
                }
            }
            else
            {
                if (TypeId != null)
                {
                    request.TypeId = TypeId.Value;
                }
            }

            return this.View(request);
        }

        [HttpPost]
        public ActionResult Edit(EditAdRequest request)
        {
            int id = request.Id;

            if (id > 0)
            {
                var item = this.Services.Ads.GetById(id, AdOptions.None);
                if (item == null)
                {
                    return this.ResultService.NotFound();
                }
                else if (this.IsAdmin() || this.IsModerator())
                {
                }
                else if (item.UserId == this.SessionService.User.Id)
                {
                }
                else
                {
                    return this.ResultService.Forbidden();
                }
            }

            if (this.ModelState.IsValid)
            {
                request.ActingUserId = this.SessionService.User.Id;
                var result = this.Services.Ads.Edit(request);
                if (this.ValidateResult(result, MessageDisplayMode.TempData))
                {
                    if (result.IsPendingValidation)
                    {
                        this.TempData.AddConfirmation(this.Services.Lang.T("Ad is now pending administrator validation."));
                    }
                    else if (result.IsPendingEdit)
                    {
                        this.TempData.AddConfirmation(this.Services.Lang.T("Ad is now pending administrator validation."));
                    }
                    else
                    {
                        this.TempData.AddConfirmation(this.Services.Lang.T("Ad saved."));
                    }

                    return this.RedirectToAction("Details", new { id = result.Item.Alias, });
                }
            }

            request = this.Services.Ads.GetEditRequest(null, request);
            return this.View(request);
        }

        public ActionResult Validate(int id, string Do, string PendingEditDate, string ReturnUrl)
        {
            var user = this.SessionService.User;
            var item = this.Services.Ads.GetById(id, AdOptions.None);
            if (item == null)
                return this.ResultService.NotFound();

            DateTime pendingDate;
            DateTime.TryParseExact(PendingEditDate, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out pendingDate);
            bool? accept = null;
            if ("Accept".Equals(Do, StringComparison.OrdinalIgnoreCase))
            {
                accept = true;
            }
            else if ("Refuse".Equals(Do, StringComparison.OrdinalIgnoreCase))
            {
                accept = false;
            }

            if (accept != null)
            {
                var request = new ValidateAdRequest();
                request.ActingUserId = user.Id;
                request.Id = item.Id;
                request.Accept = accept.Value;
                request.PendingEditDate = pendingDate;

                var result = this.Services.Ads.Validate(request);
                if (this.ValidateResult(result, MessageDisplayMode.TempData))
                {
                    if (result.Item.IsValidated != null)
                    {
                        if (result.Item.IsValidated.Value)
                        {
                            this.TempData.AddConfirmation(this.Services.Lang.T("This ad is now validated."));
                        }
                        else
                        {
                            this.TempData.AddConfirmation(this.Services.Lang.T("This ad has been refused."));
                        }
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                this.TempData.AddError(Alerts.InvalidArguments);
            }

            return this.RedirectToLocal(ReturnUrl, this.Url.Action("Details", new { id = item.Alias, }));
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            this.ViewBag.IsModerator = this.IsModerator();
            this.ViewBag.IsAdmin = this.IsAdmin();

            base.OnResultExecuting(filterContext);
        }

        private bool IsModerator()
        {
            if (this.isModerator != null)
                return this.isModerator.Value;

            this.ComputeFlags();
            return this.isModerator.Value;
        }

        private bool IsAdmin()
        {
            if (this.isAdmin != null)
                return this.isAdmin.Value;

            this.ComputeFlags();
            return this.isAdmin.Value;
        }

        private void ComputeFlags()
        {
            var user = this.SessionService.User;
            if (user != null)
            {
                this.isAdmin = user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff);
                this.isModerator = this.isAdmin.Value || user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.ModerateNetwork);
            }
            else
            {
                this.isModerator = false;
                this.isAdmin = false;
            }
        }
    }
}
