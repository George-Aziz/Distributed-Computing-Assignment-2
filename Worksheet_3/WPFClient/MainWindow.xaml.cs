// Filename: MainWindow.xaml.cs
// Project:  DC Assignment 2 (Practical 3)
// Purpose:  GUI Client for Business/Data Service
// Author:   George Aziz (19765453)
// Date:     25/04/2021

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
using RestSharp;
using Newtonsoft.Json;
using API_Classes;
using System.Net;
using System.Web.Http;
using System.Net.Http;

namespace WPFClient
{
    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate DataIntermed PerformGoClick(int index);
        public delegate DataIntermed PerformSearchClick(SearchData input);
        private readonly LogData _logger;
        private RestClient _client;

        public MainWindow()
        {
            //Window Setup
            InitializeComponent();
            _client = new RestClient("https://localhost:44321/");
            RestRequest _request = new RestRequest("api/all");
            IRestResponse _response = _client.Get(_request);
            _logger = new LogData();
            _logger.LogMsg("MainWindow","Initialising");
            
            //Tells how many entries are in the DB
            Total.Text = "Total items: " + _response.Content;
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
                perform.BeginInvoke(val, callBackClick, null);
                _logger.LogMsg("Go Button Click", "Starting...");
            }
            else
            {
                _logger.LogMsg("Go Button Click", "Invalid Input");
                MessageBox.Show("Please Input Integers Only", "Invalid Input");
                return;
            }
        }

        /// <summary>
        /// GoBtn Click Thread
        /// </summary>
        /// <param name="index">Inputted User ID</param>
        /// <returns>Found User</returns>
        private DataIntermed DoGoClick(int index)
        {
            this.Dispatcher.Invoke(() =>
            {
                LockElements();
            });
            
            _client = new RestClient("https://localhost:44321/");
            RestRequest _request = new RestRequest("api/values/" + index.ToString());
            IRestResponse _response = _client.Get(_request);
            
            if (_response.StatusCode == HttpStatusCode.BadRequest)
            {
                _logger.LogMsg("Go Button Click", "Bad Request");
                throw new ActionNotSupportedException(_response.Content);
            }
            else
            {
                _logger.LogMsg("Go Button Click", "Succesfull Request");
                return JsonConvert.DeserializeObject<DataIntermed>(_response.Content);
            }
        }

        /// <summary>
        /// GoBtn Click On Completion to update the GUI
        /// </summary>
        /// <param name="asyncResult"></param>
        private void GoClickCompletion(IAsyncResult asyncResult)
        {
            PerformGoClick click ;
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            DataIntermed result;

            if (asyncObj.EndInvokeCalled == false)
            {
                try
                {
                    click = (PerformGoClick)asyncObj.AsyncDelegate;
                    result = click.EndInvoke(asyncObj);
                    _logger.LogMsg("Go Button Click", "Updating GUI");
                    if (result != null)
                    {
                        //GUI Elements Change
                        this.Dispatcher.Invoke(() =>
                        {
                            UnlockElements();
                            FNameBlock.Text = result.FirstName;
                            LNameBlock.Text = result.LastName;
                            BalanceBlock.Text = result.Balance.ToString("C");
                            AccBlock.Text = result.AcctNo.ToString();
                            PINBlock.Text = result.Pin.ToString("D4");
                            ImageConverter converter = new ImageConverter();
                            Bitmap img = (Bitmap)converter.ConvertFrom(result.Image);
                            UserImage.Source = Imaging.CreateBitmapSourceFromHBitmap(img.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        });
                    }
                }
                catch (ActionNotSupportedException ex)
                {
                    MessageBox.Show(ex.Message);
                    UnlockElements();
                    ResetElements();
                }
            }
            asyncObj.AsyncWaitHandle.Close(); //Clean Up
        }

        /// <summary>
        /// Handles the Search Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBtn_Click(object sender, RoutedEventArgs e) 
        {
            AsyncCallback callBackClick;
            callBackClick = this.SearchClickCompletion;
            PerformSearchClick perform = new PerformSearchClick(this.DoSearchClick);
            SearchData input = new SearchData();
            input.searchStr = FindLNameBlock.Text.ToString();
            perform.BeginInvoke(input, callBackClick, null);
            _logger.LogMsg("Search Button Click", "Starting...");
        }

        /// <summary>
        /// SearchBtn Thread
        /// </summary>
        /// <param name="input">SearchData object that contains last name of user</param>
        /// <returns>Found User</returns>
        private DataIntermed DoSearchClick(SearchData input)
        {
            this.Dispatcher.Invoke(() =>
            {
                LockElements();
            });

            _client = new RestClient("https://localhost:44321/");
            RestRequest _request = new RestRequest("api/search/");
            _request.AddJsonBody(input);
            IRestResponse _response = _client.Post(_request);
            if (_response.StatusCode == HttpStatusCode.BadRequest)
            {
                _logger.LogMsg("Search Button Click", "Bad Request");
                throw new ActionNotSupportedException(_response.Content);
            }
            else
            {
                _logger.LogMsg("Search Button Click", "Succesfull Request");
                return JsonConvert.DeserializeObject<DataIntermed>(_response.Content);
            }
        }

        /// <summary>
        /// SearchBtn Click On Completion to update the GUI
        /// </summary>
        /// <param name="asyncResult"></param>
        private void SearchClickCompletion(IAsyncResult asyncResult)
        {
            PerformSearchClick click;
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            DataIntermed result;

            if (asyncObj.EndInvokeCalled == false)
            {
                try
                {
                    click = (PerformSearchClick)asyncObj.AsyncDelegate;
                    result = click.EndInvoke(asyncObj);
                    _logger.LogMsg("Search Button Click", "Updating GUI");
                    //GUI Elements Change
                    this.Dispatcher.Invoke(() =>
                    {
                        UnlockElements();
                        FNameBlock.Text = result.FirstName;
                        LNameBlock.Text = result.LastName;
                        BalanceBlock.Text = result.Balance.ToString("C");
                        AccBlock.Text = result.AcctNo.ToString();
                        PINBlock.Text = result.Pin.ToString("D4");
                        ImageConverter converter = new ImageConverter();
                        Bitmap img = (Bitmap)converter.ConvertFrom(result.Image);
                        UserImage.Source = Imaging.CreateBitmapSourceFromHBitmap(img.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        //UserImage.Source = Imaging.CreateBitmapSourceFromHBitmap(result.Image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    });
                }
                catch (ActionNotSupportedException ex)
                {
                    MessageBox.Show(ex.Message);
                    UnlockElements();
                    ResetElements();
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
