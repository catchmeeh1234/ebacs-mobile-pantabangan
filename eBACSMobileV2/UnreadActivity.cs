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
using Zebra.Sdk.Comm;
using static Android.App.ActionBar;

namespace eBACSMobileV2
{
    [Activity(Label = "UnreadActivity")]
    public class UnreadActivity : Activity
    {
        TextView countt;
        Button printt;
        List<tblbillsSQLite> mBills;
        List<tblannounceSQLite> announce;
        string folder;

        Connection connection = DashboardActivity.connection;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           
            SetContentView(Resource.Layout.Unread);

            // Create your application here
            TableLayout table = FindViewById<TableLayout>(Resource.Id.tableLayout1);
            countt = FindViewById<TextView>(Resource.Id.txtcount);
            printt = FindViewById<Button>(Resource.Id.btnprint);


            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            mBills = new List<tblbillsSQLite>();

            

            printt.Click += Printt_Click;


            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    mBills = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite WHERE Reading = '0' order by CAST(ReadingSeqNo AS INTEGER) ASC");
                    announce = connection.Query<tblannounceSQLite>("SELECT * FROM tblannounceSQLite");

                }


            }
            catch (SQLiteException ex)
            {

                Toast t = Toast.MakeText(Android.App.Application.Context, "SQLiteEX" + ex.Message, ToastLength.Long);
                t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                t.Show();
            }



            if (mBills.Count == 0)
            {

                countt.Text = mBills.Count + " Found";
                Toast t = Toast.MakeText(Android.App.Application.Context, "No 0 Reading Found", ToastLength.Long);
                t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                t.Show();
            }
            else
            {

                //lagay kung ilan
                countt.Text = mBills.Count + " Found";
                for (int iii = 0; iii < mBills.Count; iii++)
                {
                    
                    TableRow row = new TableRow(this);

                    TextView t = new TextView(this);
                    // set the text to "text xx"
                    t.Text = "(" + mBills[iii].MeterNo + ")    " + mBills[iii].AccountNumber + "   -   " + mBills[iii].CustomerName;

                    row.AddView(t);

                    table.AddView(row, new TableLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent));

                }


            }


        }

        private void Printt_Click(object sender, EventArgs e)
        {
            try
            {
                if (mBills.Count == 0)
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "No Data Found", ToastLength.Long).Show();
                }
                else
                {

                    byte[] cc = new byte[] { 0x1B, 0x21, 0x00 };  // 0- normal size text

                    string myDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");
                    var stringBuilder = new StringBuilder();

                    stringBuilder.Append("\n");
                    stringBuilder.Append("          Republic of the Philippines\n");
                    stringBuilder.Append("        Pantabangan Municipal Water System\n");
                    stringBuilder.Append("              Brgy. East Poblacion\n");
                    stringBuilder.Append("             Pantabangan, Nueva Ecija\n");
                    stringBuilder.Append("          Contact Number: " + announce[1].Announce + "\n");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();

                    stringBuilder.Append("                  Unread\n");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();

                    stringBuilder.Append("Meter No. | Account No. |  Concessionaire \n");

                    for (int i = 0; i < mBills.Count; i++)
                    {

                        stringBuilder.Append(mBills[i].MeterNo + "       " + mBills[i].AccountNumber + "      " + mBills[i].CustomerName + " \n");
                    }


                    stringBuilder.AppendLine();
                    stringBuilder.Append("Summary:\n");
                    stringBuilder.Append("Total: " + mBills.Count + "\n");
                    stringBuilder.AppendLine();
                    stringBuilder.Append("Reader: ");
                    stringBuilder.AppendLine(Intent.GetStringExtra("UserName"));
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("Date Printed  " + "" + myDate + "\n");

                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();


                    connection.Write(cc);
                     Console.WriteLine("THIS: " + stringBuilder);
                    connection.Write(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
                  
                }


            }
            catch (Exception)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "No Printer Connected", ToastLength.Long).Show();

            }
        }
    }
}