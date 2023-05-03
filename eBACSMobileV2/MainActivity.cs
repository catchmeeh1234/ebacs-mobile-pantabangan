using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using eBACSMobileV2.Resources.tables;
using System.Collections.Generic;
using SQLite;
using System;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using Android.Views;

namespace eBACSMobileV2
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/pantabangan", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        Button login,sett;
        EditText user, pass;

        List<tblAccountsSQLite> accountlist;

        string folder;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource

          
            SetContentView(Resource.Layout.activity_main);


            login = FindViewById<Button>(Resource.Id.btnlogin);
            sett = FindViewById<Button>(Resource.Id.btnsetting);
            user = FindViewById<EditText>(Resource.Id.txtusername);
            pass = FindViewById<EditText>(Resource.Id.txtpassword);

            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

           

            login.Click += Login_Click;
            sett.Click += Sett_Click;
  
        }

        private void Sett_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(DownloadAccountsActivity));
            StartActivity(intent);
        }

        private void Login_Click(object sender, System.EventArgs e)
        {

            accountlist = null;

            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    accountlist = connection.Query<tblAccountsSQLite>("SELECT * FROM tblAccountsSQLite WHERE UserName='" + user.Text + "' AND Password = '" + pass.Text + "'");

                }

                if (accountlist.Count == 0)
                {

                    Toast t = Toast.MakeText(Android.App.Application.Context, "No Account Found", ToastLength.Long);
                    t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                    t.Show();
                    //Android.Widget.Toast.MakeText(Android.App.Application.Context, "No Account found or wrong Email and Password", ToastLength.Long).Show();
                }
                else
                {
                    pass.Text = "";
                    Intent intent = new Intent(this, typeof(DashboardActivity));
                    intent.PutExtra("UserName", accountlist[0].FullName);
                    StartActivity(intent);

                }
            }
            catch (Exception)
            {

            }

            //Intent intent = new Intent(this, typeof(DashboardActivity));
            //StartActivity(intent);


            //StringBuilder hash = new StringBuilder();
            //MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            //byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(pass.Text));

            //for (int i = 0; i < bytes.Length; i++)
            //{
            //    hash.Append(bytes[i].ToString("x2"));
            //}
            //user.Text = hash.ToString();
        }
    }
}