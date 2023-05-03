    using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

namespace eBACSMobileV2
{
    [Activity(Label = "DownloadAccountsActivity")]
    public class DownloadAccountsActivity : Activity
    {
        EditText ip, printeraddress,pass;
        Button upd,updateprinter,syshistory;
        ProgressBar progg;

        WebClient accountsclient, webannounceclient;

        List<tblserverSQLite> serverip;
        List<tblAccountsSQLite> accountlist;
        List<tblannouncement> mAnnouncement;
        private Uri mUrl;

        string folder;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DownloadAccounts);
            // Create your application here

        
            ip = FindViewById<EditText>(Resource.Id.txtip);
            printeraddress = FindViewById<EditText>(Resource.Id.txtprinter);
            pass = FindViewById<EditText>(Resource.Id.txtpass);
            upd = FindViewById<Button>(Resource.Id.btndownload);
            syshistory = FindViewById<Button>(Resource.Id.btnsyshistory);
            updateprinter = FindViewById<Button>(Resource.Id.btnupdateprinter);
            progg = FindViewById<ProgressBar>(Resource.Id.prog);

            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    connection.CreateTable<tblserverSQLite>();
                    connection.CreateTable<tblannounceSQLite>();

                }
            }
           catch(Exception e)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error Creating Accounts table: " + e.Message, ToastLength.Long).Show();
            }

            serverip = new List<tblserverSQLite>();

            upd.Click += Upd_Click;
            updateprinter.Click += Updateprinter_Click;
            syshistory.Click += Syshistory_Click;
            loadip();
        }

        private void Syshistory_Click(object sender, EventArgs e)
        {
            if (pass.Text == "")
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "Please enter password", ToastLength.Long).Show();
            }
            else
            {
                if (pass.Text == "pantabangan")
                {

                    Intent intent = new Intent(this, typeof(SystemHistory));
                    StartActivity(intent);
                    pass.Text = "";
                }
                else
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Wrong password", ToastLength.Long).Show();
                }
            }
        }

        private void Updateprinter_Click(object sender, EventArgs e)
        {
            
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    serverip = connection.Query<tblserverSQLite>("SELECT * FROM tblserverSQLite");

                    if (serverip.Count == 0)
                    {

                        tblserverSQLite newip = new tblserverSQLite()
                        {
                            ipaddress = "" + ip.Text,
                            printeraddress = "" + printeraddress.Text,
                        };

                        connection.Insert(newip);

                        //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Update Complete", ToastLength.Long).Show();
                    }
                    else
                    {
                        connection.Query<tblserverSQLite>("UPDATE tblserverSQLite set printeraddress=?",  printeraddress.Text);
                        //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Update Complete", ToastLength.Long).Show();
                    }

                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Save Success", ToastLength.Long).Show();

                }
            }
            catch
            {

            }
            
        }

        private void Upd_Click(object sender, EventArgs e)
        {

            if (pass.Text == "")
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "Please enter password", ToastLength.Long).Show();
            }
            else
            {
                if (pass.Text == "pantabangan")
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                    {
                        serverip = connection.Query<tblserverSQLite>("SELECT * FROM tblserverSQLite");

                        if (serverip.Count == 0)
                        {

                            tblserverSQLite newip = new tblserverSQLite()
                            {
                                ipaddress = "" + ip.Text,
                                printeraddress = "" + printeraddress.Text,
                            };

                            connection.Insert(newip);

                            //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Update Complete", ToastLength.Long).Show();
                        }
                        else
                        {
                            connection.Query<tblserverSQLite>("UPDATE tblserverSQLite set ipaddress =?, printeraddress=?", ip.Text, printeraddress.Text);
                            //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Update Complete", ToastLength.Long).Show();
                        }


                    }

                    progg.Visibility = ViewStates.Visible;

                    accountsclient = new WebClient();
                    mUrl = new Uri("http://" + ip.Text + "selectaccounts.php");

                    accountsclient.DownloadDataAsync(mUrl);
                    accountsclient.DownloadDataCompleted += Accountsclient_DownloadDataCompleted;

                    webannounceclient = new WebClient();
                    mUrl = new Uri("http://" + ip.Text + "selectannounce.php");

                    webannounceclient.DownloadDataAsync(mUrl);
                    webannounceclient.DownloadDataCompleted += Webannounceclient_DownloadDataCompleted;
                    pass.Text = "";
                }
                else
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Incorrect Password", ToastLength.Long).Show();
                }
            }


           
            
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
                   

                }
                catch (Exception ex)
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: " + ex.Message, ToastLength.Long).Show();
                }

              
            });
        }

        private void Accountsclient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {

                    connection.CreateTable<tblAccountsSQLite>();
                    connection.Query<tblAccountsSQLite>("Delete FROM tblAccountsSQLite");


                }

                try
                {
                    string errormessage = Encoding.UTF8.GetString(e.Result);
                    //Console.WriteLine("Error ng PHP" + errormessage);

                    string json = Encoding.UTF8.GetString(e.Result);
                    accountlist = JsonConvert.DeserializeObject<List<tblAccountsSQLite>>(json);
                    
                    


                    for (int i = 0; i < accountlist.Count; i++)
                    {

                        var username = accountlist[i].UserName;
                        var password = accountlist[i].Password;
                        var fullname = accountlist[i].FullName;

                        tblAccountsSQLite accountss = new tblAccountsSQLite()
                        {

                            UserName = username,
                            Password = "" + password,
                            FullName = "" + fullname,

                        };

                        using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                        {

                            connection.Insert(accountss);
                            //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Save Complete", ToastLength.Long).Show();
                            //Console.WriteLine("Save Complete");

                        }



                    }//end of for loop

                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Save Complete", ToastLength.Long).Show();
                    progg.Visibility = ViewStates.Invisible;

                }
                catch (Exception ex)
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: " + ex.Message, ToastLength.Long).Show();
                    progg.Visibility = ViewStates.Invisible;

                }


            });
        }

        private void loadip()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {

                    
                    serverip = connection.Query<tblserverSQLite>("SELECT * FROM tblserverSQLite");
                    ip.Text = serverip[0].ipaddress;
                    printeraddress.Text = serverip[0].printeraddress;
                }

            }
            catch(Exception)
            {

            }
        }
    }
}