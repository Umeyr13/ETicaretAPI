﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queryies.Product.GetAllProduct
{
    public class GetAllProductQueryResponse
    {
        public int TotalProductCount { get; set; }
        public object Products { get; set; }
    }
}
