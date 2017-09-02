using System.Collections.Generic;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface ILostItemsService
    {
        IList<string> OptionsList { get; set; }
        int Insert(LostItem item);
        void Delete(LostItem item);
        LostItem Update(LostItem item);

        IList<LostItem> SelectAll();

        LostItem SelectById(int lostItemId);

        int Count();
    }
}
