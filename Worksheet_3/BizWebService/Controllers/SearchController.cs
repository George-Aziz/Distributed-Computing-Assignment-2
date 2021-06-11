// Filename: SearchController.cs
// Project:  DC Assignment 2 (Practical 3)
// Purpose:  Business Tier Web Service Controller for retrieving record through Last name
// Author:   George Aziz (19765453)
// Date:     04/05/2021

using API_Classes;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;

namespace BizWebService.Controllers
{
    public class SearchController : ApiController
    {

        /// <summary>
        /// Finds a user through last name
        /// </summary>
        /// <param name="value">Last name of user</param>
        /// <returns>Found User</returns>
        public DataIntermed Post(SearchData value)
        {
            RestClient _client = new RestClient("https://localhost:44331/");
            RestRequest _requestNum = new RestRequest("api/all");
            IRestResponse _responseNum = _client.Get(_requestNum);
            int _numEntries = Int32.Parse(_responseNum.Content);

            RestRequest _requestData;
            if (String.IsNullOrWhiteSpace(value.searchStr)) //To save time for user, if the inputted string is null, whitespace or emppty then it will instantly throw an error
            {
                Debug.WriteLine("Invalid Last Name Input as it either contains whitespace only, is empty or null");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Invalid Last Name Input - Ensure it is not empty, null or whitespace only")
                });
            }
            else
            {
                DataIntermed _curData = null;
                DataIntermed _retData = null;
               
                for (int i = 0; i < _numEntries; i++) //Goes through All entries in database
                {
                    _requestData = new RestRequest("api/values/" + i);
                    IRestResponse _responseData = _client.Post(_requestData);
                    _curData = JsonConvert.DeserializeObject<DataIntermed>(_responseData.Content);
                    if (_curData.LastName.ToUpper().Equals(value.searchStr.ToUpper())) //If the found last name is equal then it will return
                    {
                        _retData = _curData;
                        break;
                    }
                }

                //After all names have gone through, need to check if there is a name found
                if (_retData == null)
                {
                    Debug.WriteLine("Last name not found");
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Inputted last name not found")
                    });
                }
                else //Not null means a valid name match was found
                {
                    return _curData;
                }
             
            }
        }
    }
}