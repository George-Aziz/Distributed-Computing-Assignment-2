using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientFinder.Models
{
    public class Client
    {
        public string address;
        public string port;

        public Client(string _address, string _port)
        {
            address = _address;
            port = _port;
        }
    }
}