﻿using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WoW.Crawler.Model.DTO;
using WoW.Crawler.Model.Enum;
using WoW.Crawler.Service.Client.Contract;

namespace WoW.Crawler.Service.Client
{
    public class AuctionClient : BattleNetClientBase, IAuctionClient
    {
        #region Constructors

        public AuctionClient()
        { }

        #endregion Constructors

        #region Public Members

        public async Task<AuctionDataStatusDto> GetAuctionDataStatusAsync(string realm, Region region)
        {
            // Build relative URL.
            var relativeUrl = this.BuildRelativeUrlWithQueryStr(
                String.Format("wow/auction/data/{0}", Uri.EscapeDataString(realm)));

            // Make GET request.
            var response = await this.GetClient(region).GetAsync(relativeUrl);
            response.EnsureSuccessStatusCode();

            var responseContent = await this.DeserializeContentAsync<AuctionDataStatusDto>(response.Content);
            return responseContent;
        }

        public async Task<AuctionListDto> GetAuctionListAsync(string realm, Region region)
        {
            // Get URL information.
            var auctionDataStatus = await this.GetAuctionDataStatusAsync(realm, region);
            if (auctionDataStatus.Files.Count() <= 0) return new AuctionListDto();

            // Make GET request.
            var response = await this.GetClient(region).GetAsync(auctionDataStatus.Files.First().Url);
            response.EnsureSuccessStatusCode();

            // Deserialize content.
            var responseContent = await this.DeserializeContentAsync<AuctionListDto>(response.Content);

            // Set post-processing fields.
            foreach (var auction in responseContent.Auctions)
            {
                auction.Region = region;
            }

            return responseContent;
        }

        #endregion Public Members
    }
}