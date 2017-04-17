
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    [Repository("ApiKeys")]
    public interface IApiKeysRepository
    {
        IList<ApiKey> GetAll();

        ApiKey GetByKey(string key);

        ApiKey GetById(int id);

        void Attach(ApiKey entity);
    }
}
