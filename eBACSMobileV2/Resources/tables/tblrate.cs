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
    public class tblrate
    {

        public int RateSchedulesID { get; set; }
        public string CustomerType { get; set; }
        public decimal MinimumCharge { get; set; }
        public string MeterSize { get; set; }
        public decimal twenty { get; set; }
        public decimal thirty { get; set; }
        public decimal forty { get; set; }
        public decimal fifty { get; set; }
        public decimal maxx { get; set; }


    }
}