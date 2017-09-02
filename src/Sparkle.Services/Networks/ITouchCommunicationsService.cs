using System.Collections.Generic;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface ITouchCommunicationsService
    {
        int Insert(TouchCommunication item);
        TouchCommunication Update(TouchCommunication item);
        void Delete(TouchCommunication item);

        IList<TouchCommunication> SelectAll();
        TouchCommunication GetById(int id);

        TouchCommunication GetByDay(System.DateTime dateTime);
    }
}
