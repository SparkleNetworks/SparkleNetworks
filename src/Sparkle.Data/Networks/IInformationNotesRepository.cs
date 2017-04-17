
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    [Repository]
    public interface IInformationNotesRepository : IBaseNetworkRepository<InformationNote, int>
    {
        IList<InformationNote> GetList(int offset, int count, InformationNotesFilter filter, InformationNoteOptions options);

        int CountList(InformationNotesFilter filter);

        int Count();
    }
}
