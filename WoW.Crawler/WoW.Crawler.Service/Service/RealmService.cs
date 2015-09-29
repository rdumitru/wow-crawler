using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Model.DTO;
using WoW.Crawler.Model.Enum;
using WoW.Crawler.Service.Client.Contract;
using WoW.Crawler.Service.Service.Contract;

namespace WoW.Crawler.Service.Service
{
    public class RealmService : IRealmService
    {
        #region Private Fields

        private readonly IRealmClient _realmClient;

        #endregion Private Fields

        #region Constructors

        public RealmService(IRealmClient realmClient)
        {
            this._realmClient = realmClient;
        }

        #endregion Constructors

        #region Public Members

        public Task<RealmListDto> GetRealmList(Region region)
        {
            // Get the realm list for the requested region.
            var realmListTask = this._realmClient.GetRealmList(region);
            return realmListTask;
        }

        public async Task<RealmListDto> GetAllRealms()
        {
            var allRealms = new List<RealmDto>();
            allRealms.AddRange((await this._realmClient.GetRealmList(Region.EU)).Realms);
            allRealms.AddRange((await this._realmClient.GetRealmList(Region.US)).Realms);

            return new RealmListDto { Realms = allRealms };
        }

        #endregion Public Members
    }
}