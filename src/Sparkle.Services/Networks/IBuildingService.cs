using Sparkle.Entities.Networks;
using System.Collections.Generic;

namespace Sparkle.Services.Networks
{
    public interface IBuildingService
    {
        int Insert(Building item);
        Building Update(Building item);
        void Delete(Building item);

        IList<Building> SelectAll();
        Building GetById(int buildingId);
    }
}
