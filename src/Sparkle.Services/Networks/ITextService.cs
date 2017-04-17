
namespace Sparkle.Services.Networks
{
    using Sparkle.Services.Networks.Texts;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ITextService
    {
        EditTextRequest GetEditRequest(int? textId, string culture);
        EditTextRequest GetEditRequest(int? textId, CultureInfo culture);
        EditTextResult SaveText(EditTextRequest request);

        IList<EditTextRequest> GetAllByTextId(int? textId);
    }
}
