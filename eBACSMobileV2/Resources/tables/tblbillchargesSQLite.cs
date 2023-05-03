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
    public class tblbillchargesSQLite
    {
        public int BillChargesID { get; set; }
        public int BillNumber { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
       // public string ForTheMonthOf { get; set; }
        public string Zone { get; set; }
        public string Particulars { get; set; }
        public decimal Amount { get; set; }
        public string IsPaid { get; set; }
    }
}