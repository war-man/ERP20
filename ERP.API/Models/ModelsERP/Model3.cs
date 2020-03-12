namespace ERP.Data.ModelsERP
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model3 : DbContext
    {
        public Model3()
            : base("name=Model3")
        {
        }

        public virtual DbSet<customer_group> customer_group { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<customer_group>()
                .Property(e => e.cg_thumbnail)
                .IsFixedLength();
        }
    }
}
