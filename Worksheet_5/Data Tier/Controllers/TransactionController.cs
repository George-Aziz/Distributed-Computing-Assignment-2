// Filename: TrasactionController.cs
// Project:  DC Assignment 2 (Practical 5)
// Purpose:  Data Tier Controller for Transaction Services
// Author:   George Aziz (19765453)
// Date:     06/05/2021

using API_Classes;
using Data_Tier.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Data_Tier.Controllers
{
    public class TransactionController : ApiController
    {
        private BankDB.TransactionAccessInterface transAccess = Bank.Get().GetTransAccess();
        private LogData log = new LogData();

        [Route("api/Transaction/Create/{amt}/{senderID}/{receiverID}")]
        [HttpPost]
        public TransactionDetails CreateTransaction(uint amt, uint senderID, uint receiverID)
        {
            try
            {
                
                BankDB.AccountAccessInterface acc = Bank.Get().GetAccAccess();
                acc.SelectAccount(senderID);
                log.LogMsg("Data-Tier TransactionController", "Creating Transaction between Account: " + senderID + " to Account: " + receiverID + " for $" + amt);
                //Get sender balance to make sure they have sufficient funds
                uint balance = acc.GetBalance();
                if (amt <= balance)
                {
                    TransactionDetails tran = new TransactionDetails();
                    //Create Transaction
                    tran.transactionID = transAccess.CreateTransaction();
                    tran.amount = amt;
                    tran.senderID = senderID;
                    tran.receiverID = receiverID;

                    transAccess.SelectTransaction(tran.transactionID);
                    transAccess.SetAmount(tran.amount);
                    transAccess.SetSendr(tran.senderID);
                    transAccess.SetRecvr(tran.receiverID);

                    Debug.WriteLine(tran.transactionID);
                    return tran;
                }
                else
                {
                    log.LogMsg("Data-Tier TransactionController", "ERROR Creating Transaction between Account: " + senderID + " to Account: " + receiverID + " for $" + amt);
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Balance is too low to make a transaction!")
                    });
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message)
                });
            }
        }

        [Route("api/Transaction/GetAll")]
        [HttpGet]
        public List<TransactionDetails> GetTransactions()
        {
            try
            {
                log.LogMsg("Data-Tier TransactionController", "Getting all transactions");
                //Get Transactions
                List<TransactionDetails> transactions = new List<TransactionDetails>();
                List<uint> transactionIDs = transAccess.GetTransactions();
                foreach (uint curID in transactionIDs)
                {
                    TransactionDetails curTransaction = new TransactionDetails();
                    transAccess.SelectTransaction(curID);
                    curTransaction.transactionID = curID;
                    curTransaction.senderID = transAccess.GetSendrAcct();
                    curTransaction.receiverID = transAccess.GetRecvrAcct();
                    curTransaction.amount = transAccess.GetAmount();
                    transactions.Add(curTransaction);
                }    
                
                return transactions;  
            }
            catch (Exception ex)
            {
                log.LogMsg("Data-Tier TransactionController", "ERROR Getting all transactions");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message)
                });
            }
        }
    }
}