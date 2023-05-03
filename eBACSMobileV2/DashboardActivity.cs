using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using eBACSMobileV2.Resources.tables;
using SQLite;
using Zebra.Sdk.Comm;

namespace eBACSMobileV2
{
    [Activity(Label = "DashboardActivity")]
    public class DashboardActivity : Activity
    {
        TextView username;
        Button downloaddata,uploaddata,startreading;
        Button zeroreading, highconsump, fieldfinding, readingsummary,recon,unread;

        List<tblMeterReadingReport> zeroreadinglist;
        List<tblMeterReadingReport> meterreaderreport;
        List<tblfindings> findinglist;
        List<tblbillsSQLite> summarylist;
        List<tblbillsSQLite> unreadlist;
        List<tblserverSQLite> serverip;
        List<tblannounceSQLite> announce;

        
        string folder;

       public static Connection connection;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Dashboard);


            username = FindViewById<TextView>(Resource.Id.txtuser);
            downloaddata = FindViewById<Button>(Resource.Id.btndownload);
            uploaddata = FindViewById<Button>(Resource.Id.btnupload);
            startreading = FindViewById<Button>(Resource.Id.btnreading);

            unread = FindViewById<Button>(Resource.Id.btnprintunread);

            zeroreading = FindViewById<Button>(Resource.Id.btnprintzeroreading);
            highconsump = FindViewById<Button>(Resource.Id.btnprinthighconsump);
            fieldfinding = FindViewById<Button>(Resource.Id.btnprintfieldfindings);
            readingsummary = FindViewById<Button>(Resource.Id.btnprintsummary);
            recon = FindViewById<Button>(Resource.Id.btnrecon);
            username.Text = Intent.GetStringExtra("UserName");
            //username.Text = "ogag";
            // Create your application here

            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    
                    serverip = connection.Query<tblserverSQLite>("SELECT * FROM tblserverSQLite");
                    announce = connection.Query<tblannounceSQLite>("SELECT * FROM tblannounceSQLite");
                    connection.CreateTable<tblsystemhistory>();
                }

               
            }
            catch (SQLiteException ex)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "SQLiteEX" + ex.Message, ToastLength.Long).Show();
               
            }
          


            connecttoprinter();


            recon.Click += Recon_Click;
            startreading.Click += Startreading_Click;
            downloaddata.Click += Downloaddata_Click;
            uploaddata.Click += Uploaddata_Click;


            zeroreading.Click += Zeroreading_Click;
            highconsump.Click += Highconsump_Click;
            fieldfinding.Click += Fieldfinding_Click;
            readingsummary.Click += Readingsummary_Click;
            unread.Click += Unread_Click;
        }

        private void Unread_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(UnreadActivity));
            intent.PutExtra("UserName", username.Text);
            StartActivity(intent);
        }

        private void Readingsummary_Click(object sender, EventArgs e)
        {

            //connecttoprinter();
            summarylist = new List<tblbillsSQLite>();
            summarylist.Clear();
           

            
            
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    summarylist = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite  Order by BillNo ASC");
                    unreadlist = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite WHERE Reading = '0' Order by ReadingSeqNo ASC");
                    findinglist = connection.Query<tblfindings>("SELECT * FROM tblfindings  Order by AccountNumber ASC");
                    meterreaderreport = connection.Query<tblMeterReadingReport>("SELECT * FROM tblMeterReadingReport WHERE Remarks = 'High Consumption' Order by AccountNumber ASC");

                }


            }
            catch (SQLiteException ex)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "SQLiteEX" + ex.Message, ToastLength.Long).Show();
                //Log.Info("SQLiteEX", ex.Message);
            }

            //try
            //{
                if (summarylist.Count == 0)
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "No Data Found", ToastLength.Long).Show();
                }
                else
                {



                    string myDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");
                    var stringBuilder = new StringBuilder();

                    stringBuilder.Append("\n");
                    stringBuilder.Append("          Republic of the Philippines\n");
                    stringBuilder.Append("        Pantabangan Municipal Water System\n");
                    stringBuilder.Append("             Brgy. East Poblacion\n");
                    stringBuilder.Append("             Pantabangan, Nueva Ecija\n");
                    stringBuilder.Append("          Contact Number: " + announce[1].Announce + "\n");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();

                    stringBuilder.Append("             Meter Reading Report\n");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();

                    stringBuilder.Append("Bill No. | Account No. |  Reading  | Consumption\n");

                    for (int i = 0; i < summarylist.Count; i++)
                    {

                        stringBuilder.Append(summarylist[i].BillNo + "       " + summarylist[i].AccountNumber + "      " + summarylist[i].Reading + "      " + summarylist[i].Consumption + " \n");
                    }


                    stringBuilder.AppendLine();
                    stringBuilder.Append("Summary:\n");
                    stringBuilder.Append("Total: " + summarylist.Count + "\n");
                    stringBuilder.Append("High Consumption: " + meterreaderreport.Count + "\n");
                    stringBuilder.Append("Findings: " + findinglist.Count + "\n");
                    stringBuilder.Append("Unread: " + unreadlist.Count + "\n");
                    stringBuilder.AppendLine();
                    stringBuilder.Append("Reader: " + "" + username.Text);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("Date Printed  " + "" + myDate + "\n");

                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();


                    //connection.Write(cc);
                    // Console.WriteLine("THIS: " + stringBuilder);
                    connection.Write(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
                    //if (connection.Connected)
                    //{
                    //    connection.Close();

                    //}
            }


            //}
            //catch (Exception)
            //{
            //    Android.Widget.Toast.MakeText(Android.App.Application.Context, "No Printer Connected", ToastLength.Long).Show();

            //}
        }

        private void Fieldfinding_Click(object sender, EventArgs e)
        {

            //connecttoprinter();
            findinglist = new List<tblfindings>();
            findinglist.Clear();
            byte[] cc = new byte[] { 0x1B, 0x21, 0x00 };  // 0- normal size text
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    findinglist = connection.Query<tblfindings>("SELECT * FROM tblfindings  Order by AccountNumber ASC");

                }


            }
            catch (SQLiteException ex)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "SQLiteEX" + ex.Message, ToastLength.Long).Show();
                //Log.Info("SQLiteEX", ex.Message);
            }

            try
            {
                if (findinglist.Count == 0)
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "No Field Findings Found", ToastLength.Long).Show();
                }
                else
                {



                    string myDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");
                    var stringBuilder = new StringBuilder();

                    stringBuilder.Append("\n");
                    stringBuilder.Append("          Republic of the Philippines\n");
                    stringBuilder.Append("        Pantabangan Municipal Water System\n");
                    stringBuilder.Append("             Brgy. East Poblacion\n");
                    stringBuilder.Append("             Pantabangan, Nueva Ecija\n");
                    stringBuilder.Append("          Contact Number: " + announce[1].Announce + "\n");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();

                    stringBuilder.Append("             Field Findings Report\n");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();

                    stringBuilder.Append("Account No.    |     Findings \n");

                    for (int i = 0; i < findinglist.Count; i++)
                    {

                        stringBuilder.Append(findinglist[i].AccountNumber + "           " + findinglist[i].Finding +  " \n");
                    }


                    stringBuilder.AppendLine();
                    stringBuilder.Append("Total: " + findinglist.Count + "\n");
                    stringBuilder.AppendLine();
                    stringBuilder.Append("Reader: " + "" + username.Text);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("Date Printed  " + "" + myDate + "\n");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();


                    connection.Write(cc);
                    // Console.WriteLine("THIS: " + stringBuilder);
                    connection.Write(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
                    //if (connection.Connected)
                    //{
                    //    connection.Close();

                    //}
                }


            }
            catch (Exception)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "No Printer Connected", ToastLength.Long).Show();

            }
        }

        private void Highconsump_Click(object sender, EventArgs e)
        {
            //connecttoprinter();

            meterreaderreport = new List<tblMeterReadingReport>();
            meterreaderreport.Clear();
            byte[] cc = new byte[] { 0x1B, 0x21, 0x00 };  // 0- normal size text
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    meterreaderreport = connection.Query<tblMeterReadingReport>("SELECT * FROM tblMeterReadingReport WHERE Remarks = 'High Consumption' Order by AccountNumber ASC");

                }


            }
            catch (SQLiteException ex)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "SQLiteEX" + ex.Message, ToastLength.Long).Show();
                //Log.Info("SQLiteEX", ex.Message);
            }


            try
            {
                if (meterreaderreport.Count == 0)
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "No High Consumption Found", ToastLength.Long).Show();
                }
                else
                {



                    string myDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");
                    var stringBuilder = new StringBuilder();

                    stringBuilder.Append("\n");
                    stringBuilder.Append("          Republic of the Philippines\n");
                    stringBuilder.Append("        Pantabangan Municipal Water System\n");
                    stringBuilder.Append("             Brgy. East Poblacion\n");
                    stringBuilder.Append("             Pantabangan, Nueva Ecija\n");
                    stringBuilder.Append("          Contact Number: " + announce[1].Announce + "\n");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();

                    stringBuilder.Append("             High Consumption Report\n");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();

                    stringBuilder.Append("Bill No. | Account No. |  Reading  | Consumption\n");

                    for (int i = 0; i < meterreaderreport.Count; i++)
                    {

                        stringBuilder.Append(meterreaderreport[i].BillNo + "       " + meterreaderreport[i].AccountNumber + "      " + meterreaderreport[i].CurrentReading + "        " + meterreaderreport[i].Consumption+" \n");
                    }


                    stringBuilder.AppendLine();
                    stringBuilder.Append("Total: " + meterreaderreport.Count + "\n");
                    stringBuilder.AppendLine();
                    stringBuilder.Append("Reader: " + "" + username.Text);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("Date Printed  " + "" + myDate + "\n");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();


                    connection.Write(cc);
                    // Console.WriteLine("THIS: " + stringBuilder);
                    connection.Write(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
                    //if (connection.Connected)
                    //{
                    //    connection.Close();
                        
                    //}

                }


            }
            catch (Exception)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "No Printer Connected", ToastLength.Long).Show();

            }
        }

        private void Recon_Click(object sender, EventArgs e)
        {
           
           
            connecttoprinter();
        }

        public void connecttoprinter()
        {
            
            Task.Factory.StartNew(() =>
            {
                // Do some work on a background thread, allowing the UI to remain responsive
                try
                {
                    //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Please Wait while connecting to printer", ToastLength.Long).Show();

                    //Console.WriteLine("simpleConnectionString" + simpleConnectionString);

                    var simpleConnectionString = $"BT:{serverip[0].printeraddress}";
                    connection = ConnectionBuilder.Build(simpleConnectionString);

                    if (connection.Connected)
                    {
                        connection.Close();
                        connection.Open();
                    }
                    else
                    {

                        connection.Open();
                    }




                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Connected to printer", ToastLength.Long).Show();

                }
                catch (Exception ex)
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: " + ex.Message, ToastLength.Long).Show();

                }
                // When the background work is done, continue with this code block
            }).ContinueWith(task =>
            {
                //DoSomethingOnTheUIThread();
                // the following forces the code in the ContinueWith block to be run on the
                // calling thread, often the Main/UI thread.
            }, TaskScheduler.FromCurrentSynchronizationContext());



        }

        private void Zeroreading_Click(object sender, EventArgs e)
        {
          

            zeroreadinglist = new List<tblMeterReadingReport>();
            zeroreadinglist.Clear();
            byte[] cc = new byte[] { 0x1B, 0x21, 0x00 };  // 0- normal size text
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    zeroreadinglist = connection.Query<tblMeterReadingReport>("SELECT * FROM tblMeterReadingReport WHERE Remarks = 'Zero Consumption' Order by AccountNumber ASC");
                   
                }
                

            }
            catch (SQLiteException ex)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "SQLiteEX" + ex.Message, ToastLength.Long).Show();
                //Log.Info("SQLiteEX", ex.Message);
            }

            try
            {
                if (zeroreadinglist.Count == 0)
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "No Zero Consumption Found", ToastLength.Long).Show();
                }
                else
                {



                    string myDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");
                    var stringBuilder = new StringBuilder();

                    stringBuilder.Append("\n");
                    stringBuilder.Append("          Republic of the Philippines\n");
                    stringBuilder.Append("        Pantabangan Municipal Water System\n");
                    stringBuilder.Append("             Brgy. East Poblacion\n");
                    stringBuilder.Append("             Pantabangan, Nueva Ecija\n");
                    stringBuilder.Append("          Contact Number: " + announce[1].Announce + "\n");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();

                    stringBuilder.Append("             Zero Consumption Report\n");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();

                    stringBuilder.Append("Account No.      Reading\n");

                    for (int i = 0; i < zeroreadinglist.Count; i++)
                    {
                         
                        stringBuilder.Append(zeroreadinglist[i].AccountNumber + "      " + zeroreadinglist[i].CurrentReading + " \n");
                    }


                    stringBuilder.AppendLine();
                    stringBuilder.Append("Total: " + zeroreadinglist.Count +"\n");
                    stringBuilder.AppendLine();
                    stringBuilder.Append("Reader: " + "" + username.Text);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("Date Printed  " + "" + myDate + "\n");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();


                    connection.Write(cc);
                    // Console.WriteLine("THIS: " + stringBuilder);
                    connection.Write(Encoding.UTF8.GetBytes(stringBuilder.ToString()));

                }

                
            }
            catch (Exception)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "No Printer Connected", ToastLength.Long).Show();

            }
        }

        private void Startreading_Click(object sender, EventArgs e)
        {
           

        
            Intent intent = new Intent(this, typeof(ReadingActivity));
            intent.PutExtra("UserName", username.Text);
            StartActivity(intent);
            
        }

        private void Uploaddata_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(UploadDataActivity));
            intent.PutExtra("UserName", username.Text);
            StartActivity(intent);
        }

        private void Downloaddata_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(DownloadDataActivity));
            intent.PutExtra("UserName", username.Text);
            StartActivity(intent);
        }
    }
}