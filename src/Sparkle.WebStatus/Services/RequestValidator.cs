
namespace Sparkle.WebStatus.Services
{
    using System.Web;
    using System.Web.Util;

    public class RequestValidator : System.Web.Util.RequestValidator
    {
        protected override bool IsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
        {
            validationFailureIndex = 0;
            return true;
        }
    }
}
