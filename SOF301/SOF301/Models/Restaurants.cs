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
            Orders = new HashSet<Orders>();
            Requests = new HashSet<Requests>();
        }

        [Key]
        public int RestaurantID { get; set; }

        [StringLength(36,MinimumLength = 8, ErrorMessage = "Restaurant name must between 8 and 36 characters.")]
        [DataType(DataType.Text, ErrorMessage = "Restaurant name has invalid characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Restaurant name has invalid characters.")]
        public string Name { get; set; }

        public int? CityID { get; set; }

        public int? DistrictID { get; set; }


        [StringLength(256, MinimumLength = 8, ErrorMessage = "Restaurant address must between 8 and 256 characters.")]
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
        public virtual ICollection<Orders> Orders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Requests> Requests { get; set; }

        public virtual Users Users { get; set; }
    }
}
