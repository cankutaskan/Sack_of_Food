namespace SOF301.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Foods
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Foods()
        {
            OrderItems = new HashSet<OrderItems>();
        }

        [Key]
        public int FoodID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public double? Price { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public int? RestaurantID { get; set; }

        public bool? FoodStatu { get; set; }

        public virtual Restaurants Restaurants { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItems> OrderItems { get; set; }
    }
}
