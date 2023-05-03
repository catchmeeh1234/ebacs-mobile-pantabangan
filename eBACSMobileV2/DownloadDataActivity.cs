using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using eBACSMobileV2.Resources.tables;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Essentials;

namespace eBACSMobileV2
{
    [Activity(Label = "DownloadDataActivity")]
    public class DownloadDataActivity : Activity
    {
        Button startdownload;
        TextView bill, billcharges, announce, rate, findings;
        EditText pass;
        ProgressBar progg;
        WebClient webbills, webannounceclient, webrateclient, webbillcharges,webfindings, webbillsarrears;
        Uri mUrl;

        JavaList<string> monthlist = new JavaList<string>();
        JavaList<string> yearylist = new JavaList<string>();

        ArrayAdapter monthadapter, yearadapter;
        Spinner monthspinner;
        Spinner yearspinner;

        List<tblrate> mRate;
        List<tblserverSQLite> serverip;
        List<tblbills> billlist;
        List<tblbillsSQLite> billlistsqlite;
        List<tblbillcharges> mbillcharges;
        List<tblannouncement> mAnnouncement;
        List<tblFindingList> mfindinglist;

        List<tblAccountsSQLite> accountlist;

        int pro;

        string folder;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DownloadData);
            // Create your application here

            startdownload = FindViewById<Button>(Resource.Id.btndownload);
            bill = FindViewById<TextView>(Resource.Id.lblbill);
            billcharges = FindViewById<TextView>(Resource.Id.lblbillcharge);
            announce = FindViewById<TextView>(Resource.Id.lblannounce);
            rate = FindViewById<TextView>(Resource.Id.lblrate);
            findings = FindViewById<TextView>(Resource.Id.lblfindings);
            pass = FindViewById<EditText>(Resource.Id.txtpass);

            monthspinner = FindViewById<Spinner>(Resource.Id.monthspin);
            yearspinner = FindViewById<Spinner>(Resource.Id.yearspin);

