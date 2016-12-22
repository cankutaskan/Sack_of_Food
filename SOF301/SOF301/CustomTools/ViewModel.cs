using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOF301.CustomTools
{
    public class ViewModel
    {
       public IEnumerable<SOF301.Models.Foods> foods { get; set; }
        public IEnumerable<SOF301.Models.OrderItems> basket { get; set; }

    }
}