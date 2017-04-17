using System.Collections.Generic;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface ITouchCommunicationItemsService
    {
        int Insert(TouchCommunicationItem item);
        TouchCommunicationItem Update(TouchCommunicationItem item);
        void Delete(TouchCommunicationItem item);

        IList<TouchCommunicationItem> SelectAll();
        TouchCommunicationItem GetById(int id);

        IList<TouchCommunicationItem> GetByParentId(int parentId);
    }
}
