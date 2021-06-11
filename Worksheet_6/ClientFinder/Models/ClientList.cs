using Client_API_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientFinder.Models
{
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