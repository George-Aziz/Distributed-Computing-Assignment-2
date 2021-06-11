// Filename: ClientList.cs
// Project:  DC Assignment 2 (Practical 8)
// Purpose:  Client List Model
// Author:   George Aziz (19765453)
// Date:     16/05/2021

using Client_API_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientFinder.Models
{
    /// <summary>
    /// Singleton object for client list tasks and retrieval
    /// </summary>
    public class ClientList
    {
        public readonly List<Client> clients;
        private static ClientList instance= null;
        public static ClientList Get()
        {
            if(instance == null)
            {
                instance = new ClientList();
            }
            return instance;
        }

        private ClientList()
        {
            clients = new List<Client>();
        }

        public void AddClient(Client _client)
        {
            clients.Add(_client);
        }

        public List<Client> RetrieveClients()
        {
            return clients;
        }

        public void RemoveClient(Client _client)
        {
            clients.Remove(_client);
        }
    }
}