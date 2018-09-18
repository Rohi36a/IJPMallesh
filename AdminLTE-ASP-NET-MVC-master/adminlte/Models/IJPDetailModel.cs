using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Models
{
    public class IJPDetailModel
    {

        public int Id { get; set; }
        public string Job { get; set; }
        public Nullable<decimal> Experience { get; set; }
        public Nullable<System.DateTime> LastDate { get; set; }
        public Nullable<System.DateTime> ApplicationReceived { get; set; }
        public Nullable<int> Quantity { get; set; }


        public string Status { get; set; }


        [Required(ErrorMessage = "Status")]
        [Display(Name = "Status")]
        public int StatusId { get; set; }

        public SelectList StatusSelectList { get; set; }
    }
}