
namespace Sparkle.Entities.Networks
{
    partial class SocialNetworkUserSubscription : IEntityInt32Id
    {
    }

    partial class SocialNetworkCompanySubscription : IEntityInt32Id
    {
    }

    partial class SocialNetworkConnection : IEntityInt32Id
    {
        public SocialNetworkConnectionType SocialNetworkConnectionType
        {
            get { return (SocialNetworkConnectionType)this.Type; }
            set { this.Type = (byte)value; }
        }
    }

    partial class SocialNetworkState : IEntityInt32Id, INetworkEntity
    {
        public SocialNetworkConnectionType SocialNetworkConnectionType
        {
            get { return (SocialNetworkConnectionType)this.SocialNetworkType; }
            set { this.SocialNetworkType = (byte)value; }
        }

        /// <summary>
        /// Gets a value indicating whether the connection is configured depending on its type.
        /// </summary>
        public bool IsConfigured
        {
            get
            {
                switch (this.SocialNetworkConnectionType)
                {
                    case SocialNetworkConnectionType.LinkedIn:
                        return (!string.IsNullOrEmpty(this.OAuthAccessToken) && !string.IsNullOrEmpty(this.OAuthAccessSecret));

                    case SocialNetworkConnectionType.Twitter:
                        return this.OAuthAccessToken != null;

                    case SocialNetworkConnectionType.Facebook:
                    default:
                        return false;
                }
            }
        }

        public void ClearOAuth()
        {
            this.OAuthAccessSecret = null;
            this.OAuthAccessToken = null;
            this.OAuthDateUtc = null;
            this.OAuthRequestToken = null;
            this.OAuthRequestVerifier = null;
        }
    }
}
