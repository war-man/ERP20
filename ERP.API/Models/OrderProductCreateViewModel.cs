﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP.API.Models
{
    public class OrderProductCreateViewModel
    {
        public OrderProductCreateViewModel() { }

        [Key]
        

        public int? op_quantity { get; set; }

        public byte? op_status { get; set; }

        public string op_note { get; set; }

        public DateTime? op_datetime { get; set; }



        public int? product_id { get; set; }

        public int? customer_order_id { get; set; }
    }
}