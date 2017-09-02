
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Texts;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SrkToolkit.Domain;

    public class TextService : ServiceBase, ITextService
    {
        [DebuggerStepThrough]
        internal TextService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        private ITextRepository TextRepo
        {
            get { return this.Repo.Text; }
        }

        public EditTextRequest GetEditRequest(int? textId, string culture)
        {
            if (!this.Services.SupportedCultures.Any(o => !o.IsNeutralCulture && o.Name == culture))
                return null;

            return this.GetEditRequest(textId, new CultureInfo(culture));
        }

        public EditTextRequest GetEditRequest(int? textId, CultureInfo culture)
        {
            var request = new EditTextRequest();

            if (!this.Services.SupportedCultures.Any(o => !o.IsNeutralCulture && o.Name == culture.Name))
                return null;

            var item = this.TextRepo.GetTextValueByTextIdAndCulture(textId, culture.Name);
            if (item != null)
            {
                request.UpdateFrom(item);
            }
            else
            {
                request.CultureName = culture.Name;
            }

            return request;
        }

        public EditTextResult SaveText(EditTextRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new EditTextResult(request);

            var newTextId = this.TextRepo.SetTextValue(
                request.TextId,
                request.CultureName,
                request.Title,
                request.Value,
                DateTime.UtcNow,
                request.UserId);
            if (!newTextId.HasValue)
            {
                result.Errors.Add(EditTextError.SetTextValueFailed, NetworksEnumMessages.ResourceManager);
                return result;
            }

            result.InsertedTextId = newTextId.Value;
            result.Succeed = true;
            return result;
        }

        public IList<EditTextRequest> GetAllByTextId(int? textId)
        {
            return this.Services.SupportedCultures.Where(o => !o.IsNeutralCulture).Select(o => this.GetEditRequest(textId, o)).ToList();
        }
    }
}
