// Filename: AllController.cs
// Project:  DC Assignment 2 (Practical 3)
// Purpose:  Data Tier Web Service for Retrieving Number of Records
// Author:   George Aziz (19765453)
// Date:     25/04/2021

using DataWebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataWebService.Controllers
{
    public class AllController : ApiController
    {
        /// <summary>
        /// Gets number of entries in database
        /// </summary>
        /// <returns>Number of entries</returns>
        public int Get()
        {
            Database _database = Database.Get();
            return _database.GetNumRecords();
        }
    }
}