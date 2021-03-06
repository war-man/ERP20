﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP.API.Models
{
    public class EmailCreateViewModel
    {
        public EmailCreateViewModel() { }
        
        [StringLength(50)]
        public string ema_username { get; set; }

        [StringLength(150)]
        public string ema_password { get; set; }

        [StringLength(50)]
        public string ema_api { get; set; }

        public int? company_id { get; set; }

        public int? ema_pop_or_imap_port { get; set; }

        [StringLength(50)]
        public string ema_pop_or_imap_server { get; set; }

        public int? ema_smtp_port { get; set; }

        [StringLength(50)]
        public string ema_smtp_server { get; set; }

        [StringLength(250)]
        public string ema_note { get; set; }
    }
}