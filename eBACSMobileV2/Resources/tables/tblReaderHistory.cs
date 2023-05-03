using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace eBACSMobileV2.Resources.tables
{
    public class tblReaderHistory
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BillNumber { get; set; }
        public string TimeRead { get; set; }
        public string Cons { get; set; }
        public string Reading { get; set; }
        public string Reader { get; set; }
    }
}