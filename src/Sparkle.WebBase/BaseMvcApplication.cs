
namespace Sparkle.WebBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;

    public class BaseMvcApplication : HttpApplication
    {
        protected static List<string> DefaultIgnoreRoutes = new List<string>()
        {
            "{resource}.axd", // ??
            "{service}.svc",  // services
            "Content",        // content
            "ClientBin",      // silverlight stuff
            "{pageName}.htm",             // static pages
            "{favicon}.ico",              // icons
            "{javascripts}.js",           // javascripts
            "{picture}.gif",              // pics
            "{picture}.jpg",              // pics
            "{picture}.png",              // pics
        };

        static private readonly string[] ignoreErrorUrls = new string[]
        {
            // special urls
            "/browserconfig.xml",          // IE thing
            "/sitemap.xml",                // 

            // script kiddies stuff
            "/wp-admin/",
            "/wp-login/",
            "/administrator",
            "/admin.php",
            "/index.html",
            "/checksite/custom-error-page-test",
            "/user/swfupload.asp",
            "/installer/",
            "/web/phpMyAdmin/",
            "/phpmyad-sys/",
            "/phpMyAdmi/",
            "/phpMyAds/",
            "/phpmyad/",
            "/phpMyA/",
            "/mysql-admin/",
            "/phpm/",
            "/phpmy/",
            "/mysqladmin/",
            "/mysql/",
            "/myadmin/",
            "/pma/",
            "/dbadmin/",
            "/js/mage/cookies.js",
            "/wp-includes/wlwmanifest.xml",
            "/account/register.php",
            "/Scripts/upload.php",
            "/apple-touch-icon.png",
            "/password_forgotten.php",
            "/webeditor",
            "assetmanager/asset",
            "/assetmanager.php",
            "/trackback/",
            "/Companies/trackback/",
            "/misc/drupal.js",
            "/editor1/editor/",
            "/editorold/editor/",
            "/ckeditor/editor/",
            "/wp",
        };

        static private readonly Func<string, bool>[] ignoreErrorRules = new Func<string, bool>[]
        {
            // special urls
            s => s.StartsWith("/sitemap") & s.EndsWith(".xml"),
            s => s.StartsWith("/b/Styles/)"),
            s => s.StartsWith("/b/Styles/]"),
            s => s.StartsWith("/bundles/") && s.Contains("/)"),
            s => s.StartsWith("/bundles/") && s.Contains("/]"),

            // bad html or bad bots
            s => s.Contains(").addClass("),
            s => s.Contains(").html("),

            // script kiddies stuff
            s => s.StartsWith("/wp-") & s.EndsWith(".php"),
            s => s.StartsWith("/wp/wp-"),
            s => s.Contains("/wp-admin/"),
            s => s.Contains("/wp-images/"),
            s => s.Contains("/wp-content/"),
            s => s.Contains("/wp-login.php"),
            s => s.StartsWith("/admin/"),
            s => s.StartsWith("/administrator/"),
            s => s.StartsWith("/Account/administrator/"),
            s => s.StartsWith("/Content/administrator/"),
            s => s.StartsWith("/wordpress/"),
            s => s.StartsWith("/blog/"),
            s => s.StartsWith("/cgi-bin/"),
            s => s.StartsWith("/phpMyAdmin/", StringComparison.InvariantCultureIgnoreCase),
            s => s.StartsWith("/components/com_community/"),
            s => s.StartsWith("/vbulletin/"),
            s => s.StartsWith("/community/modcp/"),
            s => s.StartsWith("/forums/modcp/"),
            s => s.StartsWith("/cc/modcp/"),
            s => s.StartsWith("/vb/modcp/"),
            s => s.StartsWith("/modcp/"),
            s => s.Contains("/uploadify.swf"),
            s => s.Contains("uploadify/uploadify"),
            s => s.Contains("uploader/uploadify"),
            s => s.Contains("upload/uploadify"),
            s => s.EndsWith("/6_S3_"),
            s => s.Contains("/xmlrpc.php"),
            s => s.Contains("/pg/dashboard"),
            s => s.Contains("/magic.php.png"),
            s => s.IndexOf("/fckeditor", StringComparison.OrdinalIgnoreCase) >= 0,
            s => s.IndexOf("/fck/editor/", StringComparison.OrdinalIgnoreCase) >= 0,
            s => s.Contains("/editor/editor/"),
            s => s.Contains("/editor/filemanager/"),
            s => s.Contains("/bitrix/"),
            s => s.Contains("ofc-library/"),
            s => s.Contains("/tiny_mce/"),
            s => s.EndsWith("tinymce.js"),
            s => s.Contains("db/") && s.EndsWith("dump.php"),
            s => s.Contains("database/") && s.EndsWith("dump.php"),
            s => s.Contains("backup/") && s.EndsWith("dump.php"),
            s => s.Contains("sql/") && s.EndsWith("dump.php"),
            s => s.Contains("bigdump/bigdump.php"),
            s => s.Contains("/hybridauth/"),
            s => s.Contains("sqlite/main.php"),
            s => s.Contains("/swfupload/"),
            s => (s.Contains("system/") || s.Contains("manage/") || s.Contains("/html"))
              && (s.Contains("edit") || s.Contains("oledit") || s.Contains("webdit") || s.IndexOf("eWebEditor", StringComparison.OrdinalIgnoreCase) >= 0),
            s => s.Contains("/elfinder"),
            s => s.Contains("/uas/"),
            s => s.IndexOf("/uploadtester.asp", StringComparison.InvariantCultureIgnoreCase) >= 0,
            s => s.EndsWith("/README.txt")
        };

        protected virtual bool IsHttpErrorToBeIgnored(HttpContext context, Exception exception)
        {
            string url = context.Request.Url.PathAndQuery;

            if (context.Response != null && context.Response.StatusCode == 404)
            {
                return true;
            }

            if (url != null && ignoreErrorUrls.Contains(url))
            {
                return true;
            }

            if (url != null && ignoreErrorRules.Any(rule => rule(url)))
            {
                return true;
            }

            return false;
        }
    }
}
