using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Crawler.Model.DTO;
using WoW.Crawler.Model.Enum;

namespace WoW.Crawler.Service.Service.Contract
{
    public interface IRealmService
    {
        Task<RealmListDto> GetRealmListAsync(Region region);

        Task<RealmListDto> GetAllRealmsAsync();
    }
}