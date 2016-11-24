namespace SOF301.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderItems
    {
        [Key]
        public int OrderItemID { get; set; }

        public int? OrderID { get; set; }

        public int? FoodID { get; set; }

        public int? FoodNumber { get; set; }

        public virtual Foods Foods { get; set; }

        public virtual Orders Orders { get; set; }
    }
}
