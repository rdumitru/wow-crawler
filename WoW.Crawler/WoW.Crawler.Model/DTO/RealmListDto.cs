﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Crawler.Model.DTO
{
    public class RealmListDto
    {
        public IEnumerable<RealmDto> Realms { get; set; }
    }
}