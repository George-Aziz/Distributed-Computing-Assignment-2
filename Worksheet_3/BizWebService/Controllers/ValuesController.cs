// Filename: ValuesController.cs
// Project:  DC Assignment 2 (Practical 3)
// Purpose:  Business Tier Web Service Controller for retrieving record through ID
// Author:   George Aziz (19765453)
// Date:     04/05/2021

using API_Classes;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;

namespace BizWebService.Controllers
{
    public class ValuesController : ApiController
    {
        /// <summary>
        /// Find user through ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Found User</returns>
        public DataIntermed Get(int id)
        {
            RestClient _client = new RestClient("https://localhost:44331/");
            RestRequest _request = new RestRequest("api/values/" + id.ToString());

            IRestResponse _response = _client.Post(_request);
                
            if (_response.StatusCode == HttpStatusCode.BadRequest) //Error happened in Data Service
            {
                Debug.WriteLine("Business: Index inputted is out of range!");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(_response.Content)
                });
            }
            else
            {
                return JsonConvert.DeserializeObject<DataIntermed>(_response.Content);
            }
        }
    }
}
