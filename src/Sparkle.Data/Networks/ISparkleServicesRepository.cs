using System.Linq;
using Sparkle.Entities.Networks;

namespace Sparkle.Data.Networks
{
    [Repository]
    public interface ISparkleServicesRepository
    {
        IQueryable<SparkleService> Select();
        int Insert(SparkleService item);
        SparkleService Update(SparkleService item);
        void Delete(SparkleService item);
    }
}
