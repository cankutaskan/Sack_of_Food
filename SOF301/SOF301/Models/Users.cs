namespace SOF301.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Users()
        {
            Orders = new HashSet<Orders>();
            Requests = new HashSet<Requests>();
            Restaurants = new HashSet<Restaurants>();
        }

        [Key]
        public int UserID { get; set; }

        public int? RoleID { get; set; }

        [Required]
        [StringLength(20,MinimumLength = 8,ErrorMessage = "User name must between 8 and 20 characters.")]
        [DataType(DataType.Text,ErrorMessage = "User name has invalid characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "User name has invalid characters.")]
        public string UserName { get; set; }

        [Required]
        [StringLength(24, MinimumLength = 8, ErrorMessage = "Password must between 8 and 24 characters.")]
        [DataType(DataType.Password, ErrorMessage = "Password is invalid.")]
        public string Password { get; set; }

        [StringLength(24)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [StringLength(24)]
        [DataType(DataType.Text)]
        public string Surname { get; set; }

        [StringLength(11)]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone number.")]
        public string Telephone { get; set; }

        [StringLength(256)]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        public int? CityID { get; set; }

        
        [StringLength(36, MinimumLength = 8, ErrorMessage = "Email address must between 8 and 36 characters.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email address is invalid.")]
        public string Email { get; set; }

        public int? DistrictID { get; set; }

        public virtual Cities Cities { get; set; }

        public virtual Districts Districts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Orders> Orders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Requests> Requests { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Restaurants> Restaurants { get; set; }

        public virtual Roles Roles { get; set; }
    }
}
