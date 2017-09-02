
namespace Sparkle.WebBase.WebApi
{
    using System;

    public class AuthorizeResult
    {
        public AuthorizeResult()
        {
        }

        public AuthorizeResult(bool success)
        {
            this.IsAuthorized = success;
        }

        public AuthorizeResult(string error, string helpMessage)
        {
            this.Error = error;
            this.IsAuthorized = error == null;
            this.HelpMessage = helpMessage;
        }

        public AuthorizeResult(string error, string errorDetail, string helpMessage)
        {
            this.Error = error;
            this.ErrorDetail = errorDetail;
            this.IsAuthorized = error == null;
            this.HelpMessage = helpMessage;
        }

        public static AuthorizeResult Success
        {
            get { return new AuthorizeResult(true); }
        }

        public bool IsAuthorized { get; set; }

        public string Error { get; set; }

        public string ErrorDetail { get; set; }

        public string HelpMessage { get; set; }
    }
}
