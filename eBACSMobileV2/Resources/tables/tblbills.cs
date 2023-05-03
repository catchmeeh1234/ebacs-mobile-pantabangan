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
    public class tblbills
    {
        public int BILLID { get; set; }
        public int BillNo { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string MeterNo { get; set; }
        public string RateSchedule { get; set; }
        public string CustomerAddress { get; set; }
        public string ReadingSeqNo { get; set; }
        public string Averagee { get; set; }
        public string isSenior { get; set; }
        public Decimal PenaltyAfterDue { get; set; }
        public string PreviousReading { get; set; }
        public string Reading { get; set; }
        public string Consumption { get; set; }
        public string ReadingDate { get; set; }
        public string DueDate { get; set; }
        public string Zone { get; set; }
        public Decimal AmountDue { get; set; }
        public string LasReadingDate { get; set; }
        public string MeterSize { get; set; }
        public string BillingDate { get; set; }
        public string IsPaid { get; set; }
        public string BillStatus { get; set; }
        public string MeterReader { get; set; }
        public Decimal AdvancePayment { get; set; }
        public Decimal ArrearsBill { get; set; }
        public Decimal ArrearsCharges { get; set; }
        public Decimal Discount { get; set; }
        public int reprinted { get; set; }
    }
}