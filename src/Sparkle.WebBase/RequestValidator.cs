
namespace Sparkle.WebBase
{
    using System.Web;
    using System.Web.Util;

    /// <summary>
    /// Allows special characters in the URL.
    /// Implementation is inspired from : http://stackoverflow.com/questions/3990704/how-to-disable-request-validation-and-enable-longer-query-strings-in-asp-net-4-0/4022106#4022106
    /// Maybe we should try: http://stackoverflow.com/a/38282721/282105
    /// </summary>
    public class RequestValidator : System.Web.Util.RequestValidator
    {
        protected override bool IsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
        {
            validationFailureIndex = 0;
            return true;
        }
    }
}
