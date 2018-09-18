using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Models
{
    public class AdvancdedSearchModel
    {

        [Display(Name = "Status")]
        public int StatusId { get; set; }


         [Display(Name = "Job")]
        public string Job { get; set; }
        public SelectList StatusList { get; set; }


        public SelectList JobList { get; set; }
    }
}