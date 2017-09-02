
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using Sparkle.Data.Networks;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public class LunchRepository : BaseNetworkRepository, ILunchRepository
    {
        public LunchRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory)
        {
        }

        public IQueryable<Lunch> Select()
        {
            return this.Context.Lunches;
        }

        public int Insert(Lunch item)
        {
            using (var dc = this.NewContext)
            {
                dc.AddToLunches(item);
                dc.SaveChanges();
            }
            return item.Id;
        }

        public Lunch Update(Lunch item)
        {
            return DeleteOrUpdate(item, CRUD.Update);
        }

        public void Delete(Lunch item)
        {
            DeleteOrUpdate(item, CRUD.Delete);
        }

        private Lunch DeleteOrUpdate(Lunch item, CRUD crud)
        {
            using (var DC = this.NewContext)
            {
                EntityKey key = DC.CreateEntityKey(DC.Lunches.EntitySet.Name, item);
                Object OutItem;
                if (DC.TryGetObjectByKey(key, out OutItem))
                {
                    switch (crud)
                    {
                        case CRUD.Update:
                            DC.Lunches.ApplyCurrentValues(item);
                            break;
                        case CRUD.Delete:
                            DC.DeleteObject(OutItem);
                            break;
                    }
                    DC.SaveChanges();
                }
            }
            return item;
        }
    }
}
