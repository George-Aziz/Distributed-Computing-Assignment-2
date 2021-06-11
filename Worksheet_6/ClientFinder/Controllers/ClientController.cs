using Client_API_Classes;
using ClientFinder.Models;
using Job_Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ClientFinder.Controllers
{
    public class ClientController : ApiController
    {
        [Route("api/ClientAPI/AddClient")]
        [HttpPost]
        public void AddClient(ClientInputData input)
        {
            Client newClient = new Client()
            {
                Address = input.Address,
                Port = input.Port,
                FinishedJobsCount = input.FinishedJobsCount
            };

            ClientList list = ClientList.Get();
            list.AddClient(newClient);
        }

        [Route("api/ClientAPI/RetrieveClients")]
        [HttpGet]
        public List<Client> RetrieveClients()
        {
            ClientList list = ClientList.Get();
            return list.RetrieveClients();
        }

        [Route("api/ClientAPI/GetClient/{port}")]
        [HttpGet]
        public Client GetClient(string port)
        {
            return FindClient(port);
        }

        [Route("api/ClientAPI/RemoveClient/{port}")]
        [HttpPost]
        public void RemoveClient(string port)
        {
            ClientList list = ClientList.Get();
            list.RemoveClient(FindClient(port));  
        }

        [Route("api/ClientAPI/IncrementJobCount/{port}")]
        [HttpPost]
        public void IncrementJobCount(string port)
        {
            //ClientList.Get().clients.Find(curClient => curClient.port == port).finishedJobsCount++;
            FindClient(port).FinishedJobsCount++;
        }

        private Client FindClient(string port)
        {
            try
            {
                ClientList list = ClientList.Get();
                List<Client> clientList = list.RetrieveClients();
                Client client = clientList.Find(curClient => curClient.Port == port); //Finds client since by port since it is unique
                return client;
            }
            catch (ArgumentNullException)
            {
                //Not throwing any exception as nothing can be done if a client is not found since data passed is all automatic
                Debug.WriteLine("Could not find Client: Port Number - " + port);
                return null;
            }
        }
    }
}

