namespace ERP.Data.ModelsERP
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class service_time
    {
        [Key]
        public int st_id { get; set; }

        public TimeSpan? st_start_time { get; set; }

        public TimeSpan? st_end_time { get; set; }

        [Column(TypeName = "date")]
        public DateTime? st_start_date { get; set; }

        [Column(TypeName = "date")]
        public DateTime? st_end_date { get; set; }

        public byte? st_repeat_type { get; set; }

        public byte? st_sun_flag { get; set; }

        public byte? st_mon_flag { get; set; }

        public byte? st_tue_flag { get; set; }

        public byte? st_wed_flag { get; set; }

        public byte? st_thu_flag { get; set; }

        public byte? st_fri_flag { get; set; }

        public byte? st_sat_flag { get; set; }

        public byte? st_repeat { get; set; }

        public byte? st_repeat_every { get; set; }

        public byte? st_on_the { get; set; }

        public byte? st_on_day_flag { get; set; }

        public int? st_on_day { get; set; }

        public int? customer_order_id { get; set; }
    }
}
