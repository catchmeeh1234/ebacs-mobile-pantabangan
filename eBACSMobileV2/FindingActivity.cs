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

namespace eBACSMobileV2
{
    [Activity(Label = "FindingActivity")]
    public class FindingActivity : Activity
    {

        Spinner findings;
        TextView accno, accname,username;
        EditText rema;
        Button submit;
        List<tblFindingList> spinnerdata;
        ArrayAdapter adapter;
        JavaList<string> spinnerlist = new JavaList<string>();

        string folder;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Findings);

            // Create your application here

            findings = FindViewById<Spinner>(Resource.Id.spinner1);
            accno = FindViewById<TextView>(Resource.Id.txtaccno);
            accname = FindViewById<TextView>(Resource.Id.txtconces);
            username = FindViewById<TextView>(Resource.Id.txtname);
            rema = FindViewById<EditText>(Resource.Id.txtremarks);
            submit = FindViewById<Button>(Resource.Id.btnsubmit);


            accno.Text = Intent.GetStringExtra("Accno");
            accname.Text = Intent.GetStringExtra("Accname");
            username.Text = Intent.GetStringExtra("UserName");

            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

           


            loadspinnerdata();
           

            submit.Click += Submit_Click;
            //Android.Widget.Toast.MakeText(Android.App.Application.Context, findings.SelectedItem.ToString(), ToastLength.Long).Show();
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {

                    string newrem;

                    if (rema.Text == "")
                    {
                        newrem = findings.SelectedItem.ToString();
                    }
                    else
                    {
                        newrem = rema.Text;
                    }

                    string myDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");

                    tblfindings newfinding = new tblfindings()
                    {
                        AccountName = "" + accname.Text,
                        AccountNumber = "" + accno.Text,
                        TimeRead = "" + myDate,
                        Finding = "" + newrem,
                        Reader = "" + username.Text,
                    };

                    connection.Insert(newfinding);

                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Save Complete", ToastLength.Long).Show();

                    rema.Text = "";
                    findings.Adapter = null;
                    findings.Adapter = adapter;


                }
            }
           
            catch(Exception ex)
            { 
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: "+ ex.Message, ToastLength.Long).Show();
            }



        }

        private void loadspinnerdata()
        {
            spinnerlist.Clear();

            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    connection.CreateTable<tblfindings>();
                    spinnerdata = connection.Query<tblFindingList>("SELECT Finding FROM tblFindingList");
                    //Android.Widget.Toast.MakeText(Android.App.Application.Context, spinnerdata[0].AccountNumber, ToastLength.Long).Show();
                }


                for (int i = 0; i < spinnerdata.Count; i++)
                {

                    spinnerlist.Add(spinnerdata[i].Finding);
                }

                adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, spinnerlist);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                findings.Adapter = adapter;
            }
            catch (Exception e)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error Creating Accounts table: " + e.Message, ToastLength.Long).Show();
            }


           
        }

    }
}