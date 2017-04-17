
namespace Sparkle.App_Start
{
    using BundleTransformer.Core.Bundles;
    using BundleTransformer.Core.Orderers;
    using Sparkle.WebBase;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Optimization;

    public static class BundleConfig
    {
        public const string DefaultLibScripts = "~/b/Scripts/defaultlibs.js";
        public const string DefaultScripts = "~/b/Scripts/default.js";
        public const string PublicLibScripts = "~/b/Styles/publiclibs.js";
        public const string PublicScripts = "~/b/Styles/public.js";
        public const string DefaultStyles = "~/b/Styles/default.css";

        public static void ConfigureStaticBundles(BundleCollection collection)
        {
            var scriptTransforms = new IItemTransform[]
            {
            };

            // defaultlibs.js
            // contains all the JS libraries used when the user is authenticated
            var defaultLibScripts = new ScriptBundle(DefaultLibScripts)
                .Include("~/Scripts/modernizr.custom.35245.js")
                .Include("~/Scripts/jquery-1.11.1.js")
                .Include("~/Scripts/jquery-ui-1.10.4.custom.js")
                .Include("~/Scripts/underscore-min.js")
                .Include("~/Scripts/jquery.elastic.js")
                .Include("~/Scripts/jquery.mentionsInput.js")
                .Include("~/Scripts/knob.js");
            ////defaultScripts.Transforms.Add(JsObscureStringTransform);
            BundleTable.Bundles.Add(defaultLibScripts);

            // default.js
            // contains all the JS SCRIPTS used when the user is authenticated
            var defaultScripts = new CustomScriptBundle(DefaultScripts)
                .Include("~/Scripts/common.js", scriptTransforms)
                .Include("~/Scripts/global.js", scriptTransforms)
                .Include("~/Scripts/timeline.js", scriptTransforms);
            defaultScripts.Builder = new DefaultBundleBuilder();
            defaultScripts.Orderer = new NullOrderer();
            defaultScripts.ConcatenationToken = ";" + Environment.NewLine;
            defaultScripts.Transforms.Add(new JsMinify());
            defaultScripts.Transforms.Add(new JsObscureStringTransform());
            BundleTable.Bundles.Add(defaultScripts);

            // publiclibs.js
            // contains all the JS libraries used when the user is NOT authenticated
            var publicLibScripts = new ScriptBundle(PublicLibScripts)
                .Include("~/Scripts/modernizr.custom.35245.js")
                .Include("~/Scripts/jquery-1.11.1.js")
                .Include("~/Scripts/jquery-ui-1.10.4.custom.js")
                .Include("~/Scripts/jquery.elastic.js")
                ;
            BundleTable.Bundles.Add(publicLibScripts);

            // public.js
            // contains all the JS SCRIPTS used when the user is NOT authenticated
            var publicScripts = new CustomScriptBundle(PublicScripts)
                .Include("~/Scripts/common.js", scriptTransforms)
                .Include("~/Scripts/public.js", scriptTransforms)
                .Include("~/Scripts/timeline.js", scriptTransforms)
                ;
            publicScripts.Builder = new DefaultBundleBuilder();
            publicScripts.Orderer = new NullOrderer();
            publicScripts.ConcatenationToken = ";" + Environment.NewLine;
            publicScripts.Transforms.Add(new JsMinify());
            publicScripts.Transforms.Add(new JsObscureStringTransform());
            BundleTable.Bundles.Add(publicScripts);

            var sparkleShow = new CustomStyleBundle("~/b/Styles/SparkleShow.css")
                .Include("~/Content/SparkleShow.less");
            sparkleShow.Orderer = new NullOrderer();
            BundleTable.Bundles.Add(sparkleShow);

            var dashboardStyles = new CustomStyleBundle("~/b/Styles/Dashboard.css")
                .Include("~/Content/Dashboard.less");
            dashboardStyles.Orderer = new NullOrderer();
            BundleTable.Bundles.Add(dashboardStyles);
        }

        public static void ConfigureNetworkBundles(BundleCollection collection, HttpServerUtility server, string networkName)
        {
            var siteCssBundle = new CustomStyleBundle(BundleConfig.DefaultStyles);
            siteCssBundle.Orderer = new NullOrderer();

            var networkSiteLess = "~/Content/Networks/" + networkName + "/Styles/Site.less";
            if (System.IO.File.Exists(server.MapPath(networkSiteLess)))
                siteCssBundle.Include(networkSiteLess);
            else
                siteCssBundle.Include("~/Content/Site.less");
            siteCssBundle.Include("~/Content/Timeline.less");
            siteCssBundle.Include("~/Content/themes/sparkle/jquery-ui-1.10.4.min.css");
            siteCssBundle.Include("~/Content/jquery.mentionsInput.css");
            BundleTable.Bundles.Add(siteCssBundle);
        }
    }
}
