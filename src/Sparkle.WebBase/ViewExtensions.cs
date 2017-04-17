
namespace Sparkle.WebBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public static class ViewExtensions
    {
        /// <summary>
        /// Returns a high-level strongly-typed context container (never null).
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="view"></param>
        /// <returns>A <see cref="ViewServices"/> instance (never null).</returns>
        public static ViewServices Services<TModel>(this WebViewPage<TModel> view)
        {
            return view.ViewData.ContainsKey("ViewServices") ? (ViewServices)view.ViewData["ViewServices"] : new ViewServices();
        }
    }
}