            progg = FindViewById<ProgressBar>(Resource.Id.prog);

            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);


            monthlist.Clear();
            yearylist.Clear();


            monthlist.Add("January");
            monthlist.Add("February");
            monthlist.Add("March");
            monthlist.Add("April");
            monthlist.Add("May");
            monthlist.Add("June");
            monthlist.Add("July");
            monthlist.Add("August");
            monthlist.Add("September");
            monthlist.Add("October");
            monthlist.Add("November");
            monthlist.Add("December");

            int currentMonth = DateTime.Now.Month;
            string previousMonth = "";
       
            DateTime x = DateTime.Now.AddMonths(-1);
            previousMonth = x.ToString("MMMM");
            Console.WriteLine(previousMonth.ToString());
        
            int defaultSelectedMonth = monthlist.IndexOf(previousMonth);

            monthadapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, monthlist);
            monthadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            monthspinner.Adapter = monthadapter;
            monthspinner.SetSelection(defaultSelectedMonth);

            var datenow = DateTime.Now.AddYears(-1).ToString("yyyy");
            var datenowplus10 = DateTime.Now.AddYears(10).ToString("yyyy");
            int datee;

            datee = int.Parse(datenow);
            for (int i = int.Parse(datenow); i < int.Parse(datenowplus10); i++)
            {
                               
                yearylist.Add(datee);
                datee = datee + 1;

            }
            int currentYear = DateTime.Now.Year;
            if(currentMonth == 1)
            {
               currentYear = currentYear - 1;
            }

            int defaultSelectionIndex = yearylist.IndexOf(currentYear);
            yearadapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, yearylist);
            yearadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            yearspinner.Adapter = yearadapter;
            yearspinner.SetSelection(defaultSelectionIndex);
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    connection.CreateTable<tblannounceSQLite>();
                    connection.CreateTable<tblrateSQLite>();
                    connection.CreateTable<tblbillsSQLite>();
                    connection.CreateTable<tblbillchargesSQLite>();
                    connection.CreateTable<tblFindingList>();
                    connection.CreateTable<tblReaderHistory>();
                    connection.CreateTable<tblfindings>();
                    connection.CreateTable<tblMeterReadingReport>();
                    serverip = connection.Query<tblserverSQLite>("SELECT * FROM tblserverSQLite");

                  
                }
            }
            catch (Exception e)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: " + e.Message, ToastLength.Long).Show();
            }

            startdownload.Click += Startdownload_Click;
           
        }

        private void Startdownload_Click(object sender, EventArgs e)
        {

            if (pass.Text =="")
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "Please enter password", ToastLength.Long).Show();
            }
            else
            {

                try
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                    {
                       
                        accountlist = connection.Query<tblAccountsSQLite>("SELECT * FROM tblAccountsSQLite WHERE FullName = '" + Intent.GetStringExtra("UserName") + "'");
                    }
                }
                catch (Exception ex)
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: " + ex.Message, ToastLength.Long).Show();
                }

                if (accountlist.Count == 0)
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "No password found", ToastLength.Long).Show();
                }
                else
                {

                    var current = Connectivity.NetworkAccess;

                    if (current == Xamarin.Essentials.NetworkAccess.Internet)
                    {
                        // Connection to internet is available

                        if (pass.Text == accountlist[0].Password)
                        {
                            bill.Text = "";
                            billcharges.Text = "";
                            announce.Text = "";
                            rate.Text = "";
                            findings.Text = "";


                            startdownload.Text = "Downloading Please wait";
                            startdownload.Enabled = false;

                            pro = 0;
                            progg.Visibility = ViewStates.Visible;


                            //insert system history
                            string myDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");
                            tblsystemhistory systemhistorysave = new tblsystemhistory()
                            {

                                User = "" + Intent.GetStringExtra("UserName"),
                                Remarks = "Download Data From server (" + monthspinner.SelectedItem.ToString() + " " + yearspinner.SelectedItem.ToString() + ")",
                                Datee = "" + myDate,
                               


                            };




                            using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                            {

                                connection.Query<tblannounceSQLite>("Delete FROM tblannounceSQLite");
                                connection.Query<tblbillsSQLite>("Delete FROM tblbillsSQLite");
                                connection.Query<tblrateSQLite>("Delete FROM tblrateSQLite");
                                connection.Query<tblbillchargesSQLite>("Delete FROM tblbillchargesSQLite");
                                connection.Query<tblFindingList>("Delete FROM tblFindingList");
                                connection.Query<tblReaderHistory>("DELETE FROM tblReaderHistory");
                                connection.Query<tblfindings>("DELETE FROM tblfindings");
                                connection.Query<tblMeterReadingReport>("DELETE FROM tblMeterReadingReport");

                                connection.Insert(systemhistorysave);
                            }

                            webbills = new WebClient();
                            mUrl = new Uri("http://" + serverip[0].ipaddress + "selectbills.php?MeterReader=" + Intent.GetStringExtra("UserName").Replace(" ", "+") + "&billingdate=" + monthspinner.SelectedItem.ToString() + "+" + yearspinner.SelectedItem.ToString());


                            webbills.DownloadDataAsync(mUrl);
                            webbills.DownloadDataCompleted += Webbills_DownloadDataCompleted;


                            webbillcharges = new WebClient();
                            mUrl = new Uri("http://" + serverip[0].ipaddress + "selectbillcharges.php?MeterReader=" + Intent.GetStringExtra("UserName").Replace(" ", "+") + "&billingdate=" + monthspinner.SelectedItem.ToString() + "+" + yearspinner.SelectedItem.ToString());


                            webbillcharges.DownloadDataAsync(mUrl);
                            webbillcharges.DownloadDataCompleted += Webbillcharges_DownloadDataCompleted;

                            webrateclient = new WebClient();
                            mUrl = new Uri("http://" + serverip[0].ipaddress + "selectrate.php");

                            webrateclient.DownloadDataAsync(mUrl);
                            webrateclient.DownloadDataCompleted += Webrateclient_DownloadDataCompleted;

                            webannounceclient = new WebClient();
                            mUrl = new Uri("http://" + serverip[0].ipaddress + "selectannounce.php");

                            webannounceclient.DownloadDataAsync(mUrl);
                            webannounceclient.DownloadDataCompleted += Webannounceclient_DownloadDataCompleted;

                            webfindings = new WebClient();
                            mUrl = new Uri("http://" + serverip[0].ipaddress + "selectfindings.php");

                            webfindings.DownloadDataAsync(mUrl);
                            webfindings.DownloadDataCompleted += Webfindings_DownloadDataCompleted;
                        }
                        else
                        {
                            Android.Widget.Toast.MakeText(Android.App.Application.Context, "Wrong Password", ToastLength.Long).Show();
                        }


                    }
                    else
                    {
                        Android.Widget.Toast.MakeText(Android.App.Application.Context, "Please Connect to Wifi", ToastLength.Long).Show();
                    }


                    
                }
               
            }

            

        }

        private void Webfindings_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    string errormessage = Encoding.UTF8.GetString(e.Result);
                    //Console.WriteLine("Error ng PHP" + errormessage);

                    string json = Encoding.UTF8.GetString(e.Result);
                    mfindinglist = JsonConvert.DeserializeObject<List<tblFindingList>>(json);

                    for (int i = 0; i < mfindinglist.Count; i++)
                    {

                        var finding = mfindinglist[i].Finding;
                       
                        tblFindingList findlist = new tblFindingList()
                        {
                            Finding = finding,
                            
                        };

                        using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                        {

                            connection.Insert(findlist);

                        }

                    }//end of for loop

                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Field Findings Update: Success", ToastLength.Long).Show();
                    findings.Text = "Field Findings Update: Success";

                }
                catch (Exception ex)
                {
                    findings.Text = "Field Findings Update: Failed " + ex.Message;
                }

                pro = pro + 1;
                progvisible();
            });
        }

        private void Webannounceclient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    string errormessage = Encoding.UTF8.GetString(e.Result);
                    //Console.WriteLine("Error ng PHP" + errormessage);

                    string json = Encoding.UTF8.GetString(e.Result);
                    mAnnouncement = JsonConvert.DeserializeObject<List<tblannouncement>>(json);
                    
                    for (int i = 0; i < mAnnouncement.Count; i++)
                    {

                        var announceid = mAnnouncement[i].AnnounceID;
                        var announce = mAnnouncement[i].Announce;

                        tblannounceSQLite announced = new tblannounceSQLite()
                        {

                            AnnounceID = announceid,
                            Announce = "" + announce,

                        };

                        using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                        {

                            connection.Insert(announced);
                           
                        }

                    }//end of for loop

                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Announcement Update: Success", ToastLength.Long).Show();
                    announce.Text = "Announcement Update: Success";

                }
                catch (Exception ex)
                {
                    announce.Text = "Announcement Update: Failed" + ex.Message;

                }

                pro = pro + 1;
                progvisible();
            });
        }

        private void Webrateclient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    string errormessage = Encoding.UTF8.GetString(e.Result);
                  
                    string json = Encoding.UTF8.GetString(e.Result);
                    mRate = JsonConvert.DeserializeObject<List<tblrate>>(json);
                    
                    for (int i = 0; i < mRate.Count; i++)
                    {

                        var rateid = mRate[i].RateSchedulesID;
                        var customertype = mRate[i].CustomerType;
                        var mincharge = mRate[i].MinimumCharge;
                        var metersize = mRate[i].MeterSize;
                        var twent = mRate[i].twenty;
                        var thirty = mRate[i].thirty;
                        var forty = mRate[i].forty;
                        var fifty = mRate[i].fifty;
                        var maxx = mRate[i].maxx;

                        tblrateSQLite ratee = new tblrateSQLite()
                        {

                            RateSchedulesID = rateid,
                            CustomerType = "" + customertype,
                            MinimumCharge = mincharge,
                            MeterSize = metersize,
                            twenty = twent,
                            thirty = thirty,
                            forty = forty,
                            fifty = fifty,
                            maxx = maxx,

                        };

                        using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                        {

                            connection.Insert(ratee);

                        }

                    }//end of for loop
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Rate Schedule Update: Success", ToastLength.Long).Show();
                    rate.Text = "Rate Schedule Update: Success";
                }
                catch (Exception ex)
                {
                    rate.Text = "Rate Schedule Update: Failed" + ex.Message;

                }

                pro = pro + 1;
                progvisible();

            });
        }

        private void Webbillcharges_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {

                //try
                //{
                    string errormessage = Encoding.UTF8.GetString(e.Result);
                    //Console.WriteLine("Error ng PHP" + errormessage);

                    string json = Encoding.UTF8.GetString(e.Result);
                    mbillcharges = JsonConvert.DeserializeObject<List<tblbillcharges>>(json);
                    
                    for (int i = 0; i < mbillcharges.Count; i++)
                    {

                        var billchargeID = mbillcharges[i].BillChargesID;
                        var billnumber = mbillcharges[i].BillNumber;
                        var accnumber = mbillcharges[i].AccountNumber;
                        var accname = mbillcharges[i].AccountName;
                        //var forthemonth = mbillcharges[i].ForTheMonthOf;
                        var zone = mbillcharges[i].Zone;
                        var particulars = mbillcharges[i].Particulars;
                        var amount = mbillcharges[i].Amount;
                        var ispaid = mbillcharges[i].IsPaid;

                        tblbillchargesSQLite billchargess = new tblbillchargesSQLite()
                        {

                            BillChargesID = billchargeID,
                            BillNumber = billnumber,
                            AccountNumber = accnumber,
                            AccountName = accname,
                           // ForTheMonthOf = forthemonth,
                            Zone = zone,
                            Particulars = particulars,
                            Amount = amount,
                            IsPaid = ispaid,

                        };

                        using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                        {
                          connection.Insert(billchargess);

                        }

                    }//end of for loop
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Download Bill Charges Complete", ToastLength.Long).Show();
                    billcharges.Text = "Bill Charges Downloaded: " + mbillcharges.Count;
                //}
                //catch (Exception ex)
                //{
                //    billcharges.Text = "Error Download Bill Charges: " + ex.Message;
                //}

                pro = pro + 1;
                progvisible();

            });
        }

        private void Webbills_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    string errormessage = Encoding.UTF8.GetString(e.Result);
                 
                    string json = Encoding.UTF8.GetString(e.Result);
                    billlist = JsonConvert.DeserializeObject<List<tblbills>>(json);
                   
                    for (int i = 0; i < billlist.Count; i++)
                    {

                        var billid = billlist[i].BILLID;
                        var billno = billlist[i].BillNo;
                        var accno = billlist[i].AccountNumber;
                        var custname = billlist[i].CustomerName;
                        var metno = billlist[i].MeterNo;
                        var ratesched = billlist[i].RateSchedule;
                        var custadd = billlist[i].CustomerAddress;
                        var seqno = billlist[i].ReadingSeqNo;
                        var avee = billlist[i].Averagee;
                        var senior = billlist[i].isSenior;
                        var penaltyafter = billlist[i].PenaltyAfterDue;
                        var previousread = billlist[i].PreviousReading;
                        var reading = billlist[i].Reading;
                        var cons = billlist[i].Consumption;
                        var readdate = billlist[i].ReadingDate;
                        var duedate = billlist[i].DueDate;
                        var zone = billlist[i].Zone;
                        var amountdue = billlist[i].AmountDue;
                        var lastrgngdate = billlist[i].LasReadingDate;
                        var metersize = billlist[i].MeterSize;
                        var billingmonth = billlist[i].BillingDate;
                        var ispaid = billlist[i].IsPaid;
                        var billstatus = billlist[i].BillStatus;
                        var meterreader = billlist[i].MeterReader;
                        var advancepayment = billlist[i].AdvancePayment;
                        var arrearbills = billlist[i].ArrearsBill;
                        var arrearcharges = billlist[i].ArrearsCharges;

                        tblbillsSQLite bills = new tblbillsSQLite()
                        {

                            BILLID = billid,
                            BillNo = billno,
                            AccountNumber = "" + accno,
                            CustomerName = "" + custname,
                            MeterNo = "" + metno,
                            RateSchedule = "" + ratesched,
                            CustomerAddress = "" + custadd,
                            ReadingSeqNo = "" + seqno,
                            Averagee = "" + avee,
                            isSenior = "" + senior,
                            PenaltyAfterDue = penaltyafter,
                            PreviousReading = "" + previousread,
                            Reading = "" + reading,
                            Consumption = "" + cons,
                            ReadingDate = "" + readdate,
                            DueDate = "" + duedate,
                            Zone = "" + zone,
                            AmountDue = amountdue,
                            LasReadingDate = "" + lastrgngdate,
                            MeterSize = "" + metersize,
                            BillingDate = "" + billingmonth,
                            IsPaid = "" + ispaid,
                            BillStatus = "" + billstatus,
                            MeterReader = "" + meterreader,
                            AdvancePayment = advancepayment,
                            ArrearsBill = arrearbills,
                            ArrearsCharges = arrearcharges,
                            reprinted = 0,
                        };

                        using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                        {

                            connection.Insert(bills);
                        }

                    }//end of for loop

                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Download Bills Complete", ToastLength.Long).Show();
                    bill.Text = "Bills Downloaded: " + billlist.Count;
                    //progg.Visibility = ViewStates.Gone;
                }
                catch (Exception ex)
                {
                    bill.Text = "Error Downloading Bills: " + ex.Message;
                    Console.WriteLine("Error Downloading Bills: " + ex.Message);
                }

                pro = pro + 1;
                progvisible();
            });
        }

        private void progvisible()
        {
            if (pro == 5)
            {
                progg.Visibility = ViewStates.Gone;
                startdownload.Enabled = true;
                startdownload.Text = "Start Download";
               
            }
        }

    }
}