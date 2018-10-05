using adminlte.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class DataChartsController : Controller
    {

        //DashboardOrderEntities doe = new DashboardOrderEntities();

        //
        // GET: /DataCharts/
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult AdvancedSearch()
        {
            var advancedSearchViewModel = new AdvancdedSearchModel();

            //advancedSearchViewModel.StatusList = new SelectList(DbContext.Status
            //                                                        .Select(x => new { x.Id, x.Status1 }),
            //                                                          "Id",
            //                                                          "Status1");



            //advancedSearchViewModel.JobList = new SelectList(DbContext.IJPDetails
            //                                                               .GroupBy(x => x.Job)
            //                                                               .Where(x => x.Key != null && !x.Key.Equals(string.Empty))
            //                                                               .Select(x => new { Job = x.Key }),
            //                                                      "Job",
            //                                                      "Job");


            return View("_AdvancedSearchPartial", advancedSearchViewModel);

        }


        public ActionResult DataChart()
        {

            using (DashboardOrderEntities _context = new DashboardOrderEntities())
            {
                ViewBag.CountCustomers = _context.Customers.Count();
                ViewBag.CountOrders = _context.Orders.Count();
                ViewBag.CountProducts = _context.Products.Count();
            }

            return View();
        }


        public ActionResult GetDetails(string type)
        {
            List<ProductOrCustomerViewModel> result = GetProductOrCustomer(type);

            return PartialView("~/Views/DataCharts/GetDetails.cshtml", result);

        }


        public List<ProductOrCustomerViewModel> GetProductOrCustomer(string type)
        {
            List<ProductOrCustomerViewModel> result = null;

            using (DashboardOrderEntities _context = new DashboardOrderEntities())
            {
                if (!string.IsNullOrEmpty(type))
                {
                    if (type == "customers")
                    {
                        result = _context.Customers.Select(c => new ProductOrCustomerViewModel
                        {
                            Name = c.CustomerName,
                            Image = c.CustomerImage,
                            TypeOrCountry = c.CustomerCountry,
                            Type = "Customers"

                        }).ToList();

                    }
                    else if (type == "products")
                    {
                        result = _context.Products.Select(p => new ProductOrCustomerViewModel
                        {
                            Name = p.ProductName,
                            Image = p.ProductImage,
                            TypeOrCountry = p.ProductType,
                            Type = p.ProductType

                        }).ToList();

                    }
                }

                return result;
            }
        }



        public ActionResult TopCustomers()
        {
            List<TopCustomerViewModel> topFiveCustomers = null;
            using (DashboardOrderEntities _context = new DashboardOrderEntities())
            {
                var OrderByCustomer = (from o in _context.Orders
                                       group o by o.Customer.ID into g
                                       orderby g.Count() descending
                                       select new
                                       {
                                           CustomerID = g.Key,
                                           Count = g.Count()
                                       }).Take(5);

                topFiveCustomers = (from c in _context.Customers
                                    join o in OrderByCustomer
                                    on c.ID equals o.CustomerID
                                    select new TopCustomerViewModel
                                    {
                                        CustomerName = c.CustomerName,
                                        CustomerImage = c.CustomerImage,
                                        CustomerCountry = c.CustomerCountry,
                                        CountOrder = o.Count
                                    }).ToList();
            }

            return PartialView("~/Views/DataCharts/TopCustomers.cshtml", topFiveCustomers);
        }


        public ActionResult OrdersByCountry()
        {
            DashboardOrderEntities _context = new DashboardOrderEntities();

            var ordersByCountry = (from o in _context.Orders
                                   group o by o.Customer.CustomerCountry into g
                                   orderby g.Count() descending
                                   select new
                                   {
                                       Country = g.Key,
                                       CountOrders = g.Count()
                                   }).ToList();

            return Json(new { result = ordersByCountry }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CustomersByCountry()
        {
            DashboardOrderEntities _context = new DashboardOrderEntities();

            var customerByCountry = (from c in _context.Customers
                                     group c by c.CustomerCountry into g
                                     orderby g.Count() descending
                                     select new
                                     {
                                         Country = g.Key,
                                         CountCustomer = g.Count()
                                     }).ToList();

            return Json(new { result = customerByCountry }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult OrdersByCustomer()
        {
            DashboardOrderEntities _context = new DashboardOrderEntities();
            var ordersByCustomer = (from o in _context.Orders
                                    group o by o.Customer.ID into g
                                    select new
                                    {
                                        Name = from c in _context.Customers
                                               where c.ID == g.Key
                                               select c.CustomerName,

                                        CountOrders = g.Count()

                                    }).ToList();


            return Json(new { result = ordersByCustomer }, JsonRequestBehavior.AllowGet);
        }



    }
}