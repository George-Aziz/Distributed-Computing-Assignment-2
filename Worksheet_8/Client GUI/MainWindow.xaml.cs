// Filename: Maindow.xaml.cs
// Project:  DC Assignment 2 (Practical 8)
// Purpose:  Client Blockchain Wallet GUI
// Author:   George Aziz (19765453)
// Date:     22/05/2021

using Blockchain_Classes;
using Client_API_Classes;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows;


namespace Client_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void PerformUpdateViews();
        private readonly Thread blockchainThread;
        private readonly Thread minerThread;
        private readonly LogData logger;

        //Current client fields
        private bool _windowClosed;
        private bool serverCreated;
        private string _clientRemoteUrl;
        private ChannelFactory<IBlockChainServer> _clientBlockchainFactory;
        private IBlockChainServer _clientServerConnection;
        private uint _currentPort;
        private uint _clientWalletID;

        //API fields
        private readonly string _url = "https://localhost:44364/";
        private readonly RestClient _client;
        public static Mutex transactionsMutex = new Mutex();

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "AzizCoin Wallet";
            //Client setup
            _client = new RestClient(_url);
            _currentPort = ClientCounter.CurrentPort;
            _clientWalletID = ClientCounter.ClientID;
            logger = new LogData();

            logger.LogMsg("Main Program", "Starting Blockchain Thread");
            //Server Thread setup and execution
            blockchainThread = new Thread(new ThreadStart(DoBlockchainThread));
            blockchainThread.Start();

            logger.LogMsg("Main Program", "Starting Miner Thread");
            //Network Thread setup and execution
            minerThread = new Thread(new ThreadStart(DoMinerThread));
            minerThread.Start();

            logger.LogMsg("Main Program", "Starting UpdateView Thread");
            PerformUpdateViews updateView = new PerformUpdateViews(this.UpdateViews);
            updateView.BeginInvoke(null, null);
        }

        /// <summary>
        /// Blockchain Thread to register client into blockchain so other clients can connect to it
        /// </summary>
        public void DoBlockchainThread()
        {
            serverCreated = false;
            ServiceHost host;
            do
            {
                try
                {
                    _windowClosed = false;
                    NetTcpBinding tcp = new NetTcpBinding();
                    host = new ServiceHost(typeof(BlockchainServerImplementation));
                    host.AddServiceEndpoint(typeof(IBlockChainServer), tcp, "net.tcp://127.0.0.1:" + _currentPort.ToString() + "/BlockchainServer");
                    host.Open();

                    //Adds client into client list to be seen by other clients
                    RestRequest request = new RestRequest("api/ClientAPI/AddClient");
                    ClientInputData input = new ClientInputData
                    {
                        WalletID = _clientWalletID,
                        Address = "127.0.0.1",
                        Port = _currentPort.ToString()
                    };
                    request.AddJsonBody(input);
                    _client.Post(request);

                    //Sets up remote server connection for the client
                    _clientRemoteUrl = "net.tcp://127.0.0.1:" + _currentPort + "/BlockchainServer";
                    _clientBlockchainFactory = new ChannelFactory<IBlockChainServer>(tcp, _clientRemoteUrl);
                    _clientServerConnection = _clientBlockchainFactory.CreateChannel();

                    serverCreated = true; //Server is now created which means no need for another iteration
                    logger.LogMsg("Blockchain Thread", "Wallet: " + _clientWalletID + " added");
                    while (!_windowClosed) //Keeps server thread open until window is closed
                    { }
                    host.Close();
                }
                catch (AddressAlreadyInUseException) //Current port is in use
                {
                    logger.LogMsg("Blockchain Thread", "Incrementing Port & Wallet Number");
                    ClientCounter.CurrentPort++;
                    _currentPort++; //Increases Port Number incase it is already being used and will try again
                    ClientCounter.ClientID++; //Since the address is in use that means current client is a new client
                    _clientWalletID++;
                }
            } while (!serverCreated);
        }

        /// <summary>
        /// Miner thread to process transactions and also update blockchain
        /// </summary>
        public void DoMinerThread()
        {
            //Runs till program ends completely
            while (!_windowClosed)
            {
                if (serverCreated) //Only processes and updates client's blockchain if server has been created
                {
                    try
                    {
                        //Transaction Processing
                        ProcessTransaction();
                        //Popular Hash Check & Blockchain Update
                        RestRequest request = new RestRequest("api/ClientAPI/RetrieveClients");
                        IRestResponse response = _client.Get(request);
                        List<Client> clientList = JsonConvert.DeserializeObject<List<Client>>(response.Content);
                        UpdateChain(PopularHashCheck(clientList), clientList);
                    }
                    catch (CommunicationException)
                    {
                        // if communication fails, then it will try again with another client to connect
                        logger.LogMsg("Miner Thread","Re-establishing communication");
                    }
                }
            }
        }

        /// <summary>
        /// Process Transactions that are currently in queue
        /// </summary>
        private void ProcessTransaction()
        {
            Queue<Transaction> transactions = Transactions.GetTransactions();
            transactionsMutex.WaitOne();
            try
            {
                if (transactions.Count > 0) //Will only process transactions if there are transactions in the queue
                {
                    Transaction curTransaction = transactions.Dequeue();
                    logger.LogMsg("Miner Thread", "Client: " + _clientWalletID + " processing transaction");
                    //Ensures Transaction details are valid
                    if (curTransaction.Amount > 0 && curTransaction.WalletIDFrom >= 0 && curTransaction.WalletIDTo >= 0)
                    {
                        //Extra validation to ensure the wallet that is sending in the transaction has enough $$$ to perform the transaction
                        float WalletBal = _clientServerConnection.GetBalance(curTransaction.WalletIDFrom);
                        if (WalletBal >= curTransaction.Amount)
                        {
                            List<Block> blockchain = Blockchain.GetChain();
                            //Creates new block with all details
                            Block newBlock = new Block()
                            {
                                BlockID = blockchain.Last().BlockID + 1,
                                WalletIDFrom = curTransaction.WalletIDFrom,
                                WalletIDTo = curTransaction.WalletIDTo,
                                Amount = curTransaction.Amount,
                                Offset = 0,
                                PrevHash = blockchain.Last().Hash,
                                Hash = ""
                            };
                            newBlock = GenerateHash(newBlock); //Updates block with new hash

                            //Adds newly created block into blockchain  
                            logger.LogMsg("Miner Thread", "Client: " + _clientWalletID + " Adding transaction into block");
                            _clientServerConnection.AddBlock(newBlock);
                            curTransaction.Processed = true; //Sets the transaction to processed
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                logger.LogMsg("Miner Thread", "Client: " + _clientWalletID + " Transactions Queue is empty");
            }
            transactionsMutex.ReleaseMutex();
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Creates a dictionary with each client's last block and records how many times each hash appears
        /// </summary>
        /// <param name="clientList"></param>
        /// <returns></returns>
        private Dictionary<string, int> PopularHashCheck(List<Client> clientList)
        {
            NetTcpBinding tcp = new NetTcpBinding();
            ChannelFactory<IBlockChainServer> blockchainFactory;
            IBlockChainServer serverConnection;
            string _url;

            logger.LogMsg("Miner Thread", "Client: " + _clientWalletID + " Checking latest blockchain");
            //Checks other client's blockchains
            Dictionary<string, int> lastBlockDict = new Dictionary<string, int>(); //Last Block Dictionary to see how many times the last block appears
            foreach (Client curClient in clientList)
            {
                _url = "net.tcp://" + curClient.Address + ":" + curClient.Port + "/BlockchainServer";
                blockchainFactory = new ChannelFactory<IBlockChainServer>(tcp, _url);
                serverConnection = blockchainFactory.CreateChannel();

                string lastHash = serverConnection.GetChain().Last().Hash; //Gets last block's hash
                if (lastBlockDict.ContainsKey(lastHash))
                {
                    lastBlockDict[lastHash] += 1; //Increments count of appearances last block is in
                }
                else
                {
                    lastBlockDict.Add(lastHash, 1); //Adds into dictionary if doesn't exist already
                }
            }
            return lastBlockDict;
        }

        /// <summary>
        /// Updates the current client's blockchain by finding the blockchain with the most popular hash
        /// </summary>
        /// <param name="lastBlockDict"></param>
        /// <param name="clientList"></param>
        private void UpdateChain(Dictionary<string, int> lastBlockDict, List<Client> clientList)
        {
            NetTcpBinding tcp = new NetTcpBinding();
            ChannelFactory<IBlockChainServer> blockchainFactory;
            IBlockChainServer serverConnection;
            string _url;
            //Check other client's blockchain against current client's
            int appearancesCount = 0;
            string popularHash = "";

            foreach (KeyValuePair<string, int> curEntry in lastBlockDict)
            {
                if (curEntry.Value > appearancesCount)
                {
                    appearancesCount = curEntry.Value;
                    popularHash = curEntry.Key;
                }
            }

            //Update current client's blockchain to most up to date one
            if (!_clientServerConnection.GetChain().Last().Hash.Equals(popularHash))
            {
                logger.LogMsg("Miner Thread", "Client: " + _clientWalletID + " Updating to latest blockchain");
                foreach (Client curClient in clientList)
                {
                    _url = "net.tcp://" + curClient.Address + ":" + curClient.Port + "/BlockchainServer";
                    blockchainFactory = new ChannelFactory<IBlockChainServer>(tcp, _url);
                    serverConnection = blockchainFactory.CreateChannel();

                    if (serverConnection.GetChain().Last().Hash.Equals(popularHash))
                    {
                        //Blockchain.UpdateChain(serverConnection.GetChain());
                        _clientServerConnection.UpdateChain(serverConnection.GetChain());
                    }
                }
            }
        }

        /// <summary>
        /// Generates a new hash by brute force. Ensures that the hash starts with 12345, and if not, increases offset by 1
        /// </summary>
        /// <param name="block">The block that will have a new hash</param>
        /// <returns>Block with new hash</returns>
        private Block GenerateHash(Block block)
        {
            SHA256 sha256 = SHA256.Create();
            Block returnBlock = block;

            while (!returnBlock.Hash.StartsWith("12345"))
            {
                returnBlock.Offset += 5;

                byte[] bytes = Encoding.UTF8.GetBytes(returnBlock.ToHashString());
                byte[] hashArray = sha256.ComputeHash(bytes);
                string newHash = BitConverter.ToUInt64(hashArray, 0).ToString();
                returnBlock.Hash = newHash;
            }

            return returnBlock;
        }

        /// <summary>
        /// Before window closes, ensures client is not on client list anymore (Removes "Dead" clients)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            blockchainThread.Abort();
            minerThread.Abort();
            _windowClosed = true; //Makes bool value true so server thread can end
            RestRequest request = new RestRequest("api/ClientAPI/RemoveClient/" + _currentPort.ToString());
            _client.Post(request); //Removes current exiting client as they will no longer be active
            logger.LogMsg("Main Program", "Client: " + _clientWalletID + " leaving network");

        }

        /// <summary>
        /// Create New Tranasction Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateNewTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (UInt32.TryParse(WalletIDTo.Text, out uint to) && float.TryParse(AmountInput.Text, out float amt))
            {
                if (WalletIDTo.Text.Equals(_clientWalletID.ToString())) //Can't send to same wallet
                {
                    MessageBox.Show("Can't create a transaction with sender and receiver as the same ID");
                    logger.LogMsg("Main Program", "Client: " + _clientWalletID + " cannot send money to themselves");
                }
                else if (_clientServerConnection.GetBalance(Convert.ToUInt32(_clientWalletID)) < amt) //If client does not have enough funds for transaction
                {
                    MessageBox.Show("Can't create a transaction as you do not have enough funds");
                    logger.LogMsg("Main Program", "Client: " + _clientWalletID + " does not have enough funds for transaction");
                }
                else //Everything is valid
                {
                    //Gets All Clients
                    RestRequest request = new RestRequest("api/ClientAPI/RetrieveClients");
                    IRestResponse response = _client.Get(request);
                    List<Client> clientList = JsonConvert.DeserializeObject<List<Client>>(response.Content);

                    //Remoting server variables setup
                    NetTcpBinding tcp = new NetTcpBinding();
                    string _url;
                    ChannelFactory<IBlockChainServer> blockchainFactory;
                    IBlockChainServer serverConnection;
                    //For each client, the transaction will be added into the blockchain
                    foreach (Client curClient in clientList)
                    {
                        _url = "net.tcp://" + curClient.Address + ":" + curClient.Port + "/BlockchainServer";
                        blockchainFactory = new ChannelFactory<IBlockChainServer>(tcp, _url);
                        serverConnection = blockchainFactory.CreateChannel();

                        Transaction input = new Transaction()
                        {
                            WalletIDFrom = Convert.ToUInt32(_clientWalletID),
                            WalletIDTo = to,
                            Amount = amt,
                            Processed = false
                        };
                        //Adds transaction into transaction queue
                        serverConnection.ReceiveNewTransaction(input);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please make sure all transaction details are valid (Wallet ID: Positive Integers Only | Amount: Positive Number)");
                logger.LogMsg("Main Program", "Client: " + _clientWalletID + " Invalid Input: Wallet IDs can only be positive");
            }
        }

        /// <summary>
        /// Updates the GUI for each client
        /// </summary>
        public void UpdateViews()
        {
            while (!_windowClosed) //Keeps running till window is closed
            {
                //List<Block> blockchain = Blockchain.GetChain();
                if (serverCreated) //Only updates GUI if server has been created for client
                {
                    List<Block> blockchain = _clientServerConnection.GetChain();
                    this.Dispatcher.Invoke(() =>
                    {
                        WalletIDLabel.Content = "Your Wallet ID: " + _clientWalletID;
                        ChainCount.Text = "Blockchain count: " + _clientServerConnection.GetCount();
                        //Blockchain Listview
                        BlockchainView.Items.Clear();
                        foreach (Block block in blockchain)
                        {
                            BlockchainView.Items.Add(block);
                        }

                        //Wallet Listview 
                        WalletBalancesView.Items.Clear();
                        RestRequest request = new RestRequest("api/ClientAPI/RetrieveWallets");
                        IRestResponse response = _client.Get(request);
                        List<uint> walletList = JsonConvert.DeserializeObject<List<uint>>(response.Content);
                        foreach (uint curWalletID in walletList)
                        {
                            float curBal = _clientServerConnection.GetBalance(curWalletID);
                            WalletView curWallet = new WalletView()
                            {
                                WalletID = curWalletID,
                                Balance = curBal
                            };
                            WalletBalancesView.Items.Add(curWallet);
                        }
                    });
                    Thread.Sleep(3000); //GUI does not need to update instantly on each iteration
                }
            }
        }
    }
}
