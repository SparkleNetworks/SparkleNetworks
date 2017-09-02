
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

    [Authorize]
    public class NetworksController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.NavigationLine().Add("Networks", this.Url.Action("Index"));

            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index()
        {
            var items = this.Services.Networks.GetAll().Values.OrderBy(n => n.NetworkName).ToList();
            return this.View(items);
        }

        public ActionResult Add(NetworkModel model)
        {
            this.NavigationLine().Add("Create new", this.Url.Action("Add"));

            model = model ?? new NetworkModel();

            if (this.Request.HttpMethod == "POST" && this.ModelState.IsValid)
            {
                this.Services.Networks.Create(model);
                return this.RedirectToAction("Index");
            }

            return this.View(model);
        }

        public ActionResult Edit(NetworkModel model, Guid id)
        {
            var network = this.Services.Networks.GetById(id);

            this.NavigationLine().Add(id.ToString(), this.Url.Action("Details", new { id = id, }));
            this.NavigationLine().Add("Edit", this.Url.Action("Edit", new { id = id, }));

            if (model == null || model.Guid == Guid.Empty)
            {
                model = network;
            }
            else if (this.Request.HttpMethod == "POST" && this.ModelState.IsValid)
            {
                this.Services.Networks.Update(model);
                this.TempData.AddInfo("Saved.");
                return this.RedirectToAction("Index");
                ////return this.RedirectToAction("Details", new { id = id, });
            }

            return this.View(model);
        }

        public ActionResult Status()
        {
            this.NavigationLine().Add("Status", this.Url.Action("Status"));

            var crashes = this.GetFromCache(TimeSpan.FromMinutes(10D), System.Web.Caching.CacheItemPriority.Low, () =>
            {
                var crashStatsClient = new Srk.BetaServices.ClientApi.Clients.JsonBetaservicesClient(
                    "75F939D9-5979-48F0-9CBC-54134E812A96", "SparkleStatus", "http://errors.sparklesystems.net/betaseries-services/");
                var crashStats = crashStatsClient.GetAssemblyStats("Sparkle.Web");

                var now = DateTime.UtcNow;
                if (!crashStats.Any(s => s.Date.Year == now.Year && s.Date.Month == now.Month && s.Date.Day == now.Day))
                {
                    crashStats.Add(new Srk.BetaServices.ClientApi.AssemblyStat
                    {
                        Date = now.ToPrecision(DateTimePrecision.Day),
                        AssemblyName = crashStats.First().AssemblyName,
                        Reports = 0,
                    });
                }

                return crashStats;
            });

            this.ViewBag.Crashes = crashes;

            var items = this.GetFromCache(TimeSpan.FromSeconds(10D), System.Web.Caching.CacheItemPriority.Normal, () =>
            {
                var items1 = this.Services.Networks.GetAllStatus();

                var localErrors = new List<string>();
                try
                {
                    this.Services.Verify();
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.Data != null && ex.Data["Errors"] is List<Tuple<string, Exception>>)
                    {
                        var errors = (List<Tuple<string, Exception>>)ex.Data["Errors"];
                        for (int i = 0; i < errors.Count; i++)
                        {
                            var error = errors[i];
                            localErrors.Add(error.Item1 + (error.Item2 != null ? (": " + error.Item2.Message) : string.Empty));
                        }
                    }
                    else
                    {
                        localErrors.Add(ex.Message);
                    }
                }

                try
                {
                    this.HttpContext.GetStatusServices().Verify();
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.Data != null && ex.Data["Errors"] is List<Tuple<string, Exception>>)
                    {
                        var errors = (List<Tuple<string, Exception>>)ex.Data["Errors"];
                        for (int i = 0; i < errors.Count; i++)
                        {
                            var error = errors[i];
                            localErrors.Add(error.Item1 + (error.Item2 != null ? (": " + error.Item2.Message) : string.Empty));
                        }
                    }
                    else
                    {
                        localErrors.Add(ex.Message);
                    }
                }

                var myAssembly = this.GetType().Assembly;
                items1.Add(new NetworkStatusModel
                {
#if DEBUG
                    BuildConfiguration = "DEBUG",
#else
                    BuildConfiguration = "RELEASE",
#endif
                    BuildDateUtc = System.IO.File.GetLastWriteTimeUtc(this.Server.MapPath("~/bin/Sparkle.WebStatus.dll")),
                    Network = new NetworkModel
                    {
                        InstanceName = "sparklenetworks",
                        NetworkName = "net",
                    },
                    ServicesVerifyErrors = localErrors.ToArray(),
                    ServicesVerified = localErrors.Count == 0,
                });
                return items1;
            });

            if (this.Request.PrefersJson())
            {
                ////if (this.ApiServices.IsKeyAuthorized(this.Request))
                return this.ResultService.JsonSuccess(items);
                ////return this.ResultService.JsonError("UnauthorizedApiKey");
            }

            return this.View(items);
        }

        public ActionResult OnlineUsers(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return this.ResultService.NotFound();

            this.NavigationLine().Add("Network " + id, this.Url.Action("Details", new { id = id, }));
            this.NavigationLine().Add("Online users", this.Url.Action("OnlineUsers", new { id = id, }));

            var online = this.Services.Networks.GetOnlineUsernames(id.Value);
            return this.View(online);
        }

        public ActionResult NetworkStatus(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return this.ResultService.NotFound();

            this.NavigationLine().Add("Network " + id, this.Url.Action("Details", new { id = id, }));
            this.NavigationLine().Add("Status", this.Url.Action("NetworkStatus", new { id = id, }));

            var online = this.Services.Networks.GetStatus(id.Value);
            return this.View(online);
        }

        [HttpPost]
        public ActionResult NetworkStatus(NetworkStatusModel model)
        {
            if (model == null || model.Network == null || model.Network.Guid == Guid.Empty)
                return this.ResultService.NotFound();

            try
            {
                this.Services.Networks.InitializeNetwork(model.Network.Guid);
                this.TempData.AddConfirmation("OK");
            }
            catch (Exception ex)
            {
                this.TempData.AddError(ex.ToSummarizedString());
            }

            return this.RedirectToAction("NetworkStatus", new { id = model.Network.Guid, });
        }
    }
}
