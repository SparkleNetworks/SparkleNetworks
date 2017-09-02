
namespace Sparkle.WebBase
{
    using System;
    using System.Web.Mvc;

    public static class ModelStateDictionaryExtensions
    {
        public static string Summary(this ModelStateDictionary state)
        {
            string summary = string.Empty;
            foreach (var item in state)
            {
                summary += (string.IsNullOrEmpty(item.Key) ? "form" : item.Key) + ": " + Environment.NewLine;
                foreach (var err in item.Value.Errors)
                {
                    summary += "  " + err.ErrorMessage;
                }
            }

            return summary;
        }
    }
}
