// Filename: MainWindow.xaml.cs
// Project:  DC Assignment 2 (Practical 7)
// Purpose:  Transaction Generator GUI
// Author:   George Aziz (19765453)
// Date:     17/05/2021

using Blockchain_Classes;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Block = Blockchain_Classes.Block;

namespace Transaction_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void PerformUpdateBlockchainView();
        private delegate void PerformCreateTransaction(Transaction input);
        private delegate float PerformGetBalance(uint wallet);

        private readonly RestClient _blockchainClient;
        private readonly RestClient _minerClient;
        private readonly string _blockchainURL;
        private readonly string _minerURL;
        readonly LogData _logger;

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "AzizCoin Generator";
            _logger = new LogData();
            //Web API Setup
            _logger.LogMsg("Main Program", "Initialising Web APIs");
            _blockchainURL = "https://localhost:44367/";
            _blockchainClient = new RestClient(_blockchainURL);
            _minerURL = "https://localhost:44391/";
            _minerClient = new RestClient(_minerURL);
            PerformUpdateBlockchainView updateView = new PerformUpdateBlockchainView(this.UpdateBlockchainView);
            updateView.BeginInvoke(null, null);
        }

        /// <summary>
        /// Thread that updates the Blockahin View GUI
        /// </summary>
        private void UpdateBlockchainView()
        {
            while (true)
            {
                RestRequest blockRequest = new RestRequest("api/Blockchain/GetChain");
                IRestResponse blockResp = _blockchainClient.Get(blockRequest);
                List<Block> blockchain = JsonConvert.DeserializeObject<List<Block>>(blockResp.Content);

                RestRequest countRequest = new RestRequest("api/Blockchain/GetCount");
                IRestResponse countResp = _blockchainClient.Get(countRequest);
                int count = JsonConvert.DeserializeObject<int>(countResp.Content);
                this.Dispatcher.Invoke(() =>
                {
                    ChainCount.Text = "Blockchain count: " + count;
                    BlockchainView.Items.Clear();
                    foreach (Block block in blockchain)
                    {
                        BlockchainView.Items.Add(block);    
                    }
                });
                Thread.Sleep(3000); //Updates Blockchain every 3 seconds 
            }
        }

        /// <summary>
        /// Create New Tranasction Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateNewTransaction_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogMsg("Create New Transaction", "Starting...");
            if (UInt32.TryParse(WalletIDFrom.Text, out uint from) && UInt32.TryParse(WalletIDTo.Text, out uint to) && float.TryParse(AmountInput.Text, out float amt))
            {
                if (!WalletIDFrom.Text.Equals(WalletIDTo.Text)) //Can't send to same wallet
                {
                    Transaction input = new Transaction()
                    {
                        WalletIDFrom = from,
                        WalletIDTo = to,
                        Amount = amt,
                        Processed = false
                    };
                    PerformCreateTransaction createTransaction = new PerformCreateTransaction(this.CreateTransaction);
                    createTransaction.BeginInvoke(input, null, null);
                    _logger.LogMsg("Create New Transaction", "Succesfully Created Transaction");
                }
                else
                {
                    MessageBox.Show("Can't create a transaction with sender and receiver as the same ID");
                    _logger.LogMsg("Create New Transaction", "Invalid Transaction Creation: Sender and Receiver Wallet IDs cannot be the same");
                }
            }
            else
            {
                MessageBox.Show("Please make sure all transaction details are valid (Wallet ID: Positive Integers Only | Amount: Positive Number)");
                _logger.LogMsg("Create New Transaction", "Invalid Transaction Creation: Invalid input");
            }
        }

        /// <summary>
        /// Thread that creats a transaction
        /// </summary>
        /// <param name="input">Transaction to be created</param>
        private void CreateTransaction(Transaction input)
        {
            RestRequest request = new RestRequest("api/Miner/CreateTransaction");
            request.AddJsonBody(input);
            _minerClient.Post(request);
            _logger.LogMsg("Miner", "Creating Transaction Request Sending...");
        }

        /// <summary>
        /// Get Wallet Balance Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetWalletBalance_Click(object sender, RoutedEventArgs e)
        {
            if (UInt32.TryParse(FindWallet.Text, out uint wallet)) //Validates wallet id
            {
                AsyncCallback callback;
                callback = this.GetBalanceCompletion;
                PerformGetBalance getBalance = new PerformGetBalance(this.GetBalance);
                getBalance.BeginInvoke(wallet, callback, null);
                _logger.LogMsg("Getting Balance", "Starting...");
            }
            else
            {
                MessageBox.Show("Please make sure all Wallet ID is a positive integer");
                _logger.LogMsg("Getting Balance", "Invalid Input: Wallet ID cannot be negative");
            }
        }

        /// <summary>
        /// Thread that requests wallet balance
        /// </summary>
        /// <param name="wallet">Wallet to be checked</param>
        /// <returns>Balance of wallet</returns>
        private float GetBalance(uint wallet)
        {
            RestRequest request = new RestRequest("api/Blockchain/GetBalance/" + wallet);
            return JsonConvert.DeserializeObject<float>(_blockchainClient.Get(request).Content);
        }

        /// <summary>
        /// Updates Wallet Balance in GUI
        /// </summary>
        /// <param name="asyncResult"></param>
        private void GetBalanceCompletion(IAsyncResult asyncResult)
        {
            PerformGetBalance click;
            AsyncResult asyncObj = (AsyncResult)asyncResult;

            if (asyncObj.EndInvokeCalled == false)
            {
                click = (PerformGetBalance)asyncObj.AsyncDelegate;
                float balance = click.EndInvoke(asyncObj);

                //GUI Elements Change
                this.Dispatcher.Invoke(() =>
                {
                    if (balance == -1) //-1 is impossible other than if it doesn't exist
                    {
                        BalanceLabel.Content = "Wallet Balance: [Wallet ID Does not Exist]";
                        _logger.LogMsg("Getting Balance", "Wallet ID Inputted does not exist");
                    }
                    else //Valid output
                    {
                        BalanceLabel.Content = "Wallet Balance: " + balance.ToString();
                        _logger.LogMsg("Getting Balance", "Succesfully got balance");
                    }
                });
            }
            asyncObj.AsyncWaitHandle.Close(); //Clean Up
        }
    }
}
