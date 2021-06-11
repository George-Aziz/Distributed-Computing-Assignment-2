// Filename: MainWindow.xaml.cs
// Project:  DC Assignment 2 (Practical 1)
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
using ServerProg;
using DBDLL;
using System.Drawing;
using System.Windows.Interop;

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataServerInterface connection;

        public MainWindow()
        {
            //Window Setup
            InitializeComponent();

            //Factory that generates remote connections to remote class. This is what hides the RPC stuff
            ChannelFactory<DataServerInterface> connectFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            //Set URL and create Connection
            string URL = "net.tcp://localhost:8100/DataService";
            connectFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            connection = connectFactory.CreateChannel();
            //Tells how many entries are in the DB
            Total.Text = connection.GetNumEntries().ToString();
        }


        /// <summary>
        /// Handles Go Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //On click, Get the index...
                if(int.TryParse(IndexBlock.Text, out int index))
                {
                    //Run our RPC function, using the out mode params
                    connection.GetValuesForEntry(index, out uint acct, out uint pin, out int bal, out string fName, out string lName, out Bitmap image);
                    //Set Values in GUI
                    FNameBlock.Text = fName;
                    LNameBlock.Text = lName;
                    BalanceBlock.Text = bal.ToString("C");
                    AccBlock.Text = acct.ToString();
                    PINBlock.Text = pin.ToString("D4");
                    UserImage.Source = Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                else
                {
                    MessageBox.Show("Please ensure that the index inputted is an integer");
                }
                
               
            }
            catch(FaultException<IndexOutOfRangeFault> ex)
            {
                //Resets connection to an unfaulted state
                NetTcpBinding tcp = new NetTcpBinding();
                string URL = "net.tcp://localhost:8100/DataService";
                ChannelFactory<DataServerInterface> connectFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
                connection = connectFactory.CreateChannel();

                MessageBox.Show(ex.Detail.Issue);

            }
        }
    }
}
