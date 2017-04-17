
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;

    public class CompanyWithStatistics
    {
        public Company Company { get; set; }

        public int Members { get; set; }

        public int Invited { get; set; }
    }
}
