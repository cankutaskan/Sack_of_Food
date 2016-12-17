namespace SOF301.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Requests
    {
        [Key]
        public int RequestID { get; set; }

        public int UserID { get; set; }

        public int RestaurantID { get; set; }

        public virtual Restaurants Restaurants { get; set; }

        public virtual Users Users { get; set; }
    }
}
