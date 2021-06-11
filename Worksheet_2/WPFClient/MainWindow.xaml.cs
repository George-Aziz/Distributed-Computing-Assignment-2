// Filename: MainWindow.xaml.cs
// Project:  DC Assignment 2 (Practical 2)
// Purpose:  GUI Client for Database Service
// Author:   George Aziz (19765453)
// Date:     22/04/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ServiceModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Interop;
using System.Runtime.Remoting.Messaging;
using BizDLL;

namespace WPFClient
{
    public delegate void PerformGoClick(int index, out uint acct, out uint pin, out int bal, out string fName, out string lName, out Bitmap image);
    public delegate void PerformSearchClick(string lastname, out uint acct, out uint pin, out int bal, out string fName, out string lName, out Bitmap image);

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static BusinessServerInterface connection;

        public MainWindow()
        {
            //Window Setup
            InitializeComponent();

            //Factory that generates remote connections to remote class. This is what hides the RPC stuff
            ChannelFactory<BusinessServerInterface> connectFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            //Set URL and create Connection
            string URL = "net.tcp://localhost:8200/BusinessService";
            connectFactory = new ChannelFactory<BusinessServerInterface>(tcp, URL);
            connection = connectFactory.CreateChannel();
            //Tells how many entries are in the DB
            Total.Text = "Total items: " + connection.GetNumEntries().ToString();
        }


        /// <summary>
        /// Handles Go Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoBtn_Click(object sender, RoutedEventArgs e)
        {
            AsyncCallback callBackClick;
            callBackClick = this.GoClickCompletion;
            PerformGoClick perform = new PerformGoClick(this.DoGoClick);
            int val;
            if (Int32.TryParse(IndexBlock.Text, out val))
            { 
                perform.BeginInvoke(val, out uint acct, out uint pin, out int bal, out string fName, out string lName, out Bitmap image, callBackClick, null);
            }
            else
            {
                MessageBox.Show("Please Input Integers Only", "Invalid Input");
                return;
            }
        }

        /// <summary>
        /// Go Click Button Thread
        /// </summary>
        /// <param name="index"></param>
        /// <param name="acct"></param>
        /// <param name="pin"></param>
        /// <param name="bal"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <param name="image"></param>
        private void DoGoClick(int index, out uint acct, out uint pin, out int bal, out string fName, out string lName, out Bitmap image)
        {
            this.Dispatcher.Invoke(() =>
            {
                LockElements();
            });
            connection.GetValuesForEntry(index, out acct, out pin, out bal, out fName, out lName, out image);
        }

        /// <summary>
        /// Go Click Button On Completion to Update GUI
        /// </summary>
        /// <param name="asyncResult"></param>
        private void GoClickCompletion(IAsyncResult asyncResult)
        {
            PerformGoClick click ;
            AsyncResult asyncObj = (AsyncResult)asyncResult;

            if (asyncObj.EndInvokeCalled == false)
            {
                try
                {
                    click = (PerformGoClick)asyncObj.AsyncDelegate;
                    click.EndInvoke(out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap image, asyncObj);

                    //GUI Elements Change
                    this.Dispatcher.Invoke(() =>
                    {
                        UnlockElements();
                        FNameBlock.Text = fName;
                        LNameBlock.Text = lName;
                        BalanceBlock.Text = bal.ToString("C");
                        AccBlock.Text = acctNo.ToString();
                        PINBlock.Text = pin.ToString("D4");
                        UserImage.Source = Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    });
                }
                catch(FaultException<BusinessFault> ex)
                {
                    UnlockElements();
                    ResetElements();
                    MessageBox.Show(ex.Detail.Issue);
                }
                catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
                {
                    
                    NetTcpBinding tcp = new NetTcpBinding();
                    string URL = "net.tcp://localhost:8200/BusinessService";
                    ChannelFactory<BusinessServerInterface> connectFactory = new ChannelFactory<BusinessServerInterface>(tcp, URL);
                    connection = connectFactory.CreateChannel();
                    UnlockElements();
                    ResetElements();
                    MessageBox.Show("Business Server is Down - Please Try Again Later", "Service Down Error");
                }
            }
            asyncObj.AsyncWaitHandle.Close(); //Clean Up
        }

        /// <summary>
        /// Handles Search Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBtn_Click(object sender, RoutedEventArgs e) 
        {
            AsyncCallback callBackClick;
            callBackClick = this.SearchClickCompletion;
            PerformSearchClick perform = new PerformSearchClick(this.DoSearchClick);
            perform.BeginInvoke(FindLNameBlock.Text.ToString(), out uint acct, out uint pin, out int bal, out string fName, out string lName, out Bitmap image, callBackClick, null);
        }

