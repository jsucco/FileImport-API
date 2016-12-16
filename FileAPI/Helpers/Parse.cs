using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Reflection; 

namespace MCS.Helpers
{
    public class Parse<t> where t : class, new()
    {
        public Parse(string parser)
        {
            //delimiters.Add("sStartDate", new int[5] { 19, 17, 15, 23, 25 });
            //delimiters.Add("eStartDate", new int[5] { 25, 23, 21, 29, 31 });
            //delimiters.Add("eEndDate", new int[5] { 313, 311, 309, 317, 319 });
            setParserType(parser);

        }
        private Dictionary<string, SubStringLocations> ObjectColumnInfo;
        private t mappedObject = new t(); 
        public t mapStringToObject(string line)
        {
            t obj = new t();
            Type busType = mappedObject.GetType();
            PropertyInfo[] objProperties = busType.GetProperties(); 
            foreach (PropertyInfo info in objProperties)
            {
                if (info != null && info.CanWrite)
                {
                    mapProperty(info, line); 
                }
            }
            return mappedObject;
        }
        private void mapProperty(PropertyInfo info, string line)
        {
            
            try
            {
                if (ObjectColumnInfo.ContainsKey(info.Name))
                {
                    SubStringLocations PropertyDelimiter = ObjectColumnInfo[info.Name]; 
                    if (PropertyDelimiter != null)
                    {
                        string spliced = line.Substring(PropertyDelimiter.startIndex, PropertyDelimiter.stringLength);
                        if (spliced.Length > 0)
                        {
                            info.SetValue(mappedObject, ConvertToType(spliced, info));
                        }
                    }

                }
            } catch (Exception e) 
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
        }
        private object ConvertToType(string spliced, PropertyInfo info)
        {
            object returnObject = new object();

            switch (info.PropertyType.Name)
            {
                case "int":
                    returnObject = ConvertToInt(spliced);
                    break;
                case "decimal":
                    returnObject = ConvertToDecimal(spliced);
                    break;
                case "string":
                    returnObject = spliced;
                    break; 
                default:
                    returnObject = null;
                    break; 

            }

            return returnObject; 
        }
        private int ConvertToInt(string spliced)
        {
            Int32 retObject = 0; 
            try
            {
                retObject = Convert.ToInt32(spliced); 
            } catch (Exception e)
            {

            }
            return retObject; 
        }
        private decimal ConvertToDecimal(string spliced)
        {
            decimal retObject = 0; 
            try
            {
                retObject = Convert.ToDecimal(spliced); 
            } catch (Exception e)
            {

            }
            return retObject; 
        }
        
