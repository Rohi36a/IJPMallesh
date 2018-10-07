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

        DashboardOrderEntities _context = new DashboardOrderEntities();

        [HttpGet]
        //public ActionResult GetCustomers()
        //{
        //    using (DashboardOrderEntities _context = new DashboardOrderEntities())
        //    {
        //        var customerList = _context.Customers.Select(c => new
        //        {
        //            c.ID,
        //            c.CustomerName,
        //            c.CustomerEmail,
        //            c.CustomerPhone,
        //            c.CustomerCountry,
        //            c.CustomerImage
        //        }).ToList();

        //        return Json(new { data = customerList }, JsonRequestBehavior.AllowGet);
        //    }


        //}


        private IQueryable<Customer> SearchAssets(IDataTablesRequest requestModel, AdvSearchData searchViewModel, IQueryable<Customer> query)
        {
            // Apply filters
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                query = query.Where(p => p.CustomerEmail.Contains(value) ||
                    p.CustomerPhone.Contains(value)
                    //p.LastDate.Contains(value) ||
                    //p.ApplicationReceived.Contains(value)
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