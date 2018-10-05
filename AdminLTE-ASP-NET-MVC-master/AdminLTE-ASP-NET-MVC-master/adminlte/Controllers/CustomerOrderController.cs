using adminlte.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class CustomerOrderController : Controller
    {
        //
        // GET: /CustomerOrder/
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult CustomersList()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetCustomers()
        {
            using (DashboardOrderEntities _context = new DashboardOrderEntities())
            {
                var customerList = _context.Customers.Select(c => new
                {
                    c.ID,
                    c.CustomerName,
                    c.CustomerEmail,
                    c.CustomerPhone,
                    c.CustomerCountry,
                    c.CustomerImage
                }).ToList();

                return Json(new { data = customerList }, JsonRequestBehavior.AllowGet);
            }


        }
	}
}