using adminlte.Models;
using adminlte.Models.DataCharts;
using DataTables.Mvc;
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

        DashboardOrderEntities _context = new DashboardOrderEntities();
        //
        // GET: /DataCharts/
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult AdvancedSearchCustomer()
        {
            var advSearchData = new AdvSearchData();

            //advSearchData.CustomerEmailList = new SelectList(_context.Customers
            //                                                        .Select(x => new { x.CustomerEmail, x. }),
            //                                                          "Id",
            //                                                          "Status1");



            advSearchData.CustomerEmailList = new SelectList(_context.Customers
                                                                           .GroupBy(x => x.CustomerEmail)
                                                                           .Where(x => x.Key != null && !x.Key.Equals(string.Empty))
                                                                           .Select(x => new { CustomerEmail = x.Key }),
                                                                  "CustomerEmail",
                                                                  "CustomerEmail");


            advSearchData.CustomerPhoneList = new SelectList(_context.Customers
                                                                           .GroupBy(x => x.CustomerPhone)
                                                                           .Where(x => x.Key != null && !x.Key.Equals(string.Empty))
                                                                           .Select(x => new { CustomerPhone = x.Key }),
                                                                  "CustomerPhone",
                                                                  "CustomerPhone");


            return View("AdvancedSearchCustomer", advSearchData);

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



        private IQueryable<Customer> SearchAssets(IDataTablesRequest requestModel, AdvSearchData searchViewModel, IQueryable<Customer> query)
        {
            // Apply filters
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                query = query.Where(p => p.CustomerEmail.Contains(value) ||
                                        p.CustomerPhone.Contains(value) ||
                                        p.CustomerName.Contains(value) ||
                                        p.CustomerCountry.Contains(value)
                                   );

            }


            /***** Advanced Search ******/
            if (searchViewModel.CustomerEmail != null)
                query = query.Where(x => x.CustomerEmail.Equals(searchViewModel.CustomerEmail));

            if (searchViewModel.CustomerPhone != null)
                query = query.Where(x => x.CustomerPhone.Equals(searchViewModel.CustomerPhone));

            /***** Advanced Search ******/

            var filteredCount = query.Count();

            // Sort
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            foreach (var column in sortedColumns)
            {
                orderByString += orderByString != String.Empty ? "," : "";
                orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
            }

            //query = query.OrderBy(orderByString == string.Empty ? "BarCode asc" : orderByString);

            return query;
        }

        public ActionResult GetCustomers([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, AdvSearchData searchViewModel)
        {
            IQueryable<Customer> query = _context.Customers;
            var totalCount = query.Count();

            // searching and sorting
            query = SearchAssets(requestModel, searchViewModel, query);
            var filteredCount = query.Count();




            // Paging
            query = query.OrderBy(m => m.ID).Skip(requestModel.Start).Take(requestModel.Length);



            var data = query.Select(CST => new
            {
                ID = CST.ID,
                CustomerEmail = CST.CustomerEmail,
                CustomerName = CST.CustomerName,
                CustomerPhone = CST.CustomerPhone,
                CustomerCountry = CST.CustomerCountry,
                CustomerImage = CST.CustomerImage
                //Quantity = IJP,

            }).ToList();


            return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);

        }


    }
}