
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface ITextRepository : IBaseNetworkRepository
    {
        IList<TextValue> GetTextValueByTextId(int? textId);
        TextValue GetTextValueByTextIdAndCulture(int? textId, string culture);

        int? SetTextValue(int? textId, string culture, string title, string value, DateTime date, int userId);
    }
}
