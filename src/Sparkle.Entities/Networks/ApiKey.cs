
namespace Sparkle.Entities.Networks
{
    partial class ApiKey : IEntityInt32Id
    {
        public override string ToString()
        {
            return "ApiKey " + this.Id + " " + this.Key;
        }
    }
}
