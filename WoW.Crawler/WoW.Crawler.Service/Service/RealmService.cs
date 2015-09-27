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

        public async Task<RealmListDto> GetRealmList(Region region)
        {
            // Get the realm list for the requested region.
            var realmList = await this._realmClient.GetRealmList(region);

            return realmList;
        }

        #endregion Public Members
    }
}