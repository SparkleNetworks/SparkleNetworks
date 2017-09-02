
namespace Sparkle.WebStatus.Controllers
{
    using Newtonsoft.Json;
    using Sparkle.WebStatus.Domain;
    using SrkToolkit.Web;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // this page is empty but required
            return this.View();
        }

        [Authorize]
        public ActionResult AuthorizeEcho(string message)
        {
            if (this.Request.PrefersJson())
            {
                return this.ResultService.JsonSuccess(message);
            }
            else
            {
                return this.View(message);
            }
        }
    }
}