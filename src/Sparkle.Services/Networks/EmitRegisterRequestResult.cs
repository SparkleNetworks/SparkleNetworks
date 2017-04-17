
namespace Sparkle.Services.Networks
{
    using Sparkle.Common;
    using SrkToolkit.Common.Validation;

    public class EmitRegisterRequestResult
    {
        public EmailAddress EmailAddress { get; set; }

        public string CompanyName { get; set; }

        public EmitRegisterRequestCode Code { get; set; }

        public Entities.Networks.RegisterRequest Entity { get; set; }
    }

    public enum EmitRegisterRequestCode
    {
        RequestExists,
        RequestEmitted,
        EmailAddressAlreadyInUse,
        NoSuchCompany,
    }
}
