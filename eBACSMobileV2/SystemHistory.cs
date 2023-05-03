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
using eBACSMobileV2.Resources.tables;
using SQLite;
using static Android.App.ActionBar;

namespace eBACSMobileV2
{
    [Activity(Label = "SystemHistory")]
    public class SystemHistory : Activity
    {

        string folder;
        Button del;
        EditText pass;

        List<tblsystemhistory> syshistory;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SystemHistory);
            // Create your application here
            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            del = FindViewById<Button>(Resource.Id.btndelete);
            pass = FindViewById<EditText>(Resource.Id.txtpass);
            TableLayout table = FindViewById<TableLayout>(Resource.Id.tableLayout1);

            syshistory = new List<tblsystemhistory>();


            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    syshistory = connection.Query<tblsystemhistory>("SELECT * FROM tblsystemhistory");
                  
                }


            }
            catch (SQLiteException ex)
            {

                Toast t = Toast.MakeText(Android.App.Application.Context, "SQLiteEX" + ex.Message, ToastLength.Long);
                t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                t.Show();
            }

            if (syshistory.Count == 0)
            {

              
            }
            else
            {
                for (int iii = 0; iii < syshistory.Count; iii++)
                {

                    TableRow row = new TableRow(this);

                    TextView t = new TextView(this);
                    // set the text to "text xx"
                    t.Text = "" + syshistory[iii].User + "\n" + syshistory[iii].Remarks + " \n " + syshistory[iii].Datee + "\n ____________________";

                    row.AddView(t);

                    table.AddView(row, new TableLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent));

                }

            }

            del.Click += Del_Click;

        }

        private void Del_Click(object sender, EventArgs e)
        {
            if (pass.Text == "")
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "Please enter password", ToastLength.Long).Show();
            }
            else
            {
                if (pass.Text == "srwd")
                {

                    try
                    {
                        using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                        {
                            
                            connection.Query<tblsystemhistory>("Delete FROM tblsystemhistory");
                        }

                        Toast t = Toast.MakeText(Android.App.Application.Context, "Delete Complete", ToastLength.Long);
                        t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                        t.Show();


                    }
                    catch (SQLiteException ex)
                    {

                        Toast t = Toast.MakeText(Android.App.Application.Context, "SQLiteEX" + ex.Message, ToastLength.Long);
                        t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                        t.Show();
                    }

                }

                else
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Incorrect password", ToastLength.Long).Show();
                }
            }
        }
    }
}