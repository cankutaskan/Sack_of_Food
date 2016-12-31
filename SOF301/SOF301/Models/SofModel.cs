namespace SOF301.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SofModel : DbContext
    {
        public SofModel()
            : base("name=SofModel1")
        {
        }

        public virtual DbSet<Cities> Cities { get; set; }
        public virtual DbSet<Districts> Districts { get; set; }
        public virtual DbSet<Foods> Foods { get; set; }
        public virtual DbSet<OrderItems> OrderItems { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Requests> Requests { get; set; }
        public virtual DbSet<Restaurants> Restaurants { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurants>()
                .HasMany(e => e.Requests)
                .WithRequired(e => e.Restaurants)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Requests)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(false);
        }
    }
}
