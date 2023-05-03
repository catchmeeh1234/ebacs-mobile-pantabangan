using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using eBACSMobileV2.Resources.tables;
using SQLite;
using Xamarin.Essentials;

namespace eBACSMobileV2
{
    [Activity(Label = "UploadDataActivity")]
    public class UploadDataActivity : Activity
    {

        Button startupload;

        TextView customer, bill,historyreader,findi,mtrreport;
        ProgressBar progg;


        WebClient mclientbills, mclientcustomer, mReaderHistory,mfindings,mmeterreport;
        List<tblbillsSQLite> mBills;
        List<tblReaderHistory> readhistory;
        List<tblserverSQLite> serverip;
        List<tblfindings> findings;
        List<tblMeterReadingReport> meterreport;


        Uri mUrl;

        string folder;
        int pro;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UploadData);
            // Create your application here

            startupload = FindViewById<Button>(Resource.Id.btnupload);
            customer = FindViewById<TextView>(Resource.Id.lblcustomer);
            bill = FindViewById<TextView>(Resource.Id.lblbill);
            historyreader = FindViewById<TextView>(Resource.Id.lblhistory);
            findi = FindViewById<TextView>(Resource.Id.lblfindings);
            mtrreport = FindViewById<TextView>(Resource.Id.lblmeterreport);
            progg = FindViewById<ProgressBar>(Resource.Id.prog);



