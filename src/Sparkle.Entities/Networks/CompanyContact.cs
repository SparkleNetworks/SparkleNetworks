
namespace Sparkle.Entities.Networks
{
    partial class CompanyContact : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " " + this.Message;
        }
    }
}
