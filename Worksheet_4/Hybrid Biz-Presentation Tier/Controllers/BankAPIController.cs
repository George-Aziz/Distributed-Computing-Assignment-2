// Filename: BankAPIController.cs
// Project:  DC Assignment 2 (Practical 4)
// Purpose:  Business Tier Controller for all services relating to account, user & transactions
// Author:   George Aziz (19765453)
// Date:     06/04/2021

using API_Classes;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Hybrid_Biz_Presentation_Tier.Controllers
{
    public class BankAPIController : ApiController
    {
        private string URL = "https://localhost:44382";
        private RestClient client;
        private LogData log = new LogData();

        /// <summary>
        /// Creates a user and an account
        /// </summary>
        /// <param name="fName">First Name string </param>
        /// <param name="lName">Last Name string</param>
        /// <returns>Newly Created User Details</returns>
        [Route("api/BankAPI/CreateUser/{fName}/{lName}")]
        [HttpPost]
        public UserDetails CreateUserAndAccount(string fName, string lName)
        {
            //Ensures First Name is only letters
            foreach (char c in fName)
            {
                if (!char.IsLetter(c))
                {
                    log.LogMsg("Business-Tier BankAPI", "Invalid First Name Inputted: " + fName);
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("First name must all be completely alphabetical with no numbers!")
                    });
                }
            }
            //Ensures Last Name is only letters
            foreach (char c in lName)
            {
                if (!char.IsLetter(c))
                {
                    log.LogMsg("Business-Tier BankAPI", "Invalid Last Name Inputted: " + lName);
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Last name must all be completely alphabetical with no numbers!")
                    });
                }
            }

            //Create new User
            client = new RestClient(URL);
            RestRequest userRequest = new RestRequest("api/User/Create/" + fName + "/" + lName);
            IRestResponse userResponse = client.Post(userRequest);
            UserDetails user = JsonConvert.DeserializeObject<UserDetails>(userResponse.Content);
            log.LogMsg("Business-Tier BankAPI", "Creating new user under the name of " + fName + ", " + lName);

            //Create new Account
            RestRequest accRequest = new RestRequest("api/Account/Create/" + user.userID.ToString());
            client.Post(accRequest);
            log.LogMsg("Business-Tier BankAPI", "Creating new account under the name of " + fName + ", " + lName + " with user ID: " + user.userID);

            //Save Current State to disk
            RestRequest saveRequest = new RestRequest("api/Save");
            client.Post(saveRequest);
            log.LogMsg("Business-Tier BankAPI", "Saving to Disk");

            return user;
        }

        /// <summary>
        /// Gets Specific user with user ID
        /// </summary>
        /// <param name="_userID">User ID of user that needs to be found</param>
        /// <returns>User details of corresponding user ID</returns>
        [Route("api/BankAPI/GetUser/{_userID}")]
        [HttpGet]
        public UserDetails GetUser(string _userID)
        {
            //User ID is an unsigned integer and is first checked if its a valid uint
            if (!uint.TryParse(_userID, out uint userID))
            {
                log.LogMsg("Business-Tier BankAPI", "Invalid userID - Only Numbers Allowed");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Inputted userID is invalid - Ensure it is only numbers!")
                });
            }
            else //If input is a valid uint
            {
                //Retrieves User with User ID
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/User/Get/" + userID.ToString());
                IRestResponse response = client.Get(request);
                log.LogMsg("Business-Tier BankAPI", "Retrieving User with ID " + userID.ToString());
                if (response.StatusCode == HttpStatusCode.BadRequest) //If User was not found 
                {
                    log.LogMsg("Business-Tier BankAPI", "Invalid User ID");
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Invalid User ID!\n\n" + response.Content)
                    });
                }
                else //Valid User ID
                {
                    log.LogMsg("Business-Tier BankAPI", "Valid User ID Returned");
                    return JsonConvert.DeserializeObject<UserDetails>(response.Content);
                }
            }
        }

        /// <summary>
        /// Gets Specific user with user ID
        /// </summary>
        /// <param name="_accID">Account ID of account that needs to be found</param>
        /// <returns>Account details of corresponding account ID</returns>
        [Route("api/BankAPI/Account/{_accID}")]
        [HttpGet]
        public AccountDetails GetAccount(string _accID)
        {
            //Account ID is an unsigned integer and is first checked if its a valid uint
            if (!uint.TryParse(_accID, out uint accID))
            {
                log.LogMsg("Business-Tier BankAPI", "Invalid userID - Only Numbers Allowed");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Inputted User ID is invalid - Ensure it is only numbers!")
                });
            }
            else //If input is a valid uint
            {
                //Retrieves Account with Account ID
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/Account/" + accID.ToString());
                IRestResponse response = client.Get(request);
                log.LogMsg("Business-Tier BankAPI", "Getting Account Number " + accID.ToString());
                if (response.StatusCode == HttpStatusCode.BadRequest) //If Account was not found 
                {
                    log.LogMsg("Business-Tier BankAPI", "Invalid Account ID");
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Invalid Account ID!\n\n" + response.Content)
                    });
                }
                else //Valid Account ID
                {
                    log.LogMsg("Business-Tier BankAPI", "Valid Account ID Returned");
                    return JsonConvert.DeserializeObject<AccountDetails>(response.Content);
                }
            }
        }

        /// <summary>
        /// Deposits money into a specified account ID
        /// </summary>
        /// <param name="_accID">Account ID</param>
        /// <param name="_amt">Amount to be deposited</param>
        /// <returns>Amount Deposited</returns>
        [Route("api/BankAPI/Deposit/{_accID}/{_amt}")]
        [HttpPost]
        public uint DepositMoney(string _accID, string _amt)
        {
            //Account ID & Amount are an unsigned integer and is first checked if its a valid uint
            if (!uint.TryParse(_accID, out uint accID) || !uint.TryParse(_amt, out uint amt))
            {
                log.LogMsg("Business-Tier BankAPI", "Invalid Account ID or Amount - Only Numbers Allowed");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Inputted Account ID or Amount is invalid - Ensure it is only numbers!")
                });
            }
            else //If input is a valid uint
            {
                //Deposit Money
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/Account/Deposit/" + accID.ToString() + "/" + amt.ToString());
                IRestResponse response = client.Post(request);
                log.LogMsg("Business-Tier BankAPI", "Depositing $" + amt.ToString() + " under Account [" + accID.ToString() + "]");
                if (response.StatusCode == HttpStatusCode.BadRequest) //An error occured such as invalid account
                {
                    log.LogMsg("Business-Tier BankAPI", "Invalid Deposit");
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Invalid Deposit!\n\n" + response.Content)
                    });
                }
                else //Depost Succesfull
                {
                    //Save Current State to Disk
                    RestRequest saveRequest = new RestRequest("api/Save");
                    log.LogMsg("Business-Tier BankAPI", "Valid Deposit - Saving to Disk");
                    client.Post(saveRequest);

                    return Convert.ToUInt32(response.Content);
                }
            }
        }

        /// <summary>
        /// Withraws money from a specified account ID
        /// </summary>
        /// <param name="_accID">Account ID</param>
        /// <param name="_amt">Amount to be deposited</param>
        /// <returns>Amount Withdrawn</returns>
        [Route("api/BankAPI/Withdraw/{_accID}/{_amt}")]
        [HttpPost]
        public uint WithdrawMoney(string _accID, string _amt)
        {
            //Account ID & Amount are an unsigned integer and is first checked if its a valid uint
            if (!uint.TryParse(_accID, out uint accID) || !uint.TryParse(_amt, out uint amt))
            {
                log.LogMsg("Business-Tier BankAPI", "Invalid Account ID or Amount - Only Numbers Allowed");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Inputted Account ID or Amount is invalid - Ensure it is only numbers!")
                });
            }
            else //If input is a valid uint
            {
                //Withdraw Money
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/Account/Withdraw/" + accID.ToString() + "/" + amt.ToString());
                IRestResponse response = client.Post(request);
                log.LogMsg("Business-Tier BankAPI", "Withdrawing $" + amt.ToString() + " under Account [" + accID.ToString() + "]");
                if (response.StatusCode == HttpStatusCode.BadRequest) //An error occured such as invalid account
                {
                    log.LogMsg("Business-Tier BankAPI", "Invalid Withdrawal");
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Invalid Withdrawal!\n\n" + response.Content)
                    });
                }
                else //Withdrawal succesfull
                {
                    //Save Current State to disk
                    RestRequest saveRequest = new RestRequest("api/Save");
                    log.LogMsg("Business-Tier BankAPI", "Valid Withdrawal - Saving to Disk");
                    client.Post(saveRequest);

                    return Convert.ToUInt32(response.Content);
                }
            }
        }

        /// <summary>
        /// Creates a transaction from one account to another
        /// </summary>
        /// <param name="_amt">Amount to be transferred</param>
        /// <param name="_senderID">Account ID of Sender</param>
        /// <param name="_receiverID">Account ID of Receiver</param>
        /// <returns>Transaction details of the newly created transaction</returns>
        [Route("api/BankAPI/CreateTransaction/{_amt}/{_senderID}/{_receiverID}")]
        [HttpPost]
        public TransactionDetails CreateTransaction(string _amt, string _senderID, string _receiverID)
        {
            //Both Sender & Receiver IDs as well as amount are unsigned int so they must be first checked whether they are valid
            if (!uint.TryParse(_amt, out uint amt) || !uint.TryParse(_senderID, out uint senderID) || !uint.TryParse(_receiverID, out uint receiverID))
            {
                log.LogMsg("Business-Tier BankAPI", "Invalid Account IDs or Amount - Only Numbers Allowed");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Inputted Account IDs or Amount is invalid - Ensure it is only numbers!")
                });
            }
            else //If input is a valid uint
            {
                
                //Creates the new transaction
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/Transaction/Create/" + amt.ToString() + "/" + senderID.ToString() + "/" + receiverID.ToString());
                log.LogMsg("Business-Tier BankAPI", "Creating Transaction between Sender - " + senderID.ToString() + " and Reciver - " + receiverID.ToString() + "for $" + amt.ToString());
                IRestResponse response = client.Post(request);
                if (response.StatusCode == HttpStatusCode.BadRequest) //If the transaction failed to get created
                {
                    log.LogMsg("Business-Tier BankAPI", "Could not create new transaction!");
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) //Could be exception or not enough balance
                    {
                        Content = new StringContent("Could not create new transaction - Invalid Transaction!\n\n" + response.Content)
                    });
                }
                else //Valid transaction
                {
                    //Process all transactions in queue
                    try
                    {
                        ProcessTransactions();
                        //Save Current State to disk
                        RestRequest saveRequest = new RestRequest("api/Save");
                        log.LogMsg("Business-Tier BankAPI", "Saving to Disk");
                        client.Post(saveRequest);
                        return JsonConvert.DeserializeObject<TransactionDetails>(response.Content);
                    }
                    catch(HttpResponseException ex)
                    {
                        log.LogMsg("Business-Tier BankAPI", "Could not create new transaction!");
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Method to call the ProcessAllTransactions service from Data Tier
        /// </summary>
        private void ProcessTransactions()
        {
            RestRequest request = new RestRequest("api/ProcessTransactions");
            IRestResponse response = client.Post(request);
            log.LogMsg("Business-Tier BankAPI", "Processing All Transactions");
            if (response.StatusCode == HttpStatusCode.BadRequest) //If an error occurs
            {
                log.LogMsg("Business-Tier BankAPI", "Could not process all transactions!");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Could not create new transaction - Something went wrong with processing transaction!\n\n" + response.Content)
                });
            }
        }

        /// <summary>
        /// Gets All Transactions that has occurred in current runtime 
        /// </summary>
        /// <returns>TransactionDetails Array with all transactions</returns>
        [Route("api/BankAPI/GetTransactions")]
        [HttpGet]
        public TransactionDetails[] GetTransaction()
        {
            client = new RestClient(URL);
            RestRequest request = new RestRequest("api/Transaction/GetAll");
            IRestResponse response = client.Get(request);
            log.LogMsg("Business-Tier BankAPI", "Retrieving Transactions");
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                log.LogMsg("Business-Tier BankAPI", "Something Went Wrong Retrieving Transactions!");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Something Went Wrong Retrieving Transactions - Please Try Again Later!\n\n" + response.Content)
                });
            }
            else
            {
                log.LogMsg("Business-Tier BankAPI", "Transactions are being returned");
                return JsonConvert.DeserializeObject<List<TransactionDetails>>(response.Content).ToArray();
            }
        }
    }
}