namespace ERP.Data.ModelsERP
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("executor")]
    public partial class executor
    {
        [Key]
        public int exe_id { get; set; }

        public int customer_order_id { get; set; }

        public int staff_id { get; set; }

        public int? service_time_id { get; set; }

        [Column(TypeName = "date")]
        public DateTime work_time { get; set; }

        public TimeSpan start_time { get; set; }

        public TimeSpan end_time { get; set; }

        public bool exe_flag_overtime { get; set; }

        public int exe_time_overtime { get; set; }

        public byte exe_status { get; set; }

        public int exe_evaluate { get; set; }

        public string exe_note { get; set; }
    }
}