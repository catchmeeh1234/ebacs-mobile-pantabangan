using System;
using System.Collections.Generic;
using System.Globalization;
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
using static Android.App.ActionBar;

namespace eBACSMobileV2
{
    [Activity(Label = "ReadingActivity")]
    public class ReadingActivity : Activity
    {
        string Highconsumprem, CustomerType, Metersize;
        TextView billid, billno, accno, meterno, conces, classs, address, sequence, ave, senior, previousrdgdate, currentdate, previousreading,  currentconsumption, amountdue,  penal;
        TextView othercharge, meteringfee, totalamountdue, finishcount,seniorlabel,seniordisc,advancepayment, arrears;
        EditText findd, reading, billidnos;
        List<tblbillsSQLite> mBills, countfinish, searching, datasearching, seqbills, getprinted;
        List<tblserverSQLite> serverip;
        List<tblannounceSQLite> announce;
            List<tblMeterReadingReport> highconsumptionlist;
        List<tblReaderHistory> meterreaderhistorylist;
        TableLayout tablelist;

        Button previous, next, calc, SaveUpdate, findbutton,recontoprinter,addfinding;
        decimal twenty, thirty, forty, fifty, maxx, otherchargestotal, unpaidbillstotal;
        int DataCounter, consumption, searchdatacounter, consumpcomputer, reprinted;
        decimal arrbill, arrcharges, totalarr;
        string folder,duedate, duee, fromdate,seniorvisibility, remmm;
        

        DateTime disconec;

        Connection connection = DashboardActivity.connection;
      

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Reading);
            // Create your application here

           
            billid = FindViewById<TextView>(Resource.Id.txtbillid);
            billno = FindViewById<TextView>(Resource.Id.txtbillnumber);
            accno = FindViewById<TextView>(Resource.Id.txtaccountnumber);
            meterno = FindViewById<TextView>(Resource.Id.txtmeternumber);
            conces = FindViewById<TextView>(Resource.Id.txtconcessionaire);
            classs = FindViewById<TextView>(Resource.Id.txtclass);
            address = FindViewById<TextView>(Resource.Id.txtaddress);
            sequence = FindViewById<TextView>(Resource.Id.txtsequencenumber);
            ave = FindViewById<TextView>(Resource.Id.txtaverage);
            senior = FindViewById<TextView>(Resource.Id.txtsenior);
            previousrdgdate = FindViewById<TextView>(Resource.Id.txtpreviousdate);
            currentdate = FindViewById<TextView>(Resource.Id.txtreadingdate);
            previousreading = FindViewById<TextView>(Resource.Id.txtpreviousreading);
            reading = FindViewById<EditText>(Resource.Id.txtcurrentreading);
            currentconsumption = FindViewById<TextView>(Resource.Id.txtreading);
            amountdue = FindViewById<TextView>(Resource.Id.txtamountdue);
           
            penal = FindViewById<TextView>(Resource.Id.txtpenalty);
            seniorlabel = FindViewById<TextView>(Resource.Id.lblsenior);
            seniordisc = FindViewById<TextView>(Resource.Id.txtseniordisc);

            othercharge = FindViewById<TextView>(Resource.Id.txtothercharges);
            meteringfee = FindViewById<TextView>(Resource.Id.txtmeterfee);
           
            totalamountdue = FindViewById<TextView>(Resource.Id.txttotalamount);
            finishcount = FindViewById<TextView>(Resource.Id.txtfinishcount);
            advancepayment = FindViewById<TextView>(Resource.Id.txtadvancepayment);
            arrears = FindViewById<TextView>(Resource.Id.txtarrear);


            findd = FindViewById<EditText>(Resource.Id.txtfind);
            

            calc = FindViewById<Button>(Resource.Id.btncalculate);
            previous = FindViewById<Button>(Resource.Id.btnprevious);
            next = FindViewById<Button>(Resource.Id.btnnext);
            SaveUpdate = FindViewById<Button>(Resource.Id.btnsaveandprint);
            findbutton = FindViewById<Button>(Resource.Id.btnfind);
            recontoprinter = FindViewById<Button>(Resource.Id.btnrecon);
            addfinding = FindViewById<Button>(Resource.Id.btnaddfindings);


            folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            mBills = new List<tblbillsSQLite>();

            DataCounter = 0;
            searchdatacounter = 0;

            seniorvisibility = "invisible";

