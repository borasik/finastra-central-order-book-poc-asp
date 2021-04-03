﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace central_order_book_poc.Model
{
    public class OrderEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string OrderJsonDetails { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
