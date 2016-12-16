using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;

namespace MCS.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public IList<Models.FileImportview> FileImportview { get; set; }
        public IList<select2> sources { get; set; }
        public DateTime datefrom { get; set; }
        public DateTime dateto { get; set; }
    }
    public class FileImportview
    {
        public int id { get; set; }
        public string LineBlob { get; set; }
        public int DocNumber { get; set; }
        public string Location { get; set; }
        public string Source { get; set; }
        public DateTime Timestamp { get; set; }

    }
    public class select2
    {
        public int id { get; set; }
        public string text { get; set; }
    }
    public class SourceViewModel
    {
        public IList<Models.Source> Sources { get; set; }
    }
    public class SourceCrud
    {
        public int Id;
        public string oper;
        public string Name;
        public bool Active; 
    }
    public class sourceDocInfo
    {
        public int DocNumber { get; set; }
        public string LastPath { get; set; }
    }
    public class jqgridData
    {
        public int total = 0;
        public int page = 0;
        public object records;
        public object userdata;
        public object rows; 
    }
    public class jqgridLoad
    {
        public int rows;
        public int PageCnt;
        public bool paging;
        public string Filter1;
        public bool FilterFlag; 
    }
    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }
}