            mBills = new List<tblbillsSQLite>();
            readhistory = new List<tblReaderHistory>();

            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);




            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {

                    connection.CreateTable<tblReaderHistory>();
                    mBills = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite");
                    serverip = connection.Query<tblserverSQLite>("SELECT * FROM tblserverSQLite");
                    readhistory = connection.Query<tblReaderHistory>("SELECT * FROM tblReaderHistory");
                    findings = connection.Query<tblfindings>("SELECT * FROM tblfindings");
                    meterreport = connection.Query<tblMeterReadingReport>("SELECT * FROM tblMeterReadingReport");
                }


            }
            catch (Exception ex)
            {

                Android.Widget.Toast.MakeText(Android.App.Application.Context, "SQLiteEX" + ex.Message, ToastLength.Long).Show();
                //Log.Info("SQLiteEX", ex.Message);
            }


           // Android.Widget.Toast.MakeText(Android.App.Application.Context, "Reader " + readhistory.Count, ToastLength.Long).Show();
            startupload.Click += Startupload_Click;
        }

        private void Startupload_Click(object sender, EventArgs e)
        {

            var current = Connectivity.NetworkAccess;

            if (current == Xamarin.Essentials.NetworkAccess.Internet)
            {

                
                startupload.Text = "Uploading Please Wait";
                startupload.Enabled = false;


                bill.Text = "";
                customer.Text = "";
                historyreader.Text = "";
                findi.Text = "";
                mtrreport.Text = "";


                progg.Visibility = ViewStates.Visible;

                pro = 0;



                //insert system history
                string myDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");
                tblsystemhistory systemhistorysave = new tblsystemhistory()
                {

                    User = "" + Intent.GetStringExtra("UserName"),
                    Remarks = "Upload Data To server",
                    Datee = "" + myDate,



                };

                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    connection.Insert(systemhistorysave);
                }

                

                for (int i = 0; i < mBills.Count; i++)
                {
                    mclientbills = new WebClient();

                    mUrl = new Uri("http://" + serverip[0].ipaddress + "updatebills.php?BILLID=" + mBills[i].BILLID + "&Reading=" + mBills[i].Reading + "&Consumption=" + mBills[i].Consumption + "&AmountDue=" + mBills[i].AmountDue + "&Discount=" + mBills[i].Discount + "&ReadingDate=" + mBills[i].ReadingDate + "&DueDate=" + mBills[i].DueDate);


                    mclientbills.DownloadDataAsync(mUrl);
                    mclientbills.DownloadDataCompleted += Mclientbills_DownloadDataCompleted;


                }

                bill.Text = "Bill Update: " + mBills.Count;
                pro = pro + 1;
                progvisible();



                if (readhistory.Count == 0)
                {

                    historyreader.Text = "Reader History Table Updated";
                    pro = pro + 1;
                    progvisible();


                }
                else
                {

                    for (int i = 0; i < readhistory.Count; i++)
                    {
                        mReaderHistory = new WebClient();

                        mUrl = new Uri("http://" + serverip[0].ipaddress + "insertreaderhistory.php");
                        NameValueCollection readhistoryparameters = new NameValueCollection();

                        readhistoryparameters.Add("AccountNumber", readhistory[i].AccountNumber);
                        readhistoryparameters.Add("AccountName", readhistory[i].AccountName);
                        readhistoryparameters.Add("BillNumber", readhistory[i].BillNumber);
                        readhistoryparameters.Add("TimeRead", readhistory[i].TimeRead);
                        readhistoryparameters.Add("Reading", readhistory[i].Reading);
                        readhistoryparameters.Add("Reader", readhistory[i].Reader);
                        readhistoryparameters.Add("Cons", readhistory[i].Cons);

                        Console.WriteLine("mUrl Reader" + mUrl.ToString());
                        mReaderHistory.UploadValuesAsync(mUrl, readhistoryparameters);

                    }

                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                    {


                        pro = pro + 1;
                        progvisible();
                        historyreader.Text = "Reader History Table Updated";
                        readhistory = connection.Query<tblReaderHistory>("SELECT * FROM tblReaderHistory");
                    }


                }


                if (findings.Count == 0)
                {

                    findi.Text = "Findings Table Updated";
                    pro = pro + 1;
                    progvisible();


                }
                else
                {

                    for (int i = 0; i < findings.Count; i++)
                    {
                        mfindings = new WebClient();

                        mUrl = new Uri("http://" + serverip[0].ipaddress + "insertfindings.php");
                        NameValueCollection findingsparameters = new NameValueCollection();

                        findingsparameters.Add("AccountNumber", findings[i].AccountNumber);
                        findingsparameters.Add("AccountName", findings[i].AccountName);
                        findingsparameters.Add("TimeRead", findings[i].TimeRead);
                        findingsparameters.Add("Finding", findings[i].Finding);
                        findingsparameters.Add("Reader", findings[i].Reader);


                        mfindings.UploadValuesAsync(mUrl, findingsparameters);

                    }

                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                    {

                        pro = pro + 1;
                        progvisible();
                        findi.Text = "Findings Table Updated";
                        findings = connection.Query<tblfindings>("SELECT * FROM tblfindings");
                    }


                }

                if (meterreport.Count == 0)
                {

                    mtrreport.Text = "Meter Report Table Updated";
                    pro = pro + 1;
                    progvisible();


                }
                else
                {

                    for (int i = 0; i < meterreport.Count; i++)
                    {
                        mmeterreport = new WebClient();

                        mUrl = new Uri("http://" + serverip[0].ipaddress + "insertmeterreadingreport.php");
                        NameValueCollection meterparameters = new NameValueCollection();

                        meterparameters.Add("BillNo", meterreport[i].BillNo);
                        meterparameters.Add("AccountNumber", meterreport[i].AccountNumber);
                        meterparameters.Add("AccountName", meterreport[i].AccountName);
                        meterparameters.Add("TimeRead", meterreport[i].TimeRead);
                        meterparameters.Add("PrevReading", meterreport[i].PrevReading);
                        meterparameters.Add("CurrentReading", meterreport[i].CurrentReading);
                        meterparameters.Add("Consumption", meterreport[i].Consumption);
                        meterparameters.Add("Reader", meterreport[i].Reader);
                        meterparameters.Add("Remarks", meterreport[i].Remarks);


                        mmeterreport.UploadValuesAsync(mUrl, meterparameters);

                    }

                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                    {

                        pro = pro + 1;
                        progvisible();
                        mtrreport.Text = "Meter Report Table Updated";
                        meterreport = connection.Query<tblMeterReadingReport>("SELECT * FROM tblMeterReadingReport");
                    }


                }
            }
            else
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "Please Connect to Wifi", ToastLength.Long).Show();
            }

            
        }

    

        private void Mclientbills_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                
                string json = Encoding.UTF8.GetString(e.Result); //reads in json
                
            });
        }


        private void progvisible()
        {
            if (pro == 4)
            {
                progg.Visibility = ViewStates.Gone;
                startupload.Enabled = true;
                startupload.Text = "Start Upload";
            }
        }
    }
}