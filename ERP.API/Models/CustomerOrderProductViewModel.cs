﻿using ERP.Data.ModelsERP;
using ERP.Data.ModelsERP.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.API.Models
{
    public class CustomerOrderProductViewModel
    {
        public List<productorderviewmodel> list_product { get; set; }
        public customer customer{get;set ;}
        public int? cuo_total_price { get; set; }

        public byte? cuo_status { get; set; }

        public int? cuo_discount { get; set; }

        public byte? cuo_payment_type { get; set; }

        public byte? cuo_payment_status { get; set; }

        public int? cuo_ship_tax { get; set; }
        public string cuo_address { get; set; }
        
    }
}