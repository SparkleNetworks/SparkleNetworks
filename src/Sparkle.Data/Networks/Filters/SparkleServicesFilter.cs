
namespace Sparkle.Data.Filters
{
    using System.Linq;
    using Sparkle.Entities.Networks;

    public static class SparkleServicesFilter
    {
        public static IEnumerable<SparkleService> ById(
        this IEnumerable<SparkleService> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IEnumerable<SparkleService> Available(
        this IEnumerable<SparkleService> qry)
        {
            return qry.Where(o => o.Available == true);
        }
    }
}
