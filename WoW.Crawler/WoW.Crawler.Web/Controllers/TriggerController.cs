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
            // Get all the realms on EU and US.
            var allRealms = (await this._realmService.GetAllRealmsAsync()).Realms;

            // Create tasks for sending messages to the next worker.
            var sendMessageTasks = allRealms.Select(realm =>
            {
                var id = Guid.NewGuid();
                var body = JsonConvert.SerializeObject(realm, Formatting.None);

                return this._queueClient.SendMessageAsync(id, body);
            });

            // Await all tasks.
            await Task.WhenAll(sendMessageTasks);

            return this.Request.CreateResponse(HttpStatusCode.Accepted);
        }

        #endregion Endpoints
    }
}