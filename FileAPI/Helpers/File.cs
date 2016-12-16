using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Models;

namespace MCS.Helpers
{
    public class File
    {
        private HashSet<string> FileRows = new HashSet<string>();

        private int fileSourceId = 0;

        private string fileLocation = "";

        private int newRowLimit = 250; 

        public File(int? SourceId, string Location)
        {
            if (SourceId == null)
                SourceId = 0; 

            fileSourceId = (int)SourceId;

            if (Location.Length == 0)
                throw new Exception("Must provide a location of the file");

            fileLocation = Location; 


            fillHashFromDb(); 
        }

        private void fillHashFromDb()
        {
            using (var _db = new BleacherDb())
            {
                var imports = (from x in _db.MCSImports where x.SourceId == fileSourceId && x.Location == fileLocation select x).ToList(); 

                if (imports != null && imports.Count > 0)
                {

                    foreach (MCSImport item in imports)
                    {
                        if (item.LineBlob.Trim().Length > 0)
                            FileRows.Add(item.LineBlob.Trim()); 
                    }

                    if (FileRows.Count > 0)
                        updateHashCache(); 
                }
            }
        }

        public List<Models.MCSImport> getNewRows(List<Models.MCSImport> postedRecords)
        {
            List<Models.MCSImport> newRows = new List<Models.MCSImport>();

            if (FileRows == null)
                throw new Exception("File Hash not initialized"); 

            if (postedRecords != null)
            {
                foreach (Models.MCSImport row in postedRecords)
                {
                    if (newRows.Count >= newRowLimit)
                        break;
                    if (!FileRows.Contains(row.LineBlob.Trim()) && row.LineBlob.Length > 0)
                    {  
                        newRows.Add(row); 
                    }
                }
            }

            return newRows; 
        }

        public void addRowsToFileHash(List<Models.MCSImport> newRows)
        {
            if (newRows != null && newRows.Count > 0)
            {
                foreach (MCSImport item in newRows)
                {
                    FileRows.Add(item.LineBlob);
                }

                updateHashCache();
            }
        }
        private void updateHashCache()
        {
            HttpContext.Current.Cache.Insert("FileHash." + fileSourceId.ToString() + "." + fileLocation.ToUpper(), FileRows, null, DateTime.Now.AddDays(2), System.Web.Caching.Cache.NoSlidingExpiration); 
        }
    }
}