        private void setParserType(string parser)
        {
            parser = parser.ToUpper();
            switch (parser)
            {
                case "MCS":
                    setColumnInfo_CAR();
                    break;
                default:
                    setColumnInfo_CAR();
                    break;
                
            }
        }
        public void setColumnInfo_CAR()
        {
            Dictionary<string, SubStringLocations> ColumnInfo = new Dictionary<string, SubStringLocations>();

            ColumnInfo.Add("Operation", new SubStringLocations { matchingObjectColumn = "Operation", startIndex = 0, stringLength = 1 });
            ColumnInfo.Add("MachineNo", new SubStringLocations { matchingObjectColumn = "MachineNo", startIndex = 1, stringLength = 2 });
            ColumnInfo.Add("JobOrderNo", new SubStringLocations { matchingObjectColumn = "JobOrderNo", startIndex = 3, stringLength = 12 });
            ColumnInfo.Add("stock", new SubStringLocations { matchingObjectColumn = "stock", startIndex = 15, stringLength = 6 });
            ColumnInfo.Add("KgOfFabric", new SubStringLocations { matchingObjectColumn = "KgOfFabric", startIndex = 33, stringLength = 4 });
            ColumnInfo.Add("NoProgram1", new SubStringLocations { matchingObjectColumn = "NoProgram1", startIndex = 37, stringLength = 5 });
            ColumnInfo.Add("NoProgram2", new SubStringLocations { matchingObjectColumn = "NoProgarm2", startIndex = 42, stringLength = 5 });
            ColumnInfo.Add("NoProgram3", new SubStringLocations { matchingObjectColumn = "NoProgram3", startIndex = 47, stringLength = 5 });
            ColumnInfo.Add("NoProgram4", new SubStringLocations { matchingObjectColumn = "NoProgram4", startIndex = 52, stringLength = 5 });
            ColumnInfo.Add("NoProgram5", new SubStringLocations { matchingObjectColumn = "NoProgram5", startIndex = 57, stringLength = 5 });
            ColumnInfo.Add("StandardTimeMin", new SubStringLocations { matchingObjectColumn = "StandardTimeMin", startIndex = 62, stringLength = 5 });
            ColumnInfo.Add("FreeFieldcode1", new SubStringLocations { matchingObjectColumn = "FreeFieldcode1", startIndex = 65, stringLength = 6 });
            ColumnInfo.Add("FreeFieldcode2", new SubStringLocations { matchingObjectColumn = "FreeFieldcode2", startIndex = 71, stringLength = 6 });
            ColumnInfo.Add("FreeFieldcode3", new SubStringLocations { matchingObjectColumn = "FreeFieldcode3", startIndex = 77, stringLength = 6 });
            ColumnInfo.Add("FreeFieldDesc1", new SubStringLocations { matchingObjectColumn = "FreeFieldDesc1", startIndex = 83, stringLength = 30 });
            ColumnInfo.Add("FreeFieldDesc2", new SubStringLocations { matchingObjectColumn = "FreeFieldDesc2", startIndex = 113, stringLength = 30 });
            ColumnInfo.Add("FreeFieldDesc3", new SubStringLocations { matchingObjectColumn = "FreeFieldDesc3", startIndex = 143, stringLength = 30 });
            ColumnInfo.Add("Note1", new SubStringLocations { matchingObjectColumn = "Note1", startIndex = 173, stringLength = 60 });
            ColumnInfo.Add("Note2", new SubStringLocations { matchingObjectColumn = "Note2", startIndex = 233, stringLength = 60 });
            ColumnInfo.Add("MachineEfficiency", new SubStringLocations { matchingObjectColumn = "MachineEfficiency", startIndex = 293, stringLength = 6 });
            ColumnInfo.Add("ActualWorkTime_S", new SubStringLocations { matchingObjectColumn = "ActualWorkTime_S", startIndex = 299, stringLength = 5 });
            ColumnInfo.Add("WorkDuration_S", new SubStringLocations { matchingObjectColumn = "WorkDuration_S", startIndex = 304, stringLength = 5 });
            ColumnInfo.Add("FreeFieldCode4", new SubStringLocations { matchingObjectColumn = "FreeFieldCode4", startIndex = 321, stringLength = 6 });
            ColumnInfo.Add("FreeFieldDesc4", new SubStringLocations { matchingObjectColumn = "FreeFieldDesc4", startIndex = 327, stringLength = 30 });
            ColumnInfo.Add("IntWRespToPrevBatch_M", new SubStringLocations { matchingObjectColumn = "IntWRespToPrevBatch_M", startIndex = 357, stringLength = 5 });
            ColumnInfo.Add("WaterCons_Lt10", new SubStringLocations { matchingObjectColumn = "WaterCons_Lt10", startIndex = 362, stringLength = 5 });
            ColumnInfo.Add("SteamCons", new SubStringLocations { matchingObjectColumn = "SteamCons", startIndex = 367, stringLength = 5 });
            ColumnInfo.Add("EnergyCons_kWh", new SubStringLocations { matchingObjectColumn = "EnergyCons_kWh", startIndex = 372, stringLength = 5 });

            ObjectColumnInfo = ColumnInfo;
        }

