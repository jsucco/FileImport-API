using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;
using System.IO;
using System.Web.Script.Serialization;

namespace MCS.Helpers
{
    public class MCSAdapter
    {
        private static JavaScriptSerializer json = new JavaScriptSerializer();

        public static async Task<Models.IndexViewModel> getImportRecords(DateTime fromdate, DateTime todate)
        {
            List<Models.FileImportview> records = null;
            Models.IndexViewModel viewObject = new Models.IndexViewModel();

            viewObject.datefrom = DateTime.Now.AddDays(-14);
            viewObject.dateto = DateTime.Now.AddDays(1);

            using (var _db = new Models.BleacherDb())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var query = from x in _db.MCSImports
                            join s in _db.Sources on x.SourceId equals s.Id
                            select new Models.FileImportview { id = x.id, LineBlob = x.LineBlob, DocNumber = (int)x.DocNumber, Location = x.Location, Timestamp = (DateTime)x.Timestamp, Source = s.Name }; 
                records = await query.ToListAsync();

                if (records != null && records.Count > 0)
                    viewObject.FileImportview = records; 
            }
            return viewObject;
        }
        public static int updateContents(string fileContents)
        {
            int rowUpdated = 0;
            int sourceId = 1;
            try
            {
                if (fileContents != null)
                {
                    var mappedContents = json.Deserialize<List<MCS.Models.MCSImport>>(fileContents); 

                    if (mappedContents != null)
                    {
                        var importVars = (from x in mappedContents orderby x.id select x).ToArray();

                        if (importVars.Length > 0)
                        {
                            Helpers.File postedFile = new File(importVars[0].SourceId, importVars[0].Location);                            
                            List<Models.MCSImport> newRows = postedFile.getNewRows(mappedContents);

                            if (importVars[0].SourceId == null)
                                return 0;

                            sourceId = Convert.ToInt32(importVars[0].SourceId);

                            if (newRows.Count > 0)
                            {
                                rowUpdated = newRows.Count; // Helpers.MCSAdapter.AddLineBlobs(newRows); 
                                addLines(newRows, sourceId, importVars[0].Location);
                               
                            }
                        }
                    }

                }
            } catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }

            return rowUpdated; 
        }

        public static int limit = 100; 

        public static void addLines(List<MCS.Models.MCSImport> records, int sourceId, string location)
        {
            Task.Run(() => AddLineBlobs(records, sourceId, location));
        }
        
        public static int AddLineBlobs(List<MCS.Models.MCSImport> records,int sourceId, string location)
        {
            int rowsAff = 0;
            if (records != null && records.Count() > 0)
            {

                using (var _db = new Models.BleacherDb())
                {
                    _db.MCSImports.AddRange(records);
                    rowsAff = _db.SaveChanges();

                }
                try
                {
                    addEvent(sourceId, location, rowsAff);
                } catch (Exception e)
                {

                }
               
            } 
            return rowsAff;
        }

        private static void addEvent(int sourceId, string location, int rows)
        {
            using (var _db = new Models.BleacherDb())
            {
                Models.eventLog record = new Models.eventLog();
                record.SourceId = sourceId;
                record.Action = "Add To Import";
                record.Description = rows.ToString() + " rows";
                record.Timestamp = DateTime.Now;
                _db.eventLogs.Add(record);
                _db.SaveChanges(); 
            }
        }
        public static void insertIntoCache(IList<Models.FileImportview> sources, string sessionid)
        {
            if (sources != null && sources.Count > 0)
            {
                HttpRuntime.Cache.Insert("FileImports." + sessionid, sources, null, DateTime.Now.AddDays(4), System.Web.Caching.Cache.NoSlidingExpiration);

            }
        }
        public static List<Models.FileImportview> getFromCache(string sessionid)
        {
            List<Models.FileImportview> imports = new List<Models.FileImportview>(); 

            try
            {
                Object cached = HttpRuntime.Cache["FileImports." + sessionid];

                if (cached != null)
                    imports = (List<Models.FileImportview>)cached; 
            } catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e); 
            }
            return imports; 
        }
       
    }


}