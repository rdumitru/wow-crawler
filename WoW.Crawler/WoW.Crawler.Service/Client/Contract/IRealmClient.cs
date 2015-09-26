using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Model.DTO;
using WoW.Crawler.Model.Enum;

namespace WoW.Crawler.Service.Client.Contract
{
    public interface IRealmClient
    {
        Task<RealmListDto> GetRealmList(Region region);
    }
}