
namespace Sparkle.Services.Networks
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Services.Networks.Clubs;

    public interface IClubsService
    {
        ClubModel GetById(int clubId);

        IList<ClubModel> SelectAll(bool allNetworks = false);

        ClubModel GetByAlias(string alias);

        int Count();

        string MakeAlias(string clubName);

        EditClubResult Create(EditClubRequest model);
        EditClubRequest GetEditRequest(int id);
        EditClubResult Edit(EditClubRequest model);

        void Delete(int id);
    }
}
