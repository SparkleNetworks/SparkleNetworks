﻿
namespace Sparkle.UI
{
    using System.Web;

    public static class UrlTools
    {

        /// <summary>
        /// Create the left part of a URL based on a HTTP request.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static string Compose(HttpRequestBase httpRequest)
        {
            string url = httpRequest.IsSecureConnection ? "https://" : "http://";

            url += httpRequest.ServerVariables["SERVER_NAME"];

            if (httpRequest.ServerVariables["SERVER_PORT"] == "443" && httpRequest.IsSecureConnection || httpRequest.ServerVariables["SERVER_PORT"] == "80" && !httpRequest.IsSecureConnection)
            {
            }
            else
            {
                url += ":" + httpRequest.ServerVariables["SERVER_PORT"];
            }

            return url;
        }
    }
}
