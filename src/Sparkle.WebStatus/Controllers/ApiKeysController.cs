
namespace Sparkle.WebStatus.Controllers
{
    using Sparkle.WebStatus.Domain;
    using SrkToolkit.Web;
    using System;
    using System.Linq;
    using System.Web.Mvc;

    [Authorize]
    public class ApiKeysController : Controller
    {
        public ActionResult Index()
        {
            var items = this.Services.ApiKeys.GetAll().OrderBy(n => n.Name).ToList();
            return this.View(items);
        }

        public ActionResult Edit(ApiKeyModel request, Guid? id)
        {
            ApiKeyModel subject = null;

            if (id != null)
            {
                subject = this.Services.ApiKeys.Get(id.Value);
                if (subject == null)
                    return this.ResultService.NotFound();

                this.NavigationLine().Add(id.ToString(), this.Url.Action("Details", new { id = id, }));
                this.NavigationLine().Add("Edit", this.Url.Action("Edit", new { id = id, }));
            }
            else
            {
                this.NavigationLine().Add("Create new", this.Url.Action("Edit"));
            }

            if (request == null)
            {
                request = subject ?? new ApiKeyModel() { IsEnabled = true, };
            }

            if (this.Request.IsHttpPostRequest())
            {
                if (this.ModelState.IsValid)
                {
                    var result = this.Services.ApiKeys.Update(request);

                    this.TempData.AddInfo("Saved.");
                    return this.RedirectToAction("Index");
                    ////return this.RedirectToAction("Details", new { id = id, });
                }
            }
            else
            {
                this.ModelState.Clear();
                request = subject ?? request;
            }

            return this.View(request);
        }

        public ActionResult Details(Guid id)
        {
            var subject = this.Services.ApiKeys.Get(id);
            if (subject == null)
                return this.ResultService.NotFound();

            this.NavigationLine().Add(id.ToString(), this.Url.Action("Details", new { id = id, }));

            return this.View(subject);
        }
    }
}
