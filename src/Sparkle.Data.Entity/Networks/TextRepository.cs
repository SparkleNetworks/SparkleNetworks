
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TextRepository : BaseNetworkRepository, ITextRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public TextRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory)
        {
        }

        public IList<TextValue> GetTextValueByTextId(int? textId)
        {
            return this.Context.GetTextValueByTextId(textId).ToList();
        }

        public TextValue GetTextValueByTextIdAndCulture(int? textId, string culture)
        {
            return this.GetTextValueByTextId(textId).Where(o => o.CultureName == culture).SingleOrDefault();
        }

        public int? SetTextValue(int? textId, string culture, string title, string value, DateTime date, int userId)
        {
            return this.Context.SetTextValue(textId, culture, title, value, date, userId).Single();
        }
    }
}
