using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOF301.Models
{
    public class SOFEntity
    {
        public static SofModel entity = null;

        public static SofModel getDb()
        {
            if(entity == null)
            {
                entity = new SofModel();
            }
            return entity;
        }
    }
}