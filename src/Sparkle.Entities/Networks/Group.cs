
namespace Sparkle.Entities.Networks
{
    partial class Group : IEntityInt32Id, INetworkEntity
    {
        public NotificationFrequencyType? NotificationFrequencyValue
        {
            get { return (NotificationFrequencyType)this.NotificationFrequency; }
            set { this.NotificationFrequency = (byte)value; }
        }

        public override string ToString()
        {
            return this.Id + " " + this.Name;
        }
    }

    public enum GroupOptions
    {
        None            = 0x0000,
        Category        = 0x0001,
    }   
}
