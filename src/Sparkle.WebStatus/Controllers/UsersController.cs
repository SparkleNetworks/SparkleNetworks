
namespace Sparkle.WebStatus.Controllers
{
    using Sparkle.NetworksStatus.Domain.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using SrkToolkit.Web;

    [Authorize]
    public class UsersController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.NavigationLine().Add("Users", this.Url.Action("Index"));

            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index(int page = 0, int pageSize = 100)
        {
            var model = this.Domain.Users.GetAllSortedById(page, pageSize);
            this.ViewBag.Count = this.Domain.Users.Count();

            return this.View(model);
        }

        public ActionResult Details(int id)
        {
            this.NavigationLine().Add(id.ToString(), this.Url.Action("Details", new { id = id, }));

            var model = this.Domain.Users.GetById(id, withPrimaryEmail: true, withEmailAuths: true);
            model.EmailAddressAuthentications = this.Domain.Users.GetEmailAddressAuthentications(model.Id);

            return this.View(model);
        }

        public ActionResult Edit(int id, UserModel model)
        {
            if (id == 0)
            {
                if (model == null)
                    model = new UserModel();

                this.NavigationLine().Add("Create", this.Url.Action("Edit", new { id = id, }));
                this.ViewBag.Title = "Create user";
            }
            else
            {
                model = this.Domain.Users.GetById(id, withPrimaryEmail: true, withEmailAuths: true);
                this.NavigationLine().Add(id.ToString(), this.Url.Action("Details", new { id = id, }));
                this.NavigationLine().Add("Edit", this.Url.Action("Edit", new { id = id, }));
                this.ViewBag.Title = "Edit user";
            }

            if (this.Request.HttpMethod == "POST" && this.ModelState.IsValid)
            {
                var result = this.Domain.Users.Create(model);
                if (this.ValidateResult(result, MessageDisplayMode.ModelState))
                {
                    return this.RedirectToAction("Details", new { id = result.Data.Id, });
                }
            }

            return this.View(model);
        }

        public ActionResult EditEmailAddressAuthentication(int id, int? UserId)
        {
            this.NavigationLine().Add(UserId.ToString(), this.Url.Action("Details", new { id = UserId, }));
            EmailAddressAuthenticationModel model = null;
            if (id == 0)
            {
                if (model == null)
                    model = new EmailAddressAuthenticationModel();

                model.UserId = UserId.Value;

                this.NavigationLine().Add("Create email authentication", this.Url.Action("EditEmailAddressAuthentication", new { id = id, UserId = UserId, }));
                this.ViewBag.Title = "Create email authentication";
            }
            else
            {
                model = this.Domain.Users.GetEmailAddressAuthenticationById(id);
                this.NavigationLine().Add(id.ToString(), this.Url.Action("Details", new { id = id, }));
                this.NavigationLine().Add("Edit", this.Url.Action("Edit", new { id = id, }));
                this.ViewBag.Title = "Edit email authentication";
            }

            return this.View(model);
        }

        [HttpPost]
        public ActionResult EditEmailAddressAuthentication(int id, EmailAddressAuthenticationModel model)
        {
            if (model == null)
                return this.ResultService.NotFound();

            this.NavigationLine().Add(model.UserId.ToString(), this.Url.Action("Details", new { id = model.UserId, }));
            if (model.Id == 0)
            {
                if (model == null)
                    model = new EmailAddressAuthenticationModel();

                this.NavigationLine().Add("Create email authentication", this.Url.Action("EditEmailAddressAuthentication", new { id = id, UserId = model.UserId, }));
                this.ViewBag.Title = "Create email authentication";
            }
            else
            {
                this.NavigationLine().Add(id.ToString(), this.Url.Action("Details", new { id = id, }));
                this.NavigationLine().Add("Edit", this.Url.Action("Edit", new { id = id, }));
                this.ViewBag.Title = "Edit email authentication";
            }

            if (this.ModelState.IsValid)
            {
                if (id == 0)
                {
                    var result = this.Domain.Users.CreateEmailAddressAuthentications(model);
                    if (this.ValidateResult(result, MessageDisplayMode.TempData))
                    {
                        this.TempData.AddConfirmation("created");
                        return this.RedirectToAction("Details", new { id = model.UserId, });
                    }
                }
                else
                {
                    var result = this.Domain.Users.EditEmailAddressAuthentications(model);
                    if (this.ValidateResult(result, MessageDisplayMode.TempData))
                    {
                        this.TempData.AddConfirmation("updated");
                        return this.RedirectToAction("Details", new { id = model.UserId, });
                    }
                }
            }

            return this.View(model);
        }
    }
}