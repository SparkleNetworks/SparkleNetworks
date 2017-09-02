
namespace Sparkle.EmailTemplates
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web.Mvc;
    using RazorEngine.Configuration;
    using RazorEngine.Templating;
    using Sparkle.Common;
    using Sparkle.EmailTemplates.Internals;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.Services.Networks;
    using Sparkle.UI;
    using SrkToolkit.Web.Fakes;

    /// <summary>
    /// This generates HTML email messages from templates stored within this assembly.
    /// Uses src\External\FX40\RazorEngine.dll as razor compiler and runner.
    /// </summary>
    public class DefaultEmailTemplateProvider : IEmailTemplateProvider
    {
        private const string ResourceFileExtension = ".cshtml";

        private static Regex includeNamesRegex = new Regex("@\\*\\s+Razor\\sInclude:\\s+([A-Z0-9_\\.-]+)(?:\\s*,\\s*([A-Z0-9_\\.-]+))*\\s+\\*@", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly string folder;

        private static TemplateServiceConfiguration config;

        private static TemplateService template;

        private static readonly object TemplateServiceLock = new object();

        public DefaultEmailTemplateProvider(string folder)
        {
            this.folder = folder;
        }

        public DefaultEmailTemplateProvider()
            : this("default")
        {
        }

        /// <summary>
        /// Gets the provider that compiles razor files and runs them. 
        /// </summary>
        private TemplateService TemplateService
        {
            get
            {
                if (template == null)
                {
                    config = new TemplateServiceConfiguration
                    {
                        BaseTemplateType = typeof(CustomTemplateBase<>),
#if DEBUG
                        Debug = true,
#endif
                    };
                    template = new TemplateService(config);

                    // using statements for all views
                    template.AddNamespace("SrkToolkit.Web");
                    template.AddNamespace("Sparkle.Helpers");

                    // those invocations make sure all the necessary assemblies are loaded during razor runtime
                    // not doing that triggers exceptions
                    // I can't explain this phenomena.
                    System.Web.Mvc.SrkHtmlExtensions.TrimText(null, string.Empty, 1);
                    System.Web.HtmlString htmlString = new System.Web.HtmlString("hello");
                    Sparkle.Helpers.HtmlExtention.Text(null, "");
                    new Sparkle.Entities.Networks.Company();
                    new Sparkle.Data.Networks.Objects.CompanyExtended();
                }

                return template;
            }
        }

        /// <summary>
        /// Renders a email template.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The name of the template</param>
        /// <param name="network">NULL</param>
        /// <param name="culture">The users' language</param>
        /// <param name="timeZone">The users' time zone</param>
        /// <param name="model">The data model</param>
        /// <returns>HTML</returns>
        public string Process<T>(string key, string network, CultureInfo culture, TimeZoneInfo timeZone, T model)
            where T : BaseEmailModel
        {
            timeZone = timeZone ?? TimeZoneInfo.Utc;

            // get the global template
            string masterPath = this.folder + ".Master" + ResourceFileExtension;
            string master = this.GetResourceAsText(masterPath);
            if (master == null)
                throw new InvalidOperationException("Email rendering failed: layout '" + masterPath + "' is empty");

            // get the message template
            string childPath = this.folder + "." + key + ResourceFileExtension;
            string child = this.GetResourceAsText(childPath);
            if (child == null)
                throw new InvalidOperationException("Email rendering failed: page '" + childPath + "' is empty");

            // include shared templates if we find the following markup
            // @* Razor Include: _Timeline.cshtml, _View.cshtml *@
            var includes = this.FindIncludes(child);
            foreach (var includeName in includes)
            {
                string path = includeName;
                string content = this.GetResourceAsText(path);
                if (content != null)
                {
                    child += Environment.NewLine + content + Environment.NewLine;
                }
                else
                {
                    throw new InvalidOperationException("Email rendering failed: include '" + path + "' is empty");
                }
            }

            // HACK: we are managing templating by hand instead of relying on RazorEngine
            var oldChild = child;
            child = oldChild.Replace("Layout = \"Parent\";", "Layout = \"Parent_" + key + "\";");
            if (child == oldChild)
                throw new InvalidDataException("Something went wrong with the email template, 'Layout = \"Parent\";' was not found");

            // TODO: explain why we are using a global lock. 
            string result;
            lock (TemplateServiceLock)
            {
                // compile templates
                CustomTemplateBase<T> temp1, temp2;
                try
                {
                    temp1 = (CustomTemplateBase<T>)TemplateService.GetTemplate(master, model, "Parent_" + key);
                    temp2 = (CustomTemplateBase<T>)TemplateService.GetTemplate(child, model, key);
                }
                catch (TemplateParsingException ex)
                {
                    // an error occured while rending an html email template.
                    Debug.WriteLine("TemplateParsingException -:" + ex.Line + ":" + ex.Column + "  " + ex.Message);
                    throw ex;
                }
                catch (TemplateCompilationException ex)
                {
                    // an error occured while compiling an html email template. it seems you put a syntax error in one of the templates.
                    if (ex.Errors != null)
                    {
                        foreach (var error in ex.Errors)
                        {
                            Debug.WriteLine("TemplateCompilationException " + error.FileName + ":" + error.Line + ":" + error.Column + " " + error.ErrorNumber + " " + error.ErrorText);
                        }
                    }

                    throw ex;
                }

                // we like the HtmlHelper thing that exists in ASP MVC
                // so we are putting it in the template engine
                // we need to fake some context to make it work
                temp1.Html = new System.Web.Mvc.HtmlHelper<T>(new System.Web.Mvc.ViewContext(), new ViewPage<T>());
                temp1.Html.ViewContext.HttpContext = new BasicHttpContext();
                temp1.Html.SetTimezone(timeZone ?? TimeZoneInfo.Utc);
                temp1.Html.GetTimezone();
                temp1.Culture = culture ?? CultureInfo.CurrentCulture;
                temp1.Timezone = timeZone;

                temp2.Html = temp1.Html;
                temp2.Culture = temp1.Culture;
                temp2.Timezone = temp1.Timezone;

                // render the template as HTML
                // ThreadCulture temporarily changes the current language
                using (new ThreadCulture(culture, culture))
                {
                    model.Culture = CultureInfo.CurrentCulture;
                    model.Timezone = timeZone;
                    result = TemplateService.Run(temp2, new DynamicViewBag()); 
                }
            }
            
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new InvalidOperationException("Rendered email is empty");
            }

            return result;
        }
        
        protected IList<string> FindIncludes(string template)
        {
            var includeNamesMatches = includeNamesRegex.Matches(template);
            var includes = new List<string>();
            foreach (Match includeNamesMatch in includeNamesMatches)
            {
                var group1 = includeNamesMatch.Groups[1].Value.Trim();
                if (!string.IsNullOrEmpty(group1))
                    includes.Add(group1);

                foreach (Capture group2 in includeNamesMatch.Groups[2].Captures)
                {
                    var value = group2.Value.Trim();
                    if (!string.IsNullOrEmpty(value))
                        includes.Add(value);
                }
            }

            return includes;
        }

        /// <summary>
        /// Gets a template as text from the compiled assembly.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetResourceAsText(string path)
        {
            var assembly = this.GetType().Assembly;
            path = assembly.GetName().Name + "." + path;
            var stream = assembly.GetManifestResourceStream(path);
            if (stream == null)
                return null;

            try
            {
                var reader = new StreamReader(stream);
                var result = reader.ReadToEnd();
                return result;
            }
            finally
            {
                stream.Close();
            }
        }
    }
}