        public MCS.Models.MCSBleacher mcsRecord(String Line)
        {
            MCS.Models.MCSBleacher newRecord = new MCS.Models.MCSBleacher(); 

            if (Line.Length >= 367)
            {
                newRecord.Operation = Line.Substring(0, 1);
                newRecord.MachineNo = Line.Substring(1, 2);
                newRecord.JobOrderNo = Line.Substring(3, 12);
                newRecord.stock = Line.Substring(15, 6).Trim();

                if (newRecord.Operation == "S")
                {
                    newRecord.StartDate = parseMCSDate(Line, "sStartDate"); 
                    
                } else if (newRecord.Operation == "E")
                {
                    try
                    {
                        newRecord.KgOfFabric = Convert.ToDecimal(Line.Substring(33, 4));
                    }
                    catch (Exception e)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                    }
                    newRecord.NoProgram1 = Line.Substring(37, 5).Trim();
                    newRecord.NoProgram2 = Line.Substring(42, 5).Trim();
                    newRecord.NoProgram3 = Line.Substring(47, 5).Trim();
                    newRecord.NoProgram4 = Line.Substring(52, 5).Trim();
                    newRecord.NoProgram5 = Line.Substring(57, 5).Trim();
                    try
                    {
                        newRecord.StandardTimeMin = Convert.ToInt64(Line.Substring(62, 3));
                    }
                    catch (Exception e)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                    }
                    newRecord.FreeFieldcode1 = Line.Substring(65, 6).Trim();
                    newRecord.FreeFieldcode2 = Line.Substring(71, 6).Trim();
                    newRecord.FreeFieldcode3 = Line.Substring(77, 6).Trim();
                    newRecord.FreeFieldDesc1 = Line.Substring(83, 30).Trim();
                    newRecord.FreeFieldDesc2 = Line.Substring(113, 30).Trim();
                    newRecord.FreeFieldDesc3 = Line.Substring(143, 30).Trim();
                    newRecord.Note1 = Line.Substring(173, 60).Trim();
                    newRecord.Note2 = Line.Substring(233, 60).Trim(); 
                    try
                    {
                        newRecord.MachineEfficiency = Convert.ToDecimal(Line.Substring(293, 6)); 
                    } catch (Exception e)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(e); 
                    }
                    try
                    {
                        newRecord.ActualWorkTime_S = Convert.ToInt32(Line.Substring(299, 5));
                    } catch (Exception e)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(e); 
                    }
                    try
                    {
                        newRecord.WorkDuration_S = Convert.ToInt32(Line.Substring(304, 5));
                    } catch (Exception e)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(e); 
                    }
                    newRecord.FreeFieldCode4 = Line.Substring(321, 6);
                    newRecord.FreeFieldDesc4 = Line.Substring(327, 30);
                    try
                    {
                        newRecord.IntWRespToPrevBatch_M = Convert.ToInt32(Line.Substring(357, 5));

                    } catch (Exception e)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(e); 
                    }
                    try
                    {
                        newRecord.WaterCons_Lt10 = Convert.ToInt32(Line.Substring(362, 5));
                    } catch (Exception e)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(e); 
                    } 
                    try
                    {
                        newRecord.SteamCons = Convert.ToInt32(Line.Substring(367, 5));
                    } catch (Exception e)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                    }
                    try
                    {
                        newRecord.EnergyCons_kWh = Convert.ToInt32(Line.Substring(372, 5));
                    } catch (Exception e)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(e); 
                    }
                    newRecord.StartDate = parseMCSDate(Line, "eStartDate");
                    newRecord.WorkEndDate = parseMCSDate(Line, "eEndDate");
                }
            } 

            return newRecord; 
        }

        private static DateTime parseMCSDate(string LineBlob, string Key)
        {
            DateTime endDate = new DateTime(1900, 1, 1); 

            try
            {
                //int[] delimitArr = delimiters[Key];
                //int year = Convert.ToInt32(LineBlob.Substring(delimitArr[0], 4));
                //int month = Convert.ToInt32(LineBlob.Substring(delimitArr[1], 2));
                //int day = Convert.ToInt32(LineBlob.Substring(delimitArr[2], 2));
                //int hour = Convert.ToInt32(LineBlob.Substring(delimitArr[3], 2));
                //int min = Convert.ToInt32(LineBlob.Substring(delimitArr[4], 2));

               //endDate = new DateTime(year, month, day, hour, min, 0);
            } catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            return endDate;
        }
    }

    

    public class SubStringLocations
    {
        public string matchingObjectColumn;
        public int startIndex;
        public int stringLength;
    }
}