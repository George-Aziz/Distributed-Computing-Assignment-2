// Filename: ValuesController.cs
// Project:  DC Assignment 2 (Practical 3)
// Purpose:  Business Tier Web Service Controller for retrieving number of records from data tier
// Author:   George Aziz (19765453)
// Date:     25/04/2021

using API_Classes;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;

namespace BizWebService.Controllers
{
    public class AllController : ApiController
    {
        /// <summary>
        /// Gets Number of Entries found in database
        /// </summary>
        /// <returns>Num of Entries in database</returns>
        public int Get()
        {
            RestClient _client = new RestClient("https://localhost:44331/");
            RestRequest _request = new RestRequest("api/all");

            IRestResponse _response = _client.Get(_request);
            return Int32.Parse(_response.Content);
        }
    }
}