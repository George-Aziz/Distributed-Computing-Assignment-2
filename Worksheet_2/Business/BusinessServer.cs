// Filename: BusinessServer.cs
// Project:  DC Assignment 2 (Practical 2)
// Purpose:  Business Server Interface Implentation
// Author:   George Aziz (19765453)
// Date:     22/04/2021

using BizDLL;
using DBDLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class BusinessServer : BusinessServerInterface
    {
        private ChannelFactory<DataServerInterface> dataFactory;
        private static DataServerInterface serverConnection;
        private uint LogNumber;

        /// <summary>
        /// Sets up connection with data tier as it will be needed for functions in business tier
        /// </summary>
        public BusinessServer()
        {
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            dataFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            serverConnection = dataFactory.CreateChannel();
        }

        /// <summary>
        /// Gets Number of records in database from data tier
        /// </summary>
        /// <returns>Integer that represents amount of records in database</returns>
        public int GetNumEntries()
        {
            try
            {
                LogNumber++;
                Log("Retrieved Total Number Of Entries Available");
                return serverConnection.GetNumEntries();
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {

                NetTcpBinding tcp = new NetTcpBinding();
                string URL = "net.tcp://localhost:8100/DataService";
                dataFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
                serverConnection = dataFactory.CreateChannel();
                Log("Data Service is down");
                throw new FaultException<BusinessFault>(new BusinessFault()
                { Issue = "Data Service is down - Please Try Again Later" });
            }
        }

        /// <summary>
        /// Gets Record Values in Database through index search from data tier
        /// </summary>
        /// <param name="index"></param>
        /// <param name="acctNo"></param>
        /// <param name="pin"></param>
        /// <param name="bal"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <param name="image"></param>
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap image)
        {
            try
            {
                serverConnection.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out image);
                Log("Searched Entry With Index: " + index);
            }
            catch (FaultException<IndexOutOfRangeFault> ex)
            {
                Log("Index inputted is out of range");
                throw new FaultException<BusinessFault>(new BusinessFault()
                { Issue = "Index was out of range" });
            }
            catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
            {

                NetTcpBinding tcp = new NetTcpBinding();
                string URL = "net.tcp://localhost:8100/DataService";
                dataFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
                serverConnection = dataFactory.CreateChannel();
                Log("Data Service is down");
                throw new FaultException<BusinessFault>(new BusinessFault()
                { Issue = "Data Service is down - Please Try Again Later" });
            }
        }

        /// <summary>
        /// Gets Record Values in Database through last name search
        /// </summary>
        /// <param name="name"></param>
        /// <param name="acctNo"></param>
        /// <param name="pin"></param>
        /// <param name="bal"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <param name="image"></param>
        public void SearchEntry(string name, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap image)
        {
            uint _acctNo = 0, _pin = 0;
            int _bal = 0;
            string _fName = null, _lName = null;
            Bitmap _image = null;
            if (String.IsNullOrWhiteSpace(name)) //To save time for user, if the inputted string is null, whitespace or emppty then it will instantly throw an error
            {
                Log("Invalid Last Name Input - Whitespace only, empty or null!");
                throw new FaultException<BusinessFault>(new BusinessFault()
                { Issue = "Invalid Last Name Input: Ensure not whitespace only, empty or null! - Please Try Again" });
            }
            else
            {
                try
                {
                    for (int i = 0; i < serverConnection.GetNumEntries(); i++) //Goes through All entries in database
                    {
                        string curLName;
                        serverConnection.GetValuesForEntry(i, out _, out _, out _, out _, out curLName, out _); //Finds the last name through index search for each
                        if (curLName.ToUpper().Equals(name.ToUpper())) //If the found last name is equal then it will return
                        {
                            serverConnection.GetValuesForEntry(i, out _acctNo, out _pin, out _bal, out _fName, out _lName, out _image);
                            break;
                        }
                    }

                    if (String.IsNullOrWhiteSpace(_fName) && String.IsNullOrWhiteSpace(_lName)) //One final check is to ensure that both retireved names aren't empty (Not found)
                    {
                        Log("Unable to retrieve entry due to inputted last name not being found!");
                        throw new FaultException<BusinessFault>(new BusinessFault()
                        { Issue = "Last name not found!" });
                    }
                    else //Names are valud which mean have been found and can be returned to user
                    {
                        acctNo = _acctNo; pin = _pin; bal = _bal; fName = _fName; lName = _lName; image = _image;
                    }
                }
                catch (Exception ex) when (ex is EndpointNotFoundException || ex is CommunicationObjectFaultedException || ex is CommunicationException)
                {

                    NetTcpBinding tcp = new NetTcpBinding();
                    string URL = "net.tcp://localhost:8100/DataService";
                    dataFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
                    serverConnection = dataFactory.CreateChannel();
                    Log("Data Service is down");
                    throw new FaultException<BusinessFault>(new BusinessFault()
                    { Issue = "Data Service is down - Please Try Again Later" });
                }
            }
        }

        /// <summary>
        /// Logs everything that is happening to Console of Business Tier
        /// </summary>
        /// <param name="logString"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Log(string logString)
        {
            Console.WriteLine("Task #" + LogNumber + ": " + logString);
            LogNumber++;
        }
    }
}
