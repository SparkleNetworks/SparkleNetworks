
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface ILunchRepository
    {
        IQueryable<lunch> Select();
        int Insert(Lunch item);
        Lunch Update(Lunch item);
        void Delete(Lunch item);
    }
}