            reading.RequestFocus();

            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    mBills = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite WHERE BillStatus = 'Pending' Order by CAST(ReadingDate AS DATE) ASC, Zone ASC, CAST(ReadingSeqNo AS INTEGER) ASC");
                    countfinish = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite WHERE NOT Reading = 0 Order by AccountNumber ASC");
                    serverip = connection.Query<tblserverSQLite>("SELECT * FROM tblserverSQLite");
                    announce = connection.Query<tblannounceSQLite>("SELECT * FROM tblannounceSQLite");
                    connection.CreateTable<tblMeterReadingReport>();
                   
                }

                finishcount.Text = countfinish.Count + " of " + mBills.Count;
            }
            catch (SQLiteException ex)
            {

                Toast t = Toast.MakeText(Android.App.Application.Context, "SQLiteEX" + ex.Message, ToastLength.Long);
                t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                t.Show();
                //Android.Widget.Toast.MakeText(Android.App.Application.Context, "SQLiteEX" + ex.Message, ToastLength.Long).Show();
                //Log.Info("SQLiteEX", ex.Message);
            }
            //double xx = 6330.20;
            ///Console.WriteLine("sample" + xx.ToString("N1", CultureInfo.InvariantCulture));
            ///


            //DashboardActivity.connection
            //connecttoprinter();
             tablelist = FindViewById<TableLayout>(Resource.Id.tableLayout1);

            addfinding.Click += Addfinding_Click;
            recontoprinter.Click += Recontoprinter_Click;
            previous.Click += Previous_Click;
            calc.Click += Calc_Click;
            next.Click += Next_Click;
            SaveUpdate.Click += SaveUpdate_Click;
            findbutton.Click += Findbutton_Click;
            reading.TextChanged += Reading_TextChanged;
            LoadData();

        }

        private void Reading_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            calculatee();
        }

        private void Addfinding_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(FindingActivity));
            intent.PutExtra("UserName", Intent.GetStringExtra("UserName"));
            intent.PutExtra("Accno", accno.Text);
            intent.PutExtra("Accname", conces.Text);
            StartActivity(intent);
        }

        private void Recontoprinter_Click(object sender, EventArgs e)
        {
           
            connecttoprinter();
        }

        private void Findbutton_Click(object sender, EventArgs e)
        {
            searchData();
        }

        private void SaveUpdate_Click(object sender, EventArgs e)
        {
            if (reading.Text == "0" || reading.Text == "")
            {

                Toast t = Toast.MakeText(Android.App.Application.Context, "Invalid Reading", ToastLength.Long);
                t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                t.Show();
                //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Invalid Reading", ToastLength.Long).Show();
            }
            else if  (int.Parse(previousreading.Text) > int.Parse(reading.Text))
            {

                Toast t = Toast.MakeText(Android.App.Application.Context, "Invalid Reading", ToastLength.Long);
                t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                t.Show();
                //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Invalid Reading", ToastLength.Long).Show();
            }

            else
            {
                
                Highconsumprem = "";

                consumpcomputer = int.Parse(ave.Text) * 2;

                if (int.Parse(currentconsumption.Text) >= consumpcomputer)
                {
                    //Toast t = Toast.MakeText(Android.App.Application.Context, "High Consumption", ToastLength.Long);
                    //t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                    //t.Show();

                    if (int.Parse(currentconsumption.Text) <= 10 )

                    {

                    }
                    else
                    {
                        AlertDialog.Builder alert = new AlertDialog.Builder(this);
                        alert.SetTitle("Alert");
                        alert.SetMessage("High Consumption");
                        alert.SetPositiveButton("OK", (senderAlert, args) =>
                        {
                            alert.Dispose();
                        });


                        Dialog dialog = alert.Create();
                        dialog.Show();


                        Highconsumprem = "(HIGH CONSUMPTION)";
                        savemeterreadingreport();
                    }

                   

                }
                else
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                    {
                        try
                        {
                            highconsumptionlist = connection.Query<tblMeterReadingReport>("SELECT * FROM tblMeterReadingReport WHERE AccountNumber = '" + accno.Text + "'");

                            if (highconsumptionlist.Count == 0)
                            {
                               
                            }
                            else
                            {
                                connection.Query<tblMeterReadingReport>("Delete from tblMeterReadingReport WHERE AccountNumber =?", accno.Text);
                               
                            }


                        }
                        catch (SQLiteException ex)
                        {

                         
                        }
                    }
                }
                
                string myDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
                tblReaderHistory readerhistory = new tblReaderHistory()
                {

                    AccountNumber = "" + accno.Text,
                    AccountName = "" + conces.Text,
                    TimeRead = "" + myDate,
                    Reading = "" + reading.Text,
                    Cons = "" + currentconsumption.Text,
                    BillNumber = "" + billno.Text,
                    Reader = "" + Intent.GetStringExtra("UserName"),

                };

                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    getprinted = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite WHERE BillNo='" + billno.Text + "'");

                }

                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    try
                    {
                        connection.CreateTable<tblReaderHistory>();                        

                        string devDate = DateTime.Now.ToString("yyyy-MM-dd"); //current date
                        
                        DateTime DevDue;
                        var devCdate = DateTime.Parse(devDate); 
                        DevDue = devCdate.AddDays(11); //due date
                        string stringddue;
                        stringddue = DevDue.ToString("yyyy-MM-dd");

                        currentdate.Text = "" + devDate;
                        duedate = "" + DevDue;
                        duee = "" + DevDue;


                        connection.Query<tblbillsSQLite>("Update tblbillsSQLite set PenaltyAfterDue=?,Reading=?,Consumption=?,AmountDue=?,Discount=?,reprinted=?,ReadingDate=?, DueDate=? WHERE BILLID =?", penal.Text, reading.Text, currentconsumption.Text, amountdue.Text,seniordisc.Text, getprinted[0].reprinted + 1, devDate, stringddue, billid.Text);
                        
                        countfinish = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite WHERE NOT Reading = 0 Order by AccountNumber ASC");
                        

                       
                        connection.Insert(readerhistory);
                       
                       

                        Toast t = Toast.MakeText(Android.App.Application.Context, "Reading Save Complete", ToastLength.Long);
                        t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                        t.Show();
                        //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Reading Save Complete", ToastLength.Long).Show();
                        finishcount.Text = countfinish.Count + " of " + mBills.Count;
                    }
                    catch (SQLiteException ex)
                    {

                        Toast t = Toast.MakeText(Android.App.Application.Context, "Error: " + ex.Message, ToastLength.Long);
                        t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                        t.Show();
                       // Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: " + ex.Message, ToastLength.Long).Show();
                    }
                }

              

                findd.Text = "";

                if (currentconsumption.Text == "0")
                {
                    savezeroreadingreport();
                }
                else
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                    {
                        try
                        {
                            highconsumptionlist = connection.Query<tblMeterReadingReport>("SELECT * FROM tblMeterReadingReport WHERE AccountNumber = '" + accno.Text + "'AND Consumption = '0'");

                            if (highconsumptionlist.Count == 0)
                            {

                            }
                            else
                            {
                                connection.Query<tblMeterReadingReport>("Delete from tblMeterReadingReport WHERE AccountNumber =? AND Consumption = '0'", accno.Text);

                            }


                        }
                        catch (SQLiteException ex)
                        {


                        }
                    }
                }

                //SearchSavedData();
                printbill();
            }
        }

        private void calculatee()
        {
            consumpcomputer = 0;
        
            if (reading.Text == "0")
            {
               
            }
            else if(reading.Text == "")
            {
               

            }
            else if (currentconsumption.Text == "")
            {

            }
            else
            {
                try
                {

                    double penaltytwodecimal;

                    twenty = 0;
                    thirty = 0;
                    forty = 0;
                    fifty = 0;
                    maxx = 0;


                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                    {

                        currentconsumption.Text = (Double.Parse(reading.Text) - Double.Parse(previousreading.Text)).ToString();
                        var ratee = connection.Query<tblrateSQLite>("select * from tblrateSQLite where CustomerType=? AND MeterSize = ?", CustomerType, Metersize);

                        //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Minimum: " + ratee[0].MinimumCharge, ToastLength.Long).Show();

                        consumption = int.Parse(currentconsumption.Text);

                        if (consumption <= 10)
                        {

                            string arr = arrbill.ToString();
                            amountdue.Text = ratee[0].MinimumCharge.ToString();
                            //penal.Text = (((double.Parse(amountdue.Text) + double.Parse(arr) + double.Parse(othercharge.Text)) - double.Parse(seniordisc.Text)) * 0.15).ToString();
                        }
                        else
                        {
                            //automatic 180 or minimum
                            consumption = consumption - 10;

                            if (consumption > 10)
                            {
                                consumption = consumption - 10;
                                twenty = ratee[0].twenty * 10;

                                if (consumption > 10)
                                {
                                    consumption = consumption - 10;
                                    thirty = ratee[0].thirty * 10;

                                    if (consumption > 10)
                                    {
                                        consumption = consumption - 10;
                                        forty = ratee[0].forty * 10;

                                        if (consumption > 10)
                                        {
                                            consumption = consumption - 10;
                                            fifty = ratee[0].fifty * 10;

                                            maxx = ratee[0].maxx * consumption;
                                        }
                                        else
                                        {
                                            fifty = ratee[0].fifty * consumption;
                                            maxx = 0;
                                        }
                                    }
                                    else
                                    {
                                        forty = ratee[0].forty * consumption;
                                        fifty = 0;
                                        maxx = 0;
                                    }
                                }
                                else
                                {
                                    thirty = ratee[0].thirty * consumption;
                                    forty = 0;
                                    fifty = 0;
                                    maxx = 0;
                                }
                            }
                            else
                            {
                                twenty = ratee[0].twenty * consumption;
                                thirty = 0;
                                forty = 0;
                                fifty = 0;
                                maxx = 0;
                            }


                            string arr = arrbill.ToString();
                            amountdue.Text = (ratee[0].MinimumCharge + twenty + thirty + forty + fifty + maxx).ToString();


                     
                            penaltytwodecimal = (((double.Parse(amountdue.Text) + double.Parse(arr) + double.Parse(othercharge.Text)) - double.Parse(seniordisc.Text)) * 0.15);

                            penaltytwodecimal = System.Math.Round(penaltytwodecimal, 2);
                            //penal.Text = penaltytwodecimal.ToString();
                        
                        }
                    }// end ng using


                }
                catch (Exception)
                {

                    Toast t = Toast.MakeText(Android.App.Application.Context, "Invalid Class, Metersize or Average Consumption, Please check Database", ToastLength.Long);
                    t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                    t.Show();
                    //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Invalid Class, Metersize or Average Consumption, Please check Database", ToastLength.Long).Show();
                    currentconsumption.Text = "0";


                }

                computee();
            }


        }

        private void Calc_Click(object sender, EventArgs e)
        {

            
        }

        private void Next_Click(object sender, EventArgs e)
        {

            try
            {
                seqbills = null;
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    seqbills = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite WHERE BillStatus = 'Pending' AND CAST(ReadingSeqNo AS INT) > '" + int.Parse(sequence.Text) + "' ORDER by CAST(ReadingSeqNo AS INT) ASC");
                }

                if (seqbills.Count == 0)
                {
                    Toast t = Toast.MakeText(Android.App.Application.Context, "Last Data Found", ToastLength.Long);
                    t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                    t.Show();
                }
                else
                {

                    fillupdata();

                }


            }
            catch (SQLiteException ex)
            {

                Toast t = Toast.MakeText(Android.App.Application.Context, "SQLiteEX" + ex.Message, ToastLength.Long);
                t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                t.Show();
            }
            //if (findd.Text == "")
            //{
            //    if (DataCounter == (mBills.Count - 1))
            //    {

            //        Toast t = Toast.MakeText(Android.App.Application.Context, "Last Data Reach", ToastLength.Long);
            //        t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
            //        t.Show();
            //        //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Last Data Reach", ToastLength.Long).Show();
            //    }
            //    else
            //    {
            //        DataCounter = DataCounter + 1;
            //        LoadData();
            //    }
            //}
            //else
            //{
            //    if (searchdatacounter == (searching.Count - 1))
            //    {
            //        Toast t = Toast.MakeText(Android.App.Application.Context, "Last Data Reach", ToastLength.Long);
            //        t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
            //        t.Show();
            //        //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Last Data Reach", ToastLength.Long).Show();
            //    }
            //    else
            //    {
            //        searchdatacounter = searchdatacounter + 1;
            //        searchData();
            //    }
            //}
        }

        private void fillupdata()

        {

            try
            {

                CustomerType = "" + seqbills[0].RateSchedule;
                Metersize = "" + seqbills[0].MeterSize;


                if (seqbills[0].Reading == "0")
                {
                    //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Dito pumasok", ToastLength.Long).Show();
                    reading.Text = "";
                }
                else
                {
                    reading.Text = seqbills[0].Reading;
                }

              

                arrbill = seqbills[0].ArrearsBill;
                arrcharges = seqbills[0].ArrearsCharges;
                totalarr = arrbill + arrcharges;

                billid.Text = "" + seqbills[0].BILLID;
                billno.Text = "" + seqbills[0].BillNo;
                accno.Text = "" + seqbills[0].AccountNumber;
                meterno.Text = "" + seqbills[0].MeterNo;
                conces.Text = "" + seqbills[0].CustomerName;
                classs.Text = "" + seqbills[0].RateSchedule;
                address.Text = "" + seqbills[0].CustomerAddress;
                sequence.Text = "" + seqbills[0].ReadingSeqNo;
                ave.Text = "" + seqbills[0].Averagee;
                senior.Text = "" + seqbills[0].isSenior;
                previousrdgdate.Text = "" + seqbills[0].LasReadingDate;
                currentdate.Text = "" + seqbills[0].ReadingDate;
                previousreading.Text = "" + seqbills[0].PreviousReading;

                currentconsumption.Text = "" + seqbills[0].Consumption;
                amountdue.Text = "" + seqbills[0].AmountDue;
                penal.Text = "" + seqbills[0].PenaltyAfterDue;
                advancepayment.Text = "" + seqbills[0].AdvancePayment;
                arrears.Text = "" + totalarr;


                duedate = "" + seqbills[0].DueDate;
                duee = "" + seqbills[0].DueDate;
                fromdate = "" + seqbills[0].LasReadingDate;

               

                if (seqbills[0].isSenior == "Yes")
                {

                    seniorlabel.Visibility = ViewStates.Visible;
                    seniordisc.Visibility = ViewStates.Visible;
                    seniordisc.Text = "0.00";
                    seniorvisibility = "visible";
                }
                else
                {
                    seniorlabel.Visibility = ViewStates.Gone;
                    seniordisc.Visibility = ViewStates.Gone;
                    seniordisc.Text = "0.00";
                    seniorvisibility = "invisible";
                }

                if (amountdue.Text == "")
                {

                }
                else
                {
                    computee();
                }

            }

            catch (SQLiteException ex)
            {

                Toast t = Toast.MakeText(Android.App.Application.Context, "No Data Found", ToastLength.Long);
                t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                t.Show();
            }


        
        }

        private void Previous_Click(object sender, EventArgs e)
        {

            try
            {
                seqbills = null;
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
                {
                    seqbills = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite WHERE BillStatus = 'Pending' AND CAST(ReadingSeqNo AS INT) < '" + int.Parse(sequence.Text) + "' ORDER by CAST(ReadingSeqNo AS INT) DESC");
                }

                if (seqbills.Count == 0)
                {
                    Toast t = Toast.MakeText(Android.App.Application.Context, "First Data Found", ToastLength.Long);
                    t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                    t.Show();
                }
                else
                {

                    fillupdata();

                }


            }
            catch (SQLiteException ex)
            {

                Toast t = Toast.MakeText(Android.App.Application.Context, "SQLiteEX" + ex.Message, ToastLength.Long);
                t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                t.Show();
            }
            

           
        }

        private void LoadData()
        {
           
           
            try
            {

                CustomerType = "" + mBills[0].RateSchedule;
                Metersize = "" + mBills[0].MeterSize;


                if (mBills[0].Reading == "0")
                {
                    //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Dito pumasok", ToastLength.Long).Show();
                    reading.Text = "";
                }
                else
                {
                    reading.Text = mBills[0].Reading;
                }

                
                arrbill = mBills[0].ArrearsBill;
                arrcharges = mBills[0].ArrearsCharges;
                totalarr = arrbill + arrcharges;

                billid.Text = "" + mBills[0].BILLID;
                billno.Text = "" + mBills[0].BillNo;
                accno.Text = "" + mBills[0].AccountNumber;
                meterno.Text = "" + mBills[0].MeterNo;
                conces.Text = "" + mBills[0].CustomerName;
                classs.Text = "" + mBills[0].RateSchedule;
                address.Text = "" + mBills[0].CustomerAddress;
                sequence.Text = "" + mBills[0].ReadingSeqNo;
                ave.Text = "" + mBills[0].Averagee;
                senior.Text = "" + mBills[0].isSenior;
                previousrdgdate.Text = "" + mBills[0].LasReadingDate;
                currentdate.Text = "" + mBills[0].ReadingDate;
                previousreading.Text = "" + mBills[0].PreviousReading;

                currentconsumption.Text = "" + mBills[0].Consumption;
                amountdue.Text = "" + mBills[0].AmountDue;
                penal.Text = "" + mBills[0].PenaltyAfterDue;
                advancepayment.Text = "" + mBills[0].AdvancePayment;
                arrears.Text = "" + totalarr;


                duedate = "" + mBills[0].DueDate;
                duee = "" + mBills[0].DueDate;
                fromdate = "" + mBills[0].LasReadingDate;


                


               
                if (mBills[0].isSenior == "Yes")
                {

                    seniorlabel.Visibility = ViewStates.Visible;
                    seniordisc.Visibility = ViewStates.Visible;
                    seniordisc.Text = "0.00";
                    seniorvisibility = "visible";
                }
                else
                {
                    seniorlabel.Visibility = ViewStates.Gone;
                    seniordisc.Visibility = ViewStates.Gone;
                    seniordisc.Text = "0.00";
                    seniorvisibility = "invisible";
                }

                if (amountdue.Text == "")
                {

                }
                else
                {
                    computee();
                }

            }

            catch (Exception ex)
            {

                Toast t = Toast.MakeText(Android.App.Application.Context, "Error: No Data Found" + ex.Message, ToastLength.Long);
                t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                t.Show();
                //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: No Data Found" + ex.Message, ToastLength.Long).Show();
             
            }
            //Android.Widget.Toast.MakeText(Android.App.Application.Context, Intent.GetStringExtra("UserName"), ToastLength.Long).Show();

        }

        private void computee()
        {
            using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
            {


                var chargeslist = connection.Query<tblbillchargesSQLite>("select * from tblbillchargesSQLite where BillNumber=? AND Particulars = 'Metering Fee'", billno.Text);
                //Android.Widget.Toast.MakeText(Android.App.Application.Context, chargeslist.Count(), ToastLength.Long).Show();
                if (chargeslist.Count == 0)
                {

                    meteringfee.Text = "0";
                }
                else
                {
                    decimal chargelisttt;
                    chargelisttt = 0;
                    for (int iii = 0; iii < chargeslist.Count; iii++)
                    {
                        chargelisttt = chargelisttt + chargeslist[iii].Amount;

                    }

                    meteringfee.Text = chargelisttt.ToString();
                   
                }

                var otherchargeslist = connection.Query<tblbillchargesSQLite>("select * from tblbillchargesSQLite where BillNumber=? AND NOT Particulars = 'Metering Fee'", billno.Text);
                //Android.Widget.Toast.MakeText(Android.App.Application.Context, chargeslist.Count(), ToastLength.Long).Show();
                if (otherchargeslist.Count == 0)
                {
                    othercharge.Text = "0";
                }
                else
                {

                    otherchargestotal = 0;
                    for (int ii = 0; ii < otherchargeslist.Count; ii++)
                    {
                        otherchargestotal = otherchargestotal + otherchargeslist[ii].Amount;

                    }
                    othercharge.Text = otherchargestotal.ToString();
                }
                
            }//end ng using
            //Android.Widget.Toast.MakeText(Android.App.Application.Context, amountdue.Text, ToastLength.Long).Show();
            if (seniorvisibility == "visible")
            {
               //Android.Widget.Toast.MakeText(Android.App.Application.Context,"Pasok", ToastLength.Long).Show();
                if (int.Parse(currentconsumption.Text) <= 30)
                {
                    double advancedecimall;

                    advancedecimall = double.Parse(amountdue.Text) * 0.05;

                    seniordisc.Text = advancedecimall.ToString("N2", CultureInfo.InvariantCulture);
                }
                else
                {
                    seniordisc.Text = "0.00";
                   
                }
               
            }

            try

            {
                totalamountdue.Text = (double.Parse(amountdue.Text) + double.Parse(arrears.Text) + double.Parse(othercharge.Text) + double.Parse(meteringfee.Text) - double.Parse(seniordisc.Text) - double.Parse(advancepayment.Text)).ToString("N2", CultureInfo.InvariantCulture);
                penal.Text = System.Math.Round((double.Parse(totalamountdue.Text) * 0.15), 2).ToString();
            }
            catch
            {

            }
           
        }

        private void SearchSavedData()
        {

            billidnos = FindViewById<EditText>(Resource.Id.txtbillid);

            using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
            {
                //searching = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite WHERE CustomerName LIKE '%" + findd.Text + "%' OR AccountNumber LIKE '%" + findd.Text + "%' OR MeterNo LIKE '%" + findd.Text + "%'  OR ReadingSeqNo LIKE '%" + findd.Text + "%' OR BILLID LIKE '%" + findd.Text + "%' Order by CAST(ReadingDate AS DATE) ASC, Zone ASC, CAST(ReadingSeqNo AS INTEGER) ASC");

                //datasearching = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite WHERE BILLID = '" + billidnos.Text + "'");
                datasearching = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite WHERE BillNo='" + billno.Text + "'");

            }


            if (datasearching.Count == 0)
            {

                Toast t = Toast.MakeText(Android.App.Application.Context, "No Data Found", ToastLength.Long);
                t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                t.Show();
                //Android.Widget.Toast.MakeText(Android.App.Application.Context, "No Data Found", ToastLength.Long).Show();
            }
            else
            {

                //list sa baba



                tablelist.RemoveAllViews();

                for (int iii = 0; iii < datasearching.Count; iii++)
                {

                    TableRow row = new TableRow(this);
                    row.Clickable = true;

                    TextView t = new EditText(this);
                    // set the text to "text xx"
                    t.Text = "(" + datasearching[iii].BILLID + ")    " + datasearching[iii].MeterNo + "   -   " + datasearching[iii].CustomerName;

                    row.AddView(t);

                    tablelist.AddView(row, new TableLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent));


                }

                try
                {
                    CustomerType = "" + datasearching[0].RateSchedule;
                    Metersize = "" + datasearching[0].MeterSize;



                    if (datasearching[0].Reading == "0")
                    {
                        //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Dito pumasok", ToastLength.Long).Show();
                        reading.Text = "";
                    }
                    else
                    {
                        reading.Text = datasearching[0].Reading;
                    }


                    arrbill = datasearching[0].ArrearsBill;
                    arrcharges = datasearching[0].ArrearsCharges;
                    totalarr = arrbill + arrcharges;


                    billid.Text = "" + datasearching[0].BILLID;
                    billno.Text = "" + datasearching[0].BillNo;
                    accno.Text = "" + datasearching[0].AccountNumber;
                    meterno.Text = "" + datasearching[0].MeterNo;
                    conces.Text = "" + datasearching[0].CustomerName;
                    classs.Text = "" + datasearching[0].RateSchedule;
                    address.Text = "" + datasearching[0].CustomerAddress;
                    sequence.Text = "" + datasearching[0].ReadingSeqNo;
                    ave.Text = "" + datasearching[0].Averagee;
                    senior.Text = "" + datasearching[0].isSenior;
                    previousrdgdate.Text = "" + datasearching[0].LasReadingDate;
                    currentdate.Text = "" + datasearching[0].ReadingDate;
                    previousreading.Text = "" + datasearching[0].PreviousReading;

                    currentconsumption.Text = "" + datasearching[0].Consumption;
                    amountdue.Text = "" + datasearching[0].AmountDue;
                    penal.Text = "" + datasearching[0].PenaltyAfterDue;
                    advancepayment.Text = "" + datasearching[0].AdvancePayment;
                    arrears.Text = "" + totalarr;

                    duedate = "" + datasearching[0].DueDate;
                    duee = "" + datasearching[0].DueDate;
                    fromdate = "" + datasearching[0].LasReadingDate;





                    if (datasearching[0].isSenior == "Yes")
                    {
                        seniorlabel.Visibility = ViewStates.Visible;
                        seniordisc.Visibility = ViewStates.Visible;
                        seniordisc.Text = "0.00";
                        seniorvisibility = "visible";
                    }
                    else
                    {
                        seniorlabel.Visibility = ViewStates.Gone;
                        seniordisc.Visibility = ViewStates.Gone;
                        seniordisc.Text = "0.00";
                        seniorvisibility = "invisible";
                    }


                    computee();
                }

                catch (Exception ex)
                {
                    Toast t = Toast.MakeText(Android.App.Application.Context, "Error: No Data Found" + ex.Message, ToastLength.Long);
                    t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                    t.Show();
                    //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: No Data Found" + ex.Message, ToastLength.Long).Show();

                }
            }


        }

        private void searchData()
        {

            using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
            {
                searching = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite WHERE CustomerName LIKE '%" + findd.Text + "%' OR AccountNumber LIKE '%" + findd.Text + "%' OR MeterNo LIKE '%" + findd.Text + "%'  OR ReadingSeqNo LIKE '%" + findd.Text + "%' OR BILLID LIKE '%" + findd.Text + "%' Order by CAST(ReadingDate AS DATE) ASC, Zone ASC, CAST(ReadingSeqNo AS INTEGER) ASC");

            }


            if (searching.Count == 0)
            {

                Toast t = Toast.MakeText(Android.App.Application.Context, "No Data Found", ToastLength.Long);
                t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                t.Show();
                //Android.Widget.Toast.MakeText(Android.App.Application.Context, "No Data Found", ToastLength.Long).Show();
            }
            else
            {

                //list sa baba



                tablelist.RemoveAllViews();

                for (int iii = 0; iii < searching.Count; iii++)
                {

                    TableRow row = new TableRow(this);
                    row.Clickable = true;

                    TextView t = new EditText(this);
                    // set the text to "text xx"
                    t.Text = "(" + searching[iii].BILLID + ")    " + searching[iii].MeterNo + "   -   " + searching[iii].CustomerName;

                    row.AddView(t);

                    tablelist.AddView(row, new TableLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent));
                  
                    
                }
                
                try
                {
                    CustomerType = "" + searching[0].RateSchedule;
                    Metersize = "" + searching[0].MeterSize;



                    if (searching[0].Reading == "0")
                    {
                        //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Dito pumasok", ToastLength.Long).Show();
                        reading.Text = "";
                    }
                    else
                    {
                        reading.Text = searching[0].Reading;
                    }

                    
                    arrbill = searching[0].ArrearsBill;
                    arrcharges = searching[0].ArrearsCharges;
                    totalarr = arrbill + arrcharges;


                    billid.Text = "" + searching[0].BILLID;
                    billno.Text = "" + searching[0].BillNo;
                    accno.Text = "" + searching[0].AccountNumber;
                    meterno.Text = "" + searching[0].MeterNo;
                    conces.Text = "" + searching[0].CustomerName;
                    classs.Text = "" + searching[0].RateSchedule;
                    address.Text = "" + searching[0].CustomerAddress;
                    sequence.Text = "" + searching[0].ReadingSeqNo;
                    ave.Text = "" + searching[0].Averagee;
                    senior.Text = "" + searching[0].isSenior;
                    previousrdgdate.Text = "" + searching[0].LasReadingDate;
                    currentdate.Text = "" + searching[0].ReadingDate;
                    previousreading.Text = "" + searching[0].PreviousReading;

                    currentconsumption.Text = "" + searching[0].Consumption;
                    amountdue.Text = "" + searching[0].AmountDue;
                    penal.Text = "" + searching[0].PenaltyAfterDue;
                    advancepayment.Text = "" + searching[0].AdvancePayment;
                    arrears.Text = "" + totalarr;

                    duedate = "" + searching[0].DueDate;
                    duee = "" + searching[0].DueDate;
                    fromdate = "" + searching[0].LasReadingDate;


                   


                    if (searching[0].isSenior == "Yes")
                    {
                        seniorlabel.Visibility = ViewStates.Visible;
                        seniordisc.Visibility = ViewStates.Visible;
                        seniordisc.Text = "0.00";
                        seniorvisibility = "visible";
                    }
                    else
                    {
                        seniorlabel.Visibility = ViewStates.Gone;
                        seniordisc.Visibility = ViewStates.Gone;
                        seniordisc.Text = "0.00";
                        seniorvisibility = "invisible";
                    }


                    computee();
                }

                catch (Exception ex)
                {
                    Toast t = Toast.MakeText(Android.App.Application.Context, "Error: No Data Found" + ex.Message, ToastLength.Long);
                    t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                    t.Show();
                    //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: No Data Found" + ex.Message, ToastLength.Long).Show();
                   
                }
            }


        }
        
        private void printbill()
        {
            var stringBuilder = new StringBuilder();

            byte[] cc = new byte[] { 0x1B, 0x21, 0x00 };  // 0- normal size text
            byte[] bb = new byte[] { 0x1B, 0x21, 0x08 };  // 1- only bold text
            byte[] bb2 = new byte[] { 0x1B, 0x21, 0x20 }; // 2- bold with medium text
            byte[] bb3 = new byte[] { 0x1B, 0x21, 0x10 }; // 3- bold with large text

            byte[] ALIGN_CENTER = { 0x1b, 0x61, 0x01 };
            byte[] BARCODE_CODE128 = { 0x1d, 0x6b, 0x49 }; // Barcode type CODE128
            byte[] ALIGN_LEFT = { 0x1b, 0x61, 0x00 };
            byte[] ALIGN_RIGHT = { 0x1B, 0x61, 2 };

            byte[] table00 = { 0x1C };
            byte[] table0 = { 0x2E };//cancel character chinnese
            byte[] table1 = { 0x1b };
            byte[] table2 = { 0x74 };
            byte[] table3 = { 0x10 };

            string myDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");


            //DateTime duee;

            var datee = DateTime.Parse(duee);
            var fromdatee = DateTime.Parse(fromdate);
            var curdate = DateTime.Parse(currentdate.Text);
            var duedatee = DateTime.Parse(duedate);
            disconec = DateTime.Parse(duedate).AddDays(11);

            string stringdisconec, stringcurdatee,stringduedate, stringfromtdate;
            stringdisconec = disconec.ToString("MM/dd/yyyy");
            stringcurdatee = curdate.ToString("MM/dd/yyyy");
            stringduedate = duedatee.ToString("MM/dd/yyyy");
            stringfromtdate = fromdatee.ToString("MM/dd/yyyy");

            stringfromtdate = stringfromtdate.Replace("12:00:00 AM", "");
           
            stringcurdatee = stringcurdatee.Replace("12:00:00 AM", "");

            var printedvalue = 0;

            using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
            {
                getprinted = connection.Query<tblbillsSQLite>("SELECT * FROM tblbillsSQLite WHERE BillNo='" + billno.Text + "'");

            }
            
            if (getprinted.Count == 0)
            {

            }
            else
            {
            
                    printedvalue = getprinted[0].reprinted;
            }

            double amountafterdue;
            amountafterdue = double.Parse(totalamountdue.Text) + double.Parse(penal.Text);


            try
            {

                decimal currentbillingdecimal, meteringfeedecimal, seniordecimal, amountduedecimal, penaltydecimal, advancedecimal, arrearsdecimal;
                decimal otherchrgesdecimal;
                currentbillingdecimal = decimal.Parse(amountdue.Text);
                meteringfeedecimal = decimal.Parse(meteringfee.Text);
                seniordecimal = decimal.Parse(seniordisc.Text);
                amountduedecimal = decimal.Parse(totalamountdue.Text);
                penaltydecimal = decimal.Parse(penal.Text);
                advancedecimal = decimal.Parse(advancepayment.Text);
                arrearsdecimal = decimal.Parse(arrears.Text);
                otherchrgesdecimal = decimal.Parse(othercharge.Text);
                //stringBuilder.Append(mcaddress.Text);

                if (arrearsdecimal > 0)
                {
                    stringdisconec = "Immediately";
                    stringduedate = "Immediately";
                }
                else
                {

                    stringdisconec = stringdisconec.Replace("12:00:00 AM", "");
                    stringduedate = stringduedate.Replace("12:00:00 AM", "");

                }


                stringBuilder.Append("\n");
                stringBuilder.Append("          Republic of the Philippines\n");
                stringBuilder.Append("       Pantabangan Municipal Water System\n");
                stringBuilder.Append("              Brgy. East Poblacion\n");
                stringBuilder.Append("            Pantabangan, Nueva Ecija\n");



                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append("                   WATER BILL");

                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append("ACCOUNT INFORMATION        BILL NO.: " + billno.Text);

                stringBuilder.Append("\n");

                stringBuilder.Append("Account No.  : " + accno.Text);
                stringBuilder.Append("\n");

                stringBuilder.Append("Name         : " + conces.Text);
                //stringBuilder.Append("Name         : " + "Sample Enye Letters   ññññÑÑÑ");
                stringBuilder.Append("\n");

                stringBuilder.Append("Address      : " + address.Text);
                stringBuilder.Append("\n");

                stringBuilder.Append("Class        : " + classs.Text);
                stringBuilder.Append("\n");

                stringBuilder.Append("Meter No.    : " + meterno.Text);
                stringBuilder.Append("\n");

                stringBuilder.Append("Avg. Cons    : " + ave.Text);
                stringBuilder.Append("\n");

                stringBuilder.Append("Sequence No. : " + sequence.Text);
                stringBuilder.Append("\n");

                stringBuilder.AppendLine();



                stringBuilder.Append("                BILLING DETAILS \n");
                stringBuilder.AppendLine();


                stringBuilder.Append("Present Reading     : " + reading.Text);
                stringBuilder.Append("\n");
                stringBuilder.Append("Previous Reading    : " + previousreading.Text);
                stringBuilder.Append("\n");
                stringBuilder.Append("Consumption         : " + currentconsumption.Text);
                stringBuilder.Append("\n");
                stringBuilder.AppendLine();

                stringBuilder.Append("                 Period Covered\n");
                stringBuilder.Append("       From            |          To \n");
                stringBuilder.Append("    " + stringfromtdate + "                " + stringcurdatee + "\n");


                stringBuilder.AppendLine();


                stringBuilder.Append("     Discon Date       |" + "    Surcharge Date\n");

                stringBuilder.Append("    " + stringduedate + "                " + stringdisconec + "\n");
                stringBuilder.Append("----------------------------------------------\n");
                stringBuilder.AppendLine();


                stringBuilder.Append("BILLING SUMMARY                   AMOUNT\n");

                stringBuilder.Append("Current Billing          : " + String.Format("{0, 20}", "" + currentbillingdecimal.ToString("N2", CultureInfo.InvariantCulture) + "\n"));

                if (meteringfeedecimal > 0)
                {
                    stringBuilder.Append("Metering Fee             : " + String.Format("{0, 20}", "" + meteringfeedecimal.ToString("N2", CultureInfo.InvariantCulture) + "\n"));
                }

                if (otherchrgesdecimal > 0)
                {
                    stringBuilder.Append("Other Charges            : " + String.Format("{0, 20}", "" + otherchrgesdecimal.ToString("N2", CultureInfo.InvariantCulture) + "\n"));
                }

                if (advancedecimal > 0)
                {
                    stringBuilder.Append("Advance Payment          : " + String.Format("{0, 20}", "" + advancedecimal.ToString("N2", CultureInfo.InvariantCulture) + "\n"));
                }

                if (seniordecimal > 0)
                {
                    stringBuilder.Append("Senior Citizen Disc      : " + String.Format("{0, 20}", "" + seniordecimal.ToString("N2", CultureInfo.InvariantCulture) + "\n"));
                }

                if (arrearsdecimal > 0)
                {
                    stringBuilder.Append("Total Arrears            : " + String.Format("{0, 20}", "" + arrearsdecimal.ToString("N2", CultureInfo.InvariantCulture) + "\n"));
                }


                stringBuilder.Append("\n");
                stringBuilder.Append("Total Amount Due         : " + String.Format("{0, 20}", "" + amountduedecimal.ToString("N2", CultureInfo.InvariantCulture) + "\n"));

                stringBuilder.Append("Surcharge                : " + String.Format("{0, 20}", "" + penaltydecimal.ToString("N2", CultureInfo.InvariantCulture) + "\n"));

                stringBuilder.Append("----------------------------------------------\n");

                stringBuilder.Append("Amount After Due         : " + String.Format("{0, 20}", "" + amountafterdue.ToString("N2", CultureInfo.InvariantCulture) + "\n"));
                stringBuilder.Append("----------------------------------------------\n");

                stringBuilder.AppendLine();

                stringBuilder.Append("Advisory\n");

                stringBuilder.Append(announce[0].Announce);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append("Reader: " + "" + Intent.GetStringExtra("UserName"));
                stringBuilder.AppendLine();

                if (printedvalue >= 2)
                {
                    stringBuilder.AppendLine("---Reprinted---");
                    stringBuilder.AppendLine("Date Printed  " + "" + myDate + "\n");
                }
                else {
                stringBuilder.AppendLine("Date Printed  " + "" + myDate + "\n");
                }
                //stringBuilder.Append("Powered by IoTee Solutions Inc. \n");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();

                connection.Write(cc);
                Console.WriteLine("THIS: " + stringBuilder);
                connection.Write(ALIGN_LEFT);


                byte[] iso88591data = Encoding.GetEncoding("ISO-8859-1").GetBytes(stringBuilder.ToString());

                connection.Write(table00);
                connection.Write(table0);
                connection.Write(table1);
                connection.Write(table2);
                connection.Write(table3);
                connection.Write(iso88591data);

                //if (connection.Connected)
                //{
                //    connection.Close();
                  
                //}

                //connection.Write(ALIGN_CENTER);
                //connection.Write(Encoding.ASCII.GetBytes("\n"));


                //connection.Write(BARCODE_CODE128);
                //connection.Write(Encoding.ASCII.GetBytes("\n"));

                //connection.Write(Encoding.UTF8.GetBytes(accno.Text + "\n"));
                //connection.Write(Encoding.ASCII.GetBytes("\n"));
                //connection.Write(Encoding.ASCII.GetBytes("\n"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("THIS: " + stringBuilder);

                Console.WriteLine("String due date: " + stringduedate);
                Console.WriteLine("String due date: " + stringdisconec);
                Console.WriteLine("String due date: " + currentdate.Text);
                
                Toast t = Toast.MakeText(Android.App.Application.Context, "No Printer Connected: ", ToastLength.Long);
                t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                t.Show();
                //Android.Widget.Toast.MakeText(Android.App.Application.Context, "No Printer Connected", ToastLength.Long).Show();

            }
            Console.WriteLine("THIS: " + stringBuilder);

        }

        private void connecttoprinter()
        {

          

            //Task.Factory.StartNew(() =>
            //{
            //    // Do some work on a background thread, allowing the UI to remain responsive
            //    try
            //    {
            //        //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Please Wait while connecting to printer", ToastLength.Long).Show();

            //        //Console.WriteLine("simpleConnectionString" + simpleConnectionString);
            //        //connection = null;


            //        //var simpleConnectionString = $"BT:{serverip[0].printeraddress}";
            //        //connection = ConnectionBuilder.Build(simpleConnectionString);

            //        if (DashboardActivity.connection.Connected)
            //        {

            //            DashboardActivity.connection.Close();
            //            DashboardActivity.connection.Open();
            //        }
            //        else
            //        {

            //            DashboardActivity.connection.Open();
            //        }


            //        Android.Widget.Toast.MakeText(Android.App.Application.Context, "Connected to printer", ToastLength.Long).Show();

            //    }
            //    catch (Exception ex)
            //    {
            //        //connection.Close();
            //        Console.WriteLine("Error "+ ex.Message);
            //        Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: " + ex.Message, ToastLength.Long).Show();

            //    }
            //    // When the background work is done, continue with this code block
            //}).ContinueWith(task =>
            //{
            //    //DoSomethingOnTheUIThread();
            //    // the following forces the code in the ContinueWith block to be run on the
            //    // calling thread, often the Main/UI thread.
            //}, TaskScheduler.FromCurrentSynchronizationContext());
            
        }

        private void savezeroreadingreport()
        {

            string myDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");
            tblMeterReadingReport zerosave = new tblMeterReadingReport()
            {

                BillNo = "" + billno.Text,
                AccountNumber = "" + accno.Text,
                AccountName = "" + conces.Text,
                TimeRead = "" + myDate,
                PrevReading = "" + previousreading.Text,
                CurrentReading = "" + reading.Text,
                Consumption = "" + currentconsumption.Text,
                Remarks = "" + "Zero Consumption",
                Reader = "" + Intent.GetStringExtra("UserName"),


            };

            using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
            {
                try
                {

                    highconsumptionlist = connection.Query<tblMeterReadingReport>("SELECT * FROM tblMeterReadingReport WHERE AccountNumber = '" + accno.Text + "'");

                    if (highconsumptionlist.Count == 0)
                    {
                        connection.Insert(zerosave);
                    }
                    else
                    {
                        connection.Query<tblMeterReadingReport>("Delete from tblMeterReadingReport WHERE AccountNumber =?", accno.Text);
                        connection.Insert(zerosave);
                    }
                }
                catch (SQLiteException ex)
                {

                    Toast t = Toast.MakeText(Android.App.Application.Context, "Error: " + ex.Message, ToastLength.Long);
                    t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                    t.Show();
                    //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: " + ex.Message, ToastLength.Long).Show();
                }
            }

        }

        private void savemeterreadingreport()
        {

      
            string myDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");
            tblMeterReadingReport mtrsave = new tblMeterReadingReport()
            {

                BillNo = "" + billno.Text,
                AccountNumber = "" + accno.Text,
                AccountName = "" + conces.Text,
                TimeRead = "" + myDate,
                PrevReading = "" + previousreading.Text,
                CurrentReading = "" + reading.Text,
                Consumption = "" + currentconsumption.Text,
                Remarks = ""+ "High Consumption",
                Reader = "" + Intent.GetStringExtra("UserName"),


            };

            using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "eBacsMobile.db")))
            {
                try
                {
                    highconsumptionlist = connection.Query<tblMeterReadingReport>("SELECT * FROM tblMeterReadingReport WHERE AccountNumber = '"+ accno.Text +"'");

                    if (highconsumptionlist.Count ==0)
                    {
                        connection.Insert(mtrsave);
                    }
                    else
                    {
                        connection.Query<tblMeterReadingReport>("Delete from tblMeterReadingReport WHERE AccountNumber =?", accno.Text);
                        connection.Insert(mtrsave);
                    }
                   

                    Toast t = Toast.MakeText(Android.App.Application.Context, "Report Saved", ToastLength.Long);
                    t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                    t.Show();
                    //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Report Save Complete", ToastLength.Long).Show();

                }
                catch (SQLiteException ex)
                {

                    Toast t = Toast.MakeText(Android.App.Application.Context, "Error: " + ex.Message, ToastLength.Long);
                    t.SetGravity(GravityFlags.Top | GravityFlags.Top, 0, 0);
                    t.Show();
                    //Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: " + ex.Message, ToastLength.Long).Show();
                }
            }

        }
        private const int PrinterBluetoothMinorDeviceClassCode = 1664;
    }
}