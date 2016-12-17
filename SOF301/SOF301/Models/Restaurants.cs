namespace SOF301.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Restaurants
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Restaurants()
        {
            Foods = new HashSet<Foods>();
            Requests = new HashSet<Requests>();
        }

        [Key]
        public int RestaurantID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int? CityID { get; set; }

        public int? DistrictID { get; set; }

        [StringLength(256)]
        public string Address { get; set; }

        public int? UserID { get; set; }

        public TimeSpan? StartingHour { get; set; }

        public TimeSpan? FinishingHour { get; set; }

        public bool? RestaurantStatu { get; set; }

        public virtual Cities Cities { get; set; }

        public virtual Districts Districts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Foods> Foods { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Requests> Requests { get; set; }

        public virtual Users Users { get; set; }
    }
}
