// Filename: AdminController.cs
// Project:  DC Assignment 2 (Practical 5)
// Purpose:  Data Tier Controller for Admin Access Services
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
    public class AdminController : ApiController
    {
        private LogData log = new LogData();

        /// <summary>
        /// Saves current state (Accounts & Users) to disk
        /// </summary>
        [Route("api/Save")]
        [HttpPost]
        public void Save()
        {
            log.LogMsg("Data-Tier AdminController", "Saving To Disk");
            Bank.Get().GetBankData().SaveToDisk();
        }

        /// <summary>
        /// Processes all transactions so that users can see effect of any transactions made
        /// </summary>
        [Route("api/ProcessTransactions")]
        [HttpPost]
        public void ProcessTransactions()
        {
            try
            {
                log.LogMsg("Data-Tier AdminController", "Processing All Transactions");
                Bank.Get().GetBankData().ProcessAllTransactions();
            }
            catch(Exception ex)
            {
                log.LogMsg("Data-Tier AdminController", "ERROR Processing All Transactions");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message)
                });
            }
        }

    }
}