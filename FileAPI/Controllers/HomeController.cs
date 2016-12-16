using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;
using System.IO.Compression;
using System.IO;
using Newtonsoft.Json;

namespace MCS.Controllers
{
    public class HomeController : Controller
    {
        public static string cachedSessionId = ""; 
        public async Task<ActionResult> Index()
        {
            cachedSessionId = ControllerContext.HttpContext.Session.SessionID.ToString(); 

            Models.IndexViewModel viewMod = new Models.IndexViewModel();

            viewMod.sources = await Helpers.SourcesAdapter.getSelectSources() as IList<Models.select2>;

            return View(viewMod); 

        }

        public FileResult Download()
        {
            MemoryStream memoryStream = new MemoryStream();

            var path = Server.MapPath(@"~\CopyToAPI.zip");

            FileStream zipToDownLoad = new FileStream(path, FileMode.Open);

            zipToDownLoad.Position = 0;
            return File(zipToDownLoad, "application/zip", "CopyToAPI.zip");
            
            
        }
        public async Task<ActionResult> getImports()
        {
            Helpers.jqGrid<Models.jqgridLoad> jqgrid = new Helpers.jqGrid<Models.jqgridLoad>();
            Models.jqgridData data = new Models.jqgridData(); 
            System.Collections.Specialized.NameValueCollection RequestParams = ControllerContext.RequestContext.HttpContext.Request.Params;

            Models.jqgridLoad loadVars = jqgrid.getReqParamsAsObject(RequestParams); 

            if (loadVars != null)
            {
                Helpers.jqGrid<Models.FileImportview> jqView = new Helpers.jqGrid<Models.FileImportview>();
                if (loadVars.paging == false && loadVars.FilterFlag == false)
                {
                    Models.IndexViewModel viewObject = new Models.IndexViewModel();
                    viewObject = await Helpers.MCSAdapter.getImportRecords(DateTime.Now.AddDays(-7), DateTime.Now);

                    if (viewObject != null)
                    {


                        if (viewObject.FileImportview.Count > 0)
                        {
                            Helpers.MCSAdapter.insertIntoCache(viewObject.FileImportview, cachedSessionId);
                            Helpers.MCSAdapter.insertIntoCache(viewObject.FileImportview, cachedSessionId + ".displayed");
                        }
                            
                        data.rows = jqView.loadPageRecords(loadVars.PageCnt, loadVars.rows, viewObject.FileImportview);
                        data.records = viewObject.FileImportview.Count;
                        data.page = loadVars.PageCnt;
                        data.total = (int)Math.Ceiling((decimal)viewObject.FileImportview.Count / loadVars.rows);
                    }
                }
                else if (loadVars.FilterFlag == true)
                {
                    List<Models.FileImportview> cachedImports = Helpers.MCSAdapter.getFromCache(cachedSessionId); 

                    if (cachedImports != null && cachedImports.Count > 0)
                    {
                        List<Models.FileImportview> filteredSet = new List<Models.FileImportview>(); 
                        if (loadVars.Filter1 != "ALL")
                        {
                             filteredSet = jqView.FilterType("Source", loadVars.Filter1, cachedImports);
                        } else
                        {
                            filteredSet = Helpers.MCSAdapter.getFromCache(cachedSessionId);
                        }
                        
                        if (filteredSet != null && filteredSet.Count > 0)
                        {
                            Helpers.MCSAdapter.insertIntoCache(filteredSet, cachedSessionId + ".displayed");
                            data.rows = jqView.loadPageRecords(loadVars.PageCnt, loadVars.rows, filteredSet);
                            data.records = filteredSet.Count;
                            data.page = loadVars.PageCnt;
                            data.total = (int)Math.Ceiling((decimal)filteredSet.Count / loadVars.rows);

                        }
                    }
                }
                else
                {
                    List<Models.FileImportview> cachedImports = Helpers.MCSAdapter.getFromCache(cachedSessionId + ".displayed");

                    if (cachedImports != null && cachedImports.Count > 0)
                    {
                        data.rows = jqView.loadPageRecords(loadVars.PageCnt, loadVars.rows, cachedImports);
                        data.records = cachedImports.Count;
                        data.page = loadVars.PageCnt;
                        data.total = (int)Math.Ceiling((decimal)cachedImports.Count / loadVars.rows);
                    }
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet); 
        }
        public async Task<ActionResult> getSources()
        {
            int pageLimit = 25;
            int PageCnt = 1;
            Helpers.jqGrid<Models.Source> jqgrid = new Helpers.jqGrid<Models.Source>(); 
            Models.jqgridData data = new Models.jqgridData(); 
            
            Models.SourceViewModel sources = await Helpers.SourcesAdapter.getAllSources();

            List<Models.Source> sourcesList = sources.Sources as List<Models.Source>; 
            Helpers.SourcesAdapter.insertIntoCache(sourcesList, cachedSessionId); 

            System.Collections.Specialized.NameValueCollection RequestParams = ControllerContext.RequestContext.HttpContext.Request.Params;

            if (RequestParams["rows"] != null)
                pageLimit = Convert.ToInt32(RequestParams["rows"]);

            if (RequestParams["PageCnt"] != null)
                PageCnt = Convert.ToInt32(RequestParams["PageCnt"]);

            if (sources != null)
            {
                data.rows = jqgrid.loadPageRecords(PageCnt, pageLimit, sources.Sources);
                data.records = sources.Sources.Count;
                data.page = PageCnt;
                data.total = (int)Math.Ceiling((decimal)sources.Sources.Count / pageLimit); 
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult editSources()
        {
            System.Collections.Specialized.NameValueCollection RequestParams = ControllerContext.RequestContext.HttpContext.Request.Params;

            bool result = false;
            if (RequestParams.Count > 0)
            {
                Helpers.jqGrid<Models.SourceCrud> jqgrid = new Helpers.jqGrid<Models.SourceCrud>();

                Models.SourceCrud crudVars = jqgrid.getReqParamsAsObject(RequestParams);

                if (crudVars == null)
                    return Json(result, JsonRequestBehavior.AllowGet);

                Models.Source rowToEdit = Helpers.SourcesAdapter.getcachedRow(crudVars.Id, cachedSessionId);
                
                if (rowToEdit != null && crudVars.oper != null)
                {
                    if (crudVars.oper == "edit")
                    {
                        if (Helpers.SourcesAdapter.editSource(crudVars))
                        {
                            result = false;
                            Helpers.SourcesAdapter.editcachedRow(crudVars, cachedSessionId);
                        }
                            
                    }
                    else if (crudVars.oper == "del")
                    {
                        if (Helpers.SourcesAdapter.deleteSourceAsync(rowToEdit))
                        {
                            result = true;
                        }
                        
                    }
                } 
    
            }

            return Json(result, JsonRequestBehavior.AllowGet); 
        }
        
        public async Task<ActionResult> Sources()
        {
            Models.SourceViewModel view = new Models.SourceViewModel();

            view = await Helpers.SourcesAdapter.getAllSources(); 

            return View(view); 
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}