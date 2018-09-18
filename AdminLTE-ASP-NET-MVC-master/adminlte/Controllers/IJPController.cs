using adminlte.Models;
using DataTables.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class IJPController : Controller
    {


        IJPEntities1 DbContext = new IJPEntities1();

        //
        // GET: /IJP/
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult Get([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, AdvancdedSearchModel searchViewModel)
        {
            IQueryable<IJPDetail> query = DbContext.IJPDetails;
            var totalCount = query.Count();

            // searching and sorting
            query = SearchAssets(requestModel, searchViewModel, query);
            var filteredCount = query.Count();


           

            // Paging
            query = query.OrderBy(m=>m.Id).Skip(requestModel.Start).Take(requestModel.Length);



             var data = query.Select(IJP => new
            {
                Id = IJP.Id,
                Job = IJP.Job,
                Experience = IJP.Experience,
                LastDate = IJP.LastDate,
                ApplicationReceived = IJP.ApplicationReceived,
                Quantity = IJP.Quantity,

            }).ToList();
           

            return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);

        }

        private IQueryable<IJPDetail> SearchAssets(IDataTablesRequest requestModel, AdvancdedSearchModel searchViewModel, IQueryable<IJPDetail> query)
        {
            // Apply filters
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                query = query.Where(p => p.Job.Contains(value)
                                         //p.Experience.Contains(value) ||
                                         //p.LastDate.Contains(value) ||
                                         //p.ApplicationReceived.Contains(value)
                                   );
            }


            var statusRes = DbContext.Status
              .Where(m => m.Id == searchViewModel.StatusId)
              .Select(x => new { x.Id, x.Status1 });


            /***** Advanced Search ******/
            if (searchViewModel.StatusId != 0)
                query = query.Where(x => x.Status.Id == searchViewModel.StatusId);

            if (searchViewModel.Job != null)
                query = query.Where(x => x.Job == searchViewModel.Job);

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


        // GET: Asset/Create
        public ActionResult Create()
        {
            var model = new IJPDetailModel();
            model.StatusSelectList = GetStatusSelectList();
            return View("_CreatePartial", model);
        }

        private SelectList GetStatusSelectList(object selectedValue = null)
        {
            return new SelectList(DbContext.Status
                                             //.Where(facilitySite => facilitySite.IsActive && !facilitySite.IsDeleted)
                                             .Select(x => new { x.Id, x.Status1 }),
                                                 "Id",
                                                 "Status1", selectedValue);
        }

        // POST: Asset/Create
        [HttpPost]
        public async Task<ActionResult> Create(IJPDetailModel IJPDetail)
        {

            IJPDetail.StatusSelectList = GetStatusSelectList();

            if (!ModelState.IsValid)
                return View("_CreatePartial", IJPDetail);

            IJPDetail asset = MaptoModel(IJPDetail);

            DbContext.IJPDetails.Add(asset);
            var task = DbContext.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to add the Asset");
                return View("_CreatePartial", IJPDetail);
            }

            return Content("success");
        }

        private IJPDetail MaptoModel(IJPDetailModel assetVM)
        {
            IJPDetail asset = new IJPDetail()
            {
                Id = assetVM.Id,
                Job = assetVM.Job,
                Experience = assetVM.Experience,
                LastDate = assetVM.LastDate,
                ApplicationReceived = assetVM.ApplicationReceived,
                Quantity = assetVM.Quantity,
                //Status = assetVM.Status,
                StatusId = assetVM.StatusId
                
            };

            return asset;
        }



        [HttpGet]
        public ActionResult AdvancedSearch()
        {
            var advancedSearchViewModel = new AdvancdedSearchModel();

            advancedSearchViewModel.StatusList = new SelectList(DbContext.Status
                                                                    .Select(x => new { x.Id, x.Status1 }),
                                                                      "Id",
                                                                      "Status1");

            advancedSearchViewModel.JobList = new SelectList(DbContext.IJPDetails
                                                                           .GroupBy(x => x.Job)
                                                                           .Where(x => x.Key != null && !x.Key.Equals(string.Empty))
                                                                           .Select(x => new { Job = x.Key }),
                                                                  "Job",
                                                                  "Job");

            //advancedSearchViewModel.StatusList = new SelectList(new List<SelectListItem>
            //{
            //                                                      new SelectListItem { Text="Issued",Value=bool.TrueString},
            //                                                      new SelectListItem { Text="Not Issued",Value = bool.FalseString}
            //                                                      },
            //                                                      "Value",
            //                                                      "Text"
            //                                                    );

            return View("_AdvancedSearchPartial", advancedSearchViewModel);
        }


        // GET: Asset/Edit/5
        public ActionResult Edit(int id)
        {
            var asset = DbContext.IJPDetails.FirstOrDefault(x => x.Id == id);

            IJPDetailModel assetViewModel = MapToViewModel(asset);

            if (Request.IsAjaxRequest())
                return PartialView("_EditPartial", assetViewModel);
            return View(assetViewModel);
        }

        private SelectList GetFacilitiySitesSelectList(object selectedValue = null)
        {
            return new SelectList(DbContext.Status
                                            //.Where(facilitySite => facilitySite.IsActive && !facilitySite.IsDeleted)
                                            .Select(x => new { x.Id, x.Status1 }),
                                                "Id",
                                                "Status1", selectedValue);
        }


        // POST: Asset/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(IJPDetailModel assetVM)
        {

            assetVM.StatusSelectList = GetFacilitiySitesSelectList(assetVM.StatusId);

            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View(Request.IsAjaxRequest() ? "_EditPartial" : "Edit", assetVM);
            }

            IJPDetail asset = MaptoModel(assetVM);

            DbContext.IJPDetails.Attach(asset);
            DbContext.Entry(asset).State = EntityState.Modified;
            var task = DbContext.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to update the Asset");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View(Request.IsAjaxRequest() ? "_EditPartial" : "Edit", assetVM);
            }

            if (Request.IsAjaxRequest())
            {
                return Content("success");
            }

            return RedirectToAction("Index");

        }

        private IJPDetailModel MapToViewModel(IJPDetail asset)
        {
            var status = DbContext.Status.Where(x => x.Id == asset.StatusId).FirstOrDefault();

            IJPDetailModel iJPDetailModel = new IJPDetailModel()
            {
                Id = asset.Id,
                Job = asset.Job,
                Experience = asset.Experience,
                LastDate = asset.LastDate,
                ApplicationReceived = asset.ApplicationReceived,
                Quantity =  asset.Quantity,


                Status = status != null ? status.Status1 : String.Empty,
                StatusId = status.Id,

                StatusSelectList = new SelectList(DbContext.Status
                                                                    //.Where(fs => fs.IsActive && !fs.IsDeleted)
                                                                    .Select(x => new { x.Id, x.Status1 }),
                                                                      "Id",
                                                                      "Status1", asset.Id)
            };

            return iJPDetailModel;
        }
    }
}
