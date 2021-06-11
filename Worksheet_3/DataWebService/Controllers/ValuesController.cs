// Filename: ValuesController.cs
// Project:  DC Assignment 2 (Practical 3)
// Purpose:  Data Tier Web Service for Retrieving Records with ID
// Author:   George Aziz (19765453)
// Date:     04/05/2021

using API_Classes;
using DataWebService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataWebService.Controllers
{
    public class ValuesController : ApiController
    {
        /// <summary>
        /// Get User through ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Found User</returns>
        public DataIntermed Post(int id)
        {
            Database _database = Database.Get();
            if (id < 0 || id >= _database.GetNumRecords())
            {
                Debug.WriteLine("Data: Index inputted is out of range!");
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(string.Format("ID: {0} is out of range!", id))
                });
            }
            else
            {
                DataIntermed dataInter = new DataIntermed();
                dataInter.AcctNo = _database.GetAcctNoByIndex(id);
                dataInter.Pin = _database.GetPINByIndex(id);
                dataInter.Balance = _database.GetBalanceByIndex(id);
                dataInter.FirstName = _database.GetFirstNameByIndex(id);
                dataInter.LastName = _database.GetLastNameByIndex(id);
                ImageConverter converter = new ImageConverter();
                dataInter.Image = (byte[])converter.ConvertTo(_database.GetImageByIndex(id), typeof(byte[]));
                return dataInter;
            }
        }
    }
}
