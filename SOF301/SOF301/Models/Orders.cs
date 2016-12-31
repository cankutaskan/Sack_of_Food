namespace SOF301.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Orders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Orders()
        {
            OrderItems = new HashSet<OrderItems>();
        }

        [Key]
        public int OrderID { get; set; }

        public int? UserID { get; set; }

        public DateTime? Date { get; set; }

        public double? TotalPrice { get; set; }

        [StringLength(11)]
        public string Telephone { get; set; }

        [StringLength(256)]
        public string Address { get; set; }

        public bool? PaymentType { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public byte? OrderStatus { get; set; }

        public int? RestaurantID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItems> OrderItems { get; set; }

        public virtual Restaurants Restaurants { get; set; }

        public virtual Users Users { get; set; }
    }
}
