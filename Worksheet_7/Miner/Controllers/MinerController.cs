// Filename: MinerController.cs
// Project:  DC Assignment 2 (Practical 7)
// Purpose:  Controller for Miner Web API
// Author:   George Aziz (19765453)
// Date:     16/05/2021

using Blockchain_Classes;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Http;

namespace Miner.Controllers
{
    public class MinerController : ApiController
    {
        private delegate void PerformTransactionProcess();
        private static bool _initialRun = true;
        private static readonly Queue<Transaction> _transactions = new Queue<Transaction>();
        private static readonly Mutex _mutex = new Mutex();

        /// <summary>
        /// Creates a new transaction
        /// </summary>
        /// <param name="input">Transaction Details</param>
        [Route("api/Miner/CreateTransaction")]
        [HttpPost]
        public void CreateTransaction(Transaction input)
        {
            if (_initialRun) //If it is the first run, create the genesis block and start the thread to process transactions
            {
                PerformTransactionProcess processTransaction = new PerformTransactionProcess(this.ProcessTransaction);
                processTransaction.BeginInvoke(null, null);
                _initialRun = false;
            }
            _mutex.WaitOne();
            _transactions.Enqueue(input);
            _mutex.ReleaseMutex();
        }

        /// <summary>
        /// Method that will be run in a background thread to process all transactions in queue
        /// </summary>
        private void ProcessTransaction()
        {
            RestClient client = new RestClient("https://localhost:44367/");
            while (true) //Runs forever in the background
            {
                _mutex.WaitOne();
                    try
                    {
                        if (_transactions.Count > 0) //Will only process transactions if there are transactions in the queue
                        {
                            Transaction curTransaction = _transactions.Dequeue();

                            //Ensures Transaction details are valid
                            if (curTransaction.Amount > 0 && curTransaction.WalletIDFrom >= 0 && curTransaction.WalletIDTo >= 0)
                            {
                                //Extra validation to ensure the wallet that is sending in the transaction has enough $$$ to perform the transaction
                                RestRequest balRequest = new RestRequest("api/Blockchain/GetBalance/" + curTransaction.WalletIDFrom);
                                IRestResponse response = client.Get(balRequest);
                                float respWalletBal = JsonConvert.DeserializeObject<float>(response.Content);
                                if (respWalletBal >= curTransaction.Amount)
                                {
                                    RestRequest chainRequest = new RestRequest("api/Blockchain/GetChain");
                                    IRestResponse chainResp = client.Get(chainRequest);
                                    List<Block> blockchain = JsonConvert.DeserializeObject<List<Block>>(chainResp.Content);

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

                                    //Computes hash for new block
                                    SHA256 sha256 = SHA256.Create();
                                    while (!newBlock.Hash.StartsWith("12345"))
                                    {
                                        newBlock.Offset += 5;

                                        byte[] bytes = Encoding.UTF8.GetBytes(newBlock.ToHashString());
                                        byte[] hashArray = sha256.ComputeHash(bytes);
                                        string newHash = BitConverter.ToUInt64(hashArray, 0).ToString();
                                        newBlock.Hash = newHash;
                                    }

                                    //Adds newly created block into blockchain
                                    RestRequest addRequest = new RestRequest("api/Blockchain/AddBlock");
                                    addRequest.AddJsonBody(newBlock);
                                    client.Post(addRequest);

                                    //Sets the transaction to processed
                                    curTransaction.Processed = true;
                                }
                            }
                        }
                        Thread.Sleep(500); //Sleeps right before going for another iteration as there is no need for instant execution and constant checks
                    }
                    catch (InvalidOperationException)
                    {
                        Debug.WriteLine("Miner API Error: Transactions Queue is empty");
                    }
                _mutex.ReleaseMutex();
            }
        }
    }
}