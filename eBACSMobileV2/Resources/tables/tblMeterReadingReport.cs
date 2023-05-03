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
    class tblMeterReadingReport
    {
        public string BillNo { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string TimeRead { get; set; }
        public string PrevReading { get; set; }
        public string CurrentReading { get; set; }
        public string Consumption { get; set; }
        public string Reader { get; set; }
        public string Remarks { get; set; }
    }
}