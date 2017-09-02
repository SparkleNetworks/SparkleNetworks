
namespace Sparkle.Services.Main.Internal
{
    using Stripe;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class StripeExceptionExtensions
    {
        public static string ToFullString(this StripeException ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            var sb = new StringBuilder();
            sb.AppendLine("HttpStatusCode: " + ex.HttpStatusCode.ToString());
            sb.AppendLine("Message: " + ex.Message);
            sb.AppendLine("Source: " + ex.Source);
            if (ex.InnerException != null)
            {
                sb.AppendLine("InnerException: " + ex.InnerException.ToString());
            }

            if (ex.StripeError != null)
            {
                sb.AppendLine("StripeError.ChargeId: " + ex.StripeError.ChargeId);
                sb.AppendLine("StripeError.Code: " + ex.StripeError.Code);
                sb.AppendLine("StripeError.Error: " + ex.StripeError.Error);
                sb.AppendLine("StripeError.ErrorSubscription: " + ex.StripeError.ErrorSubscription);
                sb.AppendLine("StripeError.ErrorType: " + ex.StripeError.ErrorType);
                sb.AppendLine("StripeError.Message: " + ex.StripeError.Message);
                sb.AppendLine("StripeError.Parameter: " + ex.StripeError.Parameter);
            }

            sb.AppendLine("StackTrace: " + ex.StackTrace);

            return sb.ToString();
        }
    }
}
