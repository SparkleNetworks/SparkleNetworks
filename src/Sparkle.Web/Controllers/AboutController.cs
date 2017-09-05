
namespace Sparkle.Controllers
{
    using Sparkle.Entities.Networks;
    using Sparkle.Filters;
    using Sparkle.Models;
    using Sparkle.WebBase;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;

    [SubscriberAccess(RequiresActiveSubscription = false)]
    public class AboutController : LocalSparkleController
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            this.Services.People.OptionsList = new List<string> { "Job", "Company", "Links" };
        }

        /// <summary>
        /// About Homepage.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return this.ResultService.NotFound();
        }

        /// <summary>
        /// FAQ page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Faq()
        {
            //return this.Redirect(ExternalUrls.SparkleNetworksSupport);
            return View();
        }

        /// <summary>
        /// Televisions this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Television()
        {
            return this.ResultService.NotFound();
        }

        /// <summary>
        /// Page Why.
        /// </summary>
        /// <returns></returns>
        public ActionResult Why()
        {
            return this.ResultService.NotFound();
        }

        /// <summary>
        /// Page In the this network.
        /// </summary>
        /// <returns></returns>
        public ActionResult InThisNetwork()
        {
            ////InThisNetworkModel model = new InThisNetworkModel();
            ////IList<Company> companies = this.Services.Company.SelectAll();
            ////model.Companies = companies.Select(o => new CompanyModel(this.Services, o)).ToList();
            return RedirectToAction("Index", "Companies");
        }

        /// <summary>
        /// Page Terms.
        /// </summary>
        /// <returns></returns>
        public ActionResult Terms()
        {
            var extraTermsFilePath = "/Content/Networks/" + this.Services.Network.Name + "/Documents/Terms.md";
            extraTermsFilePath = this.Server.MapPath(extraTermsFilePath);
            this.ViewBag.ExtraTermsFilePath = System.IO.File.Exists(extraTermsFilePath) ? extraTermsFilePath : default(string);
            return this.View();
        }

        /// <summary>
        /// Page Policy.
        /// </summary>
        /// <returns></returns>
        public ActionResult Policy()
        {
            return View();
        }

        /// <summary>
        /// Page Contact.
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {
            ContactModel model = new ContactModel();
            var me = this.SessionService.User;
            if (me != null)
            {
                model.Name =  me.FirstName + " " + me.LastName;
                model.Email = me.Email;
            }
            return View(model);
        }

        public ActionResult BecomeAPartner()
        {
            return this.ResultService.Gone();
        }
    }
}
