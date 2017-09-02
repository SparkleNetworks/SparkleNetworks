
namespace Sparkle.WebStatus.Controllers
{
    using Sparkle.WebStatus.Domain;
    using SrkToolkit.Web;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    [Authorize]
    public class StatusController : Controller
    {
        public ActionResult Index()
        {
            return this.View();
        }

        public ActionResult DomainNameRecords()
        {
            var items = this.Services.DomainNameRecords.GetAll();
            this.ViewBag.CreateModel = new DomainNameRecord();

            return this.View(items.Values);
        }

        public class DataOrException<T1>
        {
            public T1 Data { get; set; }
            public Exception Exception { get; set; }
        }

        public class DataOrException<T1, T2, T3>
        {
            public T1 Data1 { get; set; }
            public T2 Data2 { get; set; }
            public T3 Data3 { get; set; }
            public Exception Exception { get; set; }
        }
    }
}