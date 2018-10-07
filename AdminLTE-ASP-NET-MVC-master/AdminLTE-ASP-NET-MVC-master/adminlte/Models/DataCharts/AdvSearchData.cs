using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Models.DataCharts
{
    public class AdvSearchData
    {

        [Display(Name = "CustomerEmail")]
        public string CustomerEmail { get; set; }


        [Display(Name = "CustomerPhone")]
        public string CustomerPhone { get; set; }


        public SelectList CustomerEmailList { get; set; }


        public SelectList CustomerPhoneList { get; set; }
    }
}