using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Crawler.Model.DTO
{
    public class AuctionListDto : DtoBase
    {
        public IEnumerable<Auction> Auctions { get; set; }
    }
}