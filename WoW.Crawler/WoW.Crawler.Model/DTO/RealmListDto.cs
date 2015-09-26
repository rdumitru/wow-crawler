using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Crawler.Model.DTO
{
    public class RealmListDto : DtoBase
    {
        public IEnumerable<Realm> Realms { get; set; }
    }
}