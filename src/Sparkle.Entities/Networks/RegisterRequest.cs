
namespace Sparkle.Entities.Networks
{
    partial class RegisterRequest : IEntityInt32Id, INetworkEntity
    {
        public string EmailAddress
        {
            get
            {
                if (this.EmailTagPart != null)
                    return this.EmailAccountPart + "+" + this.EmailTagPart + "@" + this.EmailDomain;
                else
                    return this.EmailAccountPart + "@" + this.EmailDomain;
            }
        }

        public RegisterRequestStatus StatusCode
        {
            get { return (RegisterRequestStatus)this.Status; }
            set { this.Status = (short)value; }
        }
    }

    public enum RegisterRequestStatus : short
    {
        /// <summary>
        /// New register request (0).
        /// </summary>
        New = 0,

        /// <summary>
        /// An administrator started to communicate with the person.
        /// The request is still new (1).
        /// </summary>
        ExternalCommunication = 1,

        /// <summary>
        /// Request denied (2).
        /// </summary>
        Refused = 2,

        /// <summary>
        /// Request accepted (3)
        /// </summary>
        Accepted = 3,
    }

    public enum RegisterRequestOptions
    {
        None = 0,
        AcceptedInvitation = 0x0002,
        Company            = 0x0004,
        ValidatedBy        = 0x0008,
        All = AcceptedInvitation | Company | ValidatedBy,
    }
}