        /// <summary>
        /// Search Button Click Thread
        /// </summary>
        /// <param name="name"></param>
        /// <param name="acct"></param>
        /// <param name="pin"></param>
        /// <param name="bal"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <param name="image"></param>
        private void DoSearchClick(string name, out uint acct, out uint pin, out int bal, out string fName, out string lName, out Bitmap image)
        {
            this.Dispatcher.Invoke(() =>
            {
                LockElements();
            });

            connection.SearchEntry(name, out acct, out pin, out bal, out fName, out lName, out image);
        }

        /// <summary>
        /// Search Button Click On Completion to Update GUI
        /// </summary>
        /// <param name="asyncResult"></param>
        private void SearchClickCompletion(IAsyncResult asyncResult)
        {
            PerformSearchClick click;
            AsyncResult asyncObj = (AsyncResult)asyncResult;

            if (asyncObj.EndInvokeCalled == false)
            {
                try
                {
                    click = (PerformSearchClick)asyncObj.AsyncDelegate;
                    click.EndInvoke(out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap image, asyncObj);


                    //GUI Elements Change
                    this.Dispatcher.Invoke(() =>
                    {
                        UnlockElements();
                        FNameBlock.Text = fName;
                        LNameBlock.Text = lName;
                        BalanceBlock.Text = bal.ToString("C");
                        AccBlock.Text = acctNo.ToString();
                        PINBlock.Text = pin.ToString("D4");
                        UserImage.Source = Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    });
                }
                catch (FaultException<BusinessFault> ex)
                {
                    MessageBox.Show(ex.Detail.Issue);
                    this.Dispatcher.Invoke(() =>
                    {
                        UnlockElements();
                        ResetElements(); 
                    });
                }
                catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
                {

                    NetTcpBinding tcp = new NetTcpBinding();
                    string URL = "net.tcp://localhost:8200/BusinessService";
                    ChannelFactory<BusinessServerInterface> connectFactory = new ChannelFactory<BusinessServerInterface>(tcp, URL);
                    connection = connectFactory.CreateChannel();
                    UnlockElements();
                    ResetElements();
                    MessageBox.Show("Business Server is Down - Please Try Again Later", "Service Down Error");
                }
            }
            asyncObj.AsyncWaitHandle.Close(); //Clean Up
        }

        /// <summary>
        /// Method Responsible for locking and restricting the user from interacting with GUI 
        /// </summary>
        private void LockElements()
        {
            this.Dispatcher.Invoke(() =>
            {
                FNameBlock.IsReadOnly = true;
                LNameBlock.IsReadOnly = true;
                BalanceBlock.IsReadOnly = true;
                AccBlock.IsReadOnly = true;
                PINBlock.IsReadOnly = true;
                IndexBlock.IsReadOnly = true;
                GoBtn.IsEnabled = false;
                SearchBtn.IsEnabled = false;
                UserImage.IsEnabled = false;
                FindLNameBlock.IsReadOnly = true;
                ProgBar.IsIndeterminate = true;
            });
        }

        /// <summary>
        /// Method Responsible for unlocking and allowing user to interact with GUI
        /// </summary>
        private void UnlockElements()
        {
            this.Dispatcher.Invoke(() =>
            {
                FNameBlock.IsReadOnly = false;
                LNameBlock.IsReadOnly = false;
                BalanceBlock.IsReadOnly = false;
                AccBlock.IsReadOnly = false;
                PINBlock.IsReadOnly = false;
                IndexBlock.IsReadOnly = false;
                GoBtn.IsEnabled = true;
                SearchBtn.IsEnabled = true;
                UserImage.IsEnabled = true;
                FindLNameBlock.IsReadOnly = false;
                ProgBar.IsIndeterminate = false;
            });
        }

        /// <summary>
        /// Method Responsible for resseting values of GUI Elements
        /// </summary>
        private void ResetElements()
        {
            this.Dispatcher.Invoke(() =>
            {
                FNameBlock.Text = "FirstName";
                LNameBlock.Text = "LastName";
                BalanceBlock.Text = "Balance";
                AccBlock.Text = "AcctNo";
                PINBlock.Text = "PIN";
                IndexBlock.Text = "Index?";
                FindLNameBlock.Text = "Search by Last Name";
                UserImage.Source = null;
            });
        }

    }
}
