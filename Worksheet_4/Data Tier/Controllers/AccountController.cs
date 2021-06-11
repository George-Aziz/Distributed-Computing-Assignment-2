// Filename: AccountController.cs
// Project:  DC Assignment 2 (Practical 4)
// Purpose:  Data Tier Controller for Account Access Services
// Author:   George Aziz (19765453)
// Date:     06/05/2021


using API_Classes;
using Data_Tier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Data_Tier.Controllers
{
    public class AccountController : ApiController
    {
        private BankDB.AccountAccessInterface accAccess = Bank.Get().GetAccAccess();
        private LogData log = new LogData();

        /// <summary>
        /// Get account details through specified Account ID
        /// </summary>
        /// <param name="accID">Account ID</param>
        /// <returns>Account Details of Account with specified ID</returns>
        [Route("api/Account/{accID}")]
        [HttpGet]
        public AccountDetails GetAccountDetails(uint accID)
        {
            try
            {
                log.LogMsg("Data-Tier AccountController", "Getting Account: " + accID);
                AccountDetails acc = new AccountDetails();
                //Selects Account
                accAccess.SelectAccount(accID);
                //Gets Account Details
                acc.userID = accAccess.GetOwner();
                acc.accID = accID;
                acc.accBal = accAccess.GetBalance();
                return acc;
            }
            catch(Exception ex)
            {
                log.LogMsg("Data-Tier AccountController", "Error Getting Account: " + accID);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message)
                });
                //If account does not exist then return null to be handled later
                //return null;
            }
        }

        /// <summary>
        /// Creates an Account
        /// </summary>
        /// <param name="userID">User ID associated with account</param>
        /// <returns>Newly created Account Details</returns>
        [Route("api/Account/Create/{userID}")]
        [HttpPost]
        public AccountDetails CreateAcc(uint userID)
        {
            log.LogMsg("Data-Tier AccountController", "Creating Account for User: " + userID);
            uint accID = accAccess.CreateAccount(userID);
            //Selects Account with corresponding Account ID
            accAccess.SelectAccount(accID);

            AccountDetails acc = new AccountDetails();
            acc.userID = accAccess.GetOwner();
            acc.accID = accID;
            acc.accBal = accAccess.GetBalance();
            return acc;
        }

        /// <summary>
        /// Deposits money into specified account
        /// </summary>
        /// <param name="accID">Account ID of Receiver</param>
        /// <param name="amt">Amount to Deposit</param>
        /// <returns>Amount Deposited</returns>
        [Route("api/Account/Deposit/{accID}/{amt}")]
        [HttpPost]
        public uint AccDepost(uint accID, uint amt)
        {
            try
            {
                log.LogMsg("Data-Tier AccountController", "Depositing $" + amt + " into Account: " + accID);
                accAccess.SelectAccount(accID);
                accAccess.Deposit(amt);
                return amt;
            }
            catch (Exception ex)
            {
                log.LogMsg("Data-Tier AccountController", "ERROR Depositing $" + amt + " into Account: " + accID);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message)
                });
            }
        }

        /// <summary>
        /// Withdraws money from specified account
        /// </summary>
        /// <param name="accID">Account ID of Account that money will be taken off</param>
        /// <param name="amt">Amount to Withdraw</param>
        /// <returns>Amount Withdrawn</returns>
        [Route("api/Account/Withdraw/{accID}/{amt}")]
        [HttpPost]
        public uint AccWithdraw(uint accID, uint amt)
        {
            try
            {
                log.LogMsg("Data-Tier AccountController", "Withdrawing $" + amt + " from Account: " + accID);
                accAccess.SelectAccount(accID);
                accAccess.Withdraw(amt);
                return amt;
            }
            catch(Exception ex)
            {
                log.LogMsg("Data-Tier AccountController", "ERROR Withdrawing $" + amt + " from Account: " + accID);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message)
                });
            }
        }

    }
}