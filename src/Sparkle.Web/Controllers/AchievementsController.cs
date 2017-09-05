
namespace Sparkle.Controllers
{
    using Sparkle.Filters;
    using Sparkle.Models;
    using Sparkle.Services.Networks.Models;
    using Sparkle.WebBase;
    using System.Linq;
    using System.Web.Mvc;

    [AuthorizeUser]
    public class AchievementsController : LocalSparkleController
    {
        public ActionResult Index()
        {
            if (!this.Services.AppConfiguration.Tree.Features.Companies.Achievements.IsEnabled)
                return this.ResultService.NotFound();

            AchievementsModel model = new AchievementsModel();

            model.Achievements = this.Services.Achievements.SelectAll().Where(g => g.Level == 1).ToList();
            return View(model);
        }
    }
}
