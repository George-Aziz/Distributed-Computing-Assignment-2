// Filename: UserController.cs
// Project:  DC Assignment 2 (Practical 4)
// Purpose:  Data Tier Controller for User Access Services
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
    public class UserController : ApiController
    {
        private BankDB.UserAccessInterface userAccess = Bank.Get().GetUserAccess();
        private LogData log = new LogData();

        /// <summary>
        /// Gets a User through specified User ID
        /// </summary>
        /// <param name="userID">ID of user to be retrieved</param>
        /// <returns>User details of found user</returns>
        [Route("api/User/Get/{userID}")]
        [HttpGet]
        public UserDetails GetUser(uint userID)
        {
            try
            {
                log.LogMsg("Data-Tier UserController", "Getting User: " + userID);
                UserDetails user = new UserDetails();
                userAccess.SelectUser(userID);

                userAccess.GetUserName(out user.firstName, out user.lastName);
                user.userID = userID;
                
                return user;
            }
            catch (Exception ex)
            {
                log.LogMsg("Data-Tier UserController", "ERROR Getting User: " + userID);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message)
                });
            }
        }

        /// <summary>
        /// Creates a new User
        /// </summary>
        /// <param name="fName">First Name of User</param>
        /// <param name="lName">Last Name of User</param>
        /// <returns>User Details of newly created User</returns>
        [Route("api/User/Create/{fName}/{lName}")]
        [HttpPost]
        public UserDetails CreateUser(string fName, string lName)
        {
            log.LogMsg("Data-Tier UserController", "Creating User: " + fName +  " " + lName);
            UserDetails user = new UserDetails();
            user.userID = userAccess.CreateUser();
            userAccess.SelectUser(user.userID);
            
            user.firstName = fName;
            user.lastName = lName;

            userAccess.SetUserName(user.firstName, user.lastName);
            userAccess.GetUserName(out user.firstName, out user.lastName); //To ensure it has been made correctly
            return user;
        }
    }
}