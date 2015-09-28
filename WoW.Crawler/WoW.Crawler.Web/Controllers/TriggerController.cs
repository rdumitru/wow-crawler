using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WoW.Crawler.Model.DTO;
using WoW.Crawler.Model.Enum;
using WoW.Crawler.Service.Messaging;
using WoW.Crawler.Service.Service.Contract;

namespace WoW.Crawler.Web.Controllers
{
    [RoutePrefix("api/trigger")]
    public class TriggerController : ApiController
    {
        #region Private Fields

        private readonly IRealmService _realmService;
        private readonly IQueueClientWrapper<RealmDto> _queueClient;

        #endregion Private Fields

        #region Constructors

        public TriggerController(IRealmService realmService, IQueueClientWrapper<RealmDto> queueClient)
        {
            this._realmService = realmService;
            this._queueClient = queueClient;
        }

        #endregion Constructors

        #region Endpoints

        [HttpGet, Route("ping")]
        public IHttpActionResult Ping()
        {
            return Ok();
        }

        [HttpPost, Route("")]
        public async Task<HttpResponseMessage> Trigger()
        {
            var allRealms = (await this._realmService.GetAllRealms()).Realms;
            foreach (var realm in allRealms)
            {
                var id = Guid.NewGuid();
                var body = JsonConvert.SerializeObject(realm, Formatting.None);
                await this._queueClient.SendMessageAsync(id, body);
            }

            return this.Request.CreateResponse(HttpStatusCode.Accepted);
        }

        #endregion Endpoints
    }
}