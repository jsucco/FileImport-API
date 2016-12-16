using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;

namespace MCS.Helpers
{
    public class SourcesAdapter
    {
        private static JavaScriptSerializer json = new JavaScriptSerializer();

        public static int getSourceId(String MachineName)
        {
            int sourceId = -1;
            try
            {
                using (var _db = new MCS.Models.BleacherDb())
                {
                    var SourceId = (from x in _db.Sources where x.MachineName == MachineName.Trim() select x.Id).ToArray();
                    if (SourceId != null && SourceId.Length > 0)
                    {
                        sourceId = SourceId[0];
                    }
                }
            } catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            
            return sourceId; 
        }

        public static int AddSource(string responseContent)
        {
            int SourceId = -1;

            try
            {
                MCS.Models.Source newRecord = json.Deserialize<MCS.Models.Source>(responseContent); 

                if (newRecord != null)
                {
                    using (var _db = new MCS.Models.BleacherDb())
                    {
                        List<MCS.Models.Source> currerntMachine = (from x in _db.Sources where x.MachineName == newRecord.MachineName select x).ToList();

                        if (currerntMachine.Count == 0)
                        {
                            newRecord.Active = true; 
                            _db.Sources.Add(newRecord);
                            _db.SaveChanges();
                            SourceId = newRecord.Id; 
                        } else
                        {
                            SourceId = currerntMachine[0].Id;
                        }
                            
                    }
                }
            } catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e); 
            }

            return SourceId; 
        }

        public static async Task<Models.SourceViewModel> getAllSources()
        {
            Models.SourceViewModel viewObject = new Models.SourceViewModel();

            var cachedSource = (List<Models.Source>)HttpContext.Current.Cache["FileSources." + MCS.Controllers.HomeController.cachedSessionId];

            if (cachedSource != null)
            {
                viewObject.Sources = cachedSource;
            } else
            {
                using (var _db = new Models.BleacherDb())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var query = from x in _db.Sources
                                select x;
                    viewObject.Sources = await query.ToListAsync();
                }
            }
            
            return viewObject; 
        }

        public static async Task<List<Models.select2>> getSelectSources()
        {
            List<Models.select2> selects = new List<Models.select2>();

            
            using (var _db = new Models.BleacherDb())
            {
                selects = await (from x in _db.Sources select new Models.select2 { id = x.Id, text = x.Name }).ToListAsync();
            }
            selects.Add(new Models.select2 { id = 0, text = "ALL" });

            selects = (from x in selects orderby x.id descending select x).ToList();
            return selects;
        }
        public static Models.Source getcachedRow(int rowId, string sessionid)
        {
            List<Models.Source> sources = new List<Models.Source>();
            Models.Source row = null; 

            try
            {
                sources = (List<Models.Source>)HttpRuntime.Cache["FileSources." + sessionid];

                if (sources != null && sources.Count > 0)
                {
                    row = sources[rowId - 1];
                }
            } catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e); 
            }

            return row; 
        }

        public static void editcachedRow(Models.SourceCrud edited, string sessionid)
        {
            try
            {
                List<Models.Source> cachedSources = (List<Models.Source>)HttpRuntime.Cache["FileSources." + sessionid];

                if (cachedSources != null)
                {
                    var sourceRow = (from x in cachedSources where x.Id == edited.Id select x).FirstOrDefault();

                    sourceRow.Name = edited.Name;
                    sourceRow.Active = edited.Active;

                    insertIntoCache(cachedSources, sessionid); 
                }
            } catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
        }
        public static bool editSource(Models.SourceCrud source)
        {
            bool result = false; 

            try
            {
                using (var _db = new Models.BleacherDb())
                {
                    var sourceObj = (from x in _db.Sources where x.Id == source.Id select x).FirstOrDefault();

                    if (sourceObj != null)
                    {
                        sourceObj.Name = source.Name;
                        sourceObj.Active = source.Active;
                        int aff = _db.SaveChanges();

                        if (aff > 0)
                            result = true; 
                    }
                }
            } catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e); 
            }
            return result; 
        }

        public static bool deleteSourceAsync(Models.Source source)
        {
            bool result = false; 
            if (source.Id != 1 || source.MachineName != "UNKNOWN")
            {
                using (var _db = new Models.BleacherDb())
                {
                   
                    deleteSourceInCache(source);
               
                    result = true;
                }
            }

            Task.Run(() => deleteSource(source));

            return result;
        }
        public static bool deleteSource(Models.Source source)
        {
            bool result = false;
            
            try
            {
                using (var _db = new Models.BleacherDb())
                {
                    
                    _db.MCSImports.RemoveRange(_db.MCSImports.Where(x => x.SourceId == source.Id).Select(x => x));
                    int rowsAff = _db.SaveChanges();
                    if (rowsAff > 0)
                    {
                        _db.Sources.Remove(_db.Sources.Where(x => x.Id == source.Id).Select(x => x).Take(1).FirstOrDefault());

                        _db.SaveChanges(); 

                        result = true; 
                    }
                }
            } catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }

            return result; 
        }

        public static bool sourceIsUpdate(List<MCS.Models.MCSImport> FileContents)
        {
            bool isUpdating = false;

            try
            {
                var sourceId = (from x in FileContents select x.SourceId).Take(1).ToArray();

                if (sourceId != null && sourceId.Length > 0 )
                {

                    var cacheResult = HttpContext.Current.Cache["UpdateSource_" + sourceId[0].ToString()];

                    if (cacheResult != null && (bool)cacheResult == true)
                        isUpdating = true;
                }


            }
            catch (Exception e)
            {

            }

            return isUpdating;
        }
        public static void setUpdatingSource(List<MCS.Models.MCSImport> FileContents, bool value)
        {
            if (FileContents != null && FileContents.Count > 0)
            {
                try
                {
                    var sourceId = (from x in FileContents select x.SourceId).Take(1).ToArray();

                    if (sourceId != null && sourceId.Length > 0)
                    {
                        HttpContext.Current.Cache.Insert("UpdateSource_" + sourceId[0].ToString(), value, null, DateTime.Now.AddMinutes(15), System.Web.Caching.Cache.NoSlidingExpiration);
                    }

                }
                catch (Exception e)
                {

                }
            }
        }

        public static void insertIntoCache(List<Models.Source> sources, string sessionid)
        {
            if (sources != null && sources.Count > 0)
            {
                HttpRuntime.Cache.Insert("FileSources." + sessionid, sources, null, DateTime.Now.AddDays(4), System.Web.Caching.Cache.NoSlidingExpiration);

            }
        }

        private static bool deleteSourceInCache(Models.Source source)
        {
            bool result = false; 

            try
            {
                List<Models.Source> cacheSources = (List<Models.Source>)HttpRuntime.Cache["FileSources." + MCS.Controllers.HomeController.cachedSessionId]; 

                if (cacheSources != null)
                {
                    cacheSources.Remove(cacheSources.Where(x => x.Id == source.Id).Select(x => x).FirstOrDefault());

                    HttpRuntime.Cache.Insert("FileSources." + MCS.Controllers.HomeController.cachedSessionId, cacheSources, null, DateTime.Now.AddDays(4), System.Web.Caching.Cache.NoSlidingExpiration);
                }
            } catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e); 
            }
            return result; 
        }
    }
}