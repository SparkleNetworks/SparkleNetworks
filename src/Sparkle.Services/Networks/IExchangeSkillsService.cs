using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface IExchangeSkillsService
    {
        IList<ExchangeSkill> GetActiveExchanges();
        int Add(ExchangeSkill item);
    }
}
