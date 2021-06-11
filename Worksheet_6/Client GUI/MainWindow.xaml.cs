// Filename: Maindow.xaml.cs
// Project:  DC Assignment 2 (Practical 6)
// Purpose:  Client GUI To Post Jobs
// Author:   George Aziz (19765453)
// Date:     22/05/2021

using Client_API_Classes;
using IronPython.Hosting;
using IronPython.Runtime;
using Job_Classes;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Win32;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows;


namespace Client_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Thread serverThread;
        private readonly Thread networkThread;
        private readonly string _url = "https://localhost:44364/";
        private readonly RestClient _client;
        private uint _currentPort;
        private bool _windowClosed;
        private readonly LogData _logger;
        private static Mutex mutex = new Mutex();

        public MainWindow()
        {
            InitializeComponent();
            //Client setup
            _client = new RestClient(_url);
            _currentPort = PortCounter.CurrentPort;
            _logger = new LogData();

            // Makes text box have ability of new line and tab characters for code to be inputted
            PyInputBox.AcceptsReturn = true;
            PyInputBox.AcceptsTab = true;

            _logger.LogMsg("Main Program","Initialising Server Thread");
            //Server Thread setup and execution
            serverThread = new Thread(new ThreadStart(DoServerThread));
            serverThread.Start();

            _logger.LogMsg("Main Program", "Initialising Network Thread");
            //Network Thread setup and execution
            networkThread = new Thread(new ThreadStart(DoNetworkThread));
            networkThread.Start();
        }

        /// <summary>
        /// Server Thread to Host new client and add client to Client List
        /// </summary>
        public void DoServerThread()
        {
            bool serverCreated = false;
            ServiceHost host;
            do
            {
                try
                {
                    _windowClosed = false;
                    //Each client will have their own Job Task Server
                    NetTcpBinding tcp = new NetTcpBinding();
                    host = new ServiceHost(typeof(JobTasksImplementation));
                    host.AddServiceEndpoint(typeof(IJobTasks), tcp, "net.tcp://127.0.0.1:" + _currentPort.ToString() + "/JobTaskServer");
                    host.Open();
                    
                    //Adds client into client list to be seen by other clients
                    RestRequest request = new RestRequest("api/ClientAPI/AddClient");
                    ClientInputData input = new ClientInputData
                    {
                        Address = "127.0.0.1",
                        Port = _currentPort.ToString()
                    };
                    request.AddJsonBody(input);
                    _client.Post(request);
                    serverCreated = true; //Server is now created which means no need for another iteration
                    
                    _logger.LogMsg("Server Thread", "Client: " + _currentPort + " has been added");
                    while (!_windowClosed) //Keeps server thread open until window is closed. Meanwhile updates GUI
                    {
                        Thread.Sleep(1000);
                        this.Dispatcher.Invoke(() =>
                        {
                            RestRequest clientRequest = new RestRequest("api/ClientAPI/GetClient/" + _currentPort.ToString());
                            IRestResponse response = _client.Get(clientRequest);
                            Client curClient = JsonConvert.DeserializeObject<Client>(response.Content);
                            FinishedJobsLabel.Text = "Total Jobs Finished: " + curClient.FinishedJobsCount;

                            ResultsBox.Items.Clear(); //Clears before adding all jobs again or else duplicates will appear
                            foreach (Job curJob in JobList.Jobs)
                            {
                                if (curJob.HostPort.Equals(_currentPort.ToString())) //Only displays client's jobs
                                {
                                    JobListView jobView = new JobListView();
                                    if (curJob.Requested && !curJob.Completed && curJob.Failed) //Invalid Job
                                    {
                                        jobView.Status = "Job Failed";
                                        jobView.Result = "Invalid Job";
                                    }
                                    else if (curJob.Requested && curJob.Completed) //If job has been completed
                                    {
                                        jobView.Status = "Job Complete";
                                        jobView.Result = curJob.PyResult;
                                    }
                                    else if (curJob.Requested && !curJob.Completed && !curJob.Failed) //If job has been taken but not yet completed
                                    {
                                        jobView.Status = "Completing Job";
                                        jobView.Result = "No Result Yet";
                                    }
                                    else //If not taken yet
                                    {
                                        jobView.Status = "Job Incomplete";
                                        jobView.Result = "No Result Yet";
                                    }
                                    jobView.Job = curJob.ID;
                                    ResultsBox.Items.Add(jobView);
                                }
                            }
                        });
                    }
                    host.Close();
                }
                catch (AddressAlreadyInUseException) //Current port is in use
                {
                    _logger.LogMsg("Server Thread", "Port number already in use, retrying...");
                    PortCounter.CurrentPort++;
                    _currentPort++; //Increases Port Number incase it is already being used and will try again
                }
            } while (!serverCreated);
        }

        /// <summary>
        /// Network thread to find other client's jobs and complete them 
        /// </summary>
        public void DoNetworkThread()
        {
            NetTcpBinding tcp = new NetTcpBinding();
            ChannelFactory<IJobTasks> jobFactory;
            IJobTasks serverConnection;
            string _url;

            //Runs till program ends completely
            while (true)
            {
                Thread.Sleep(2000); //Sleeps for 2 seconds so that program doesn't lag and have too many to process
                mutex.WaitOne();
                RestRequest request = new RestRequest("api/ClientAPI/RetrieveClients");
                IRestResponse response = _client.Get(request);
                List<Client> clientList = JsonConvert.DeserializeObject<List<Client>>(response.Content);

                foreach (Client curClient in clientList)
                {
                    if (_currentPort.ToString() != curClient.Port) //Client can process all jobs but their own
                    {
                        //Creates connection to client's jobtaskserver through each client's address and port
                        _url = "net.tcp://" + curClient.Address + ":" + curClient.Port + "/JobTaskServer";
                        jobFactory = new ChannelFactory<IJobTasks>(tcp, _url);
                        serverConnection = jobFactory.CreateChannel();

                        //Gets the next job on the list
                        Job newJob = serverConnection.GetJob();
                        if (newJob != null) //If newJob == null then that means there are no jobs and so do nothing
                        {
                            //Decode the Python Code from job
                            byte[] decodedBytes = Convert.FromBase64String(newJob.PyCode);
                            string pyCode = Encoding.UTF8.GetString(decodedBytes);

                            //SHA256 Verification
                            byte[] jobHash = newJob.Hash;
                            SHA256 hasher = SHA256.Create();
                            byte[] curHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(newJob.PyCode));
                            if (curHash.SequenceEqual(jobHash)) //If true then the data is fine and verified
                            {
                                try
                                {
                                    //IronPython runs the python script
                                    ScriptEngine engine = Python.CreateEngine();
                                    ScriptScope scope = engine.CreateScope();
                                    engine.Execute(pyCode, scope);
                                    dynamic function = scope.GetVariable("main");
                                    var result = function(); //Result from python script

                                    string resultString = result.ToString();
                                    serverConnection.UpdateJobResult(newJob, resultString); //Uploads the result to the job

                                    //Increments Count of Client that posted/hosted the job
                                    RestRequest countRequest = new RestRequest("api/ClientAPI/IncrementJobCount/" + _currentPort.ToString());
                                    _client.Post(countRequest);
                                    _logger.LogMsg("Network Thread", "Client: " + _currentPort + " processed a job");
                                }
                                catch (SyntaxErrorException) //If the code has a python syntax error
                                {
                                    serverConnection.FailJob(newJob);
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        MessageBox.Show("Invalid Python Code Input! - Ensure input has poper python indentation");
                                    });
                                    
                                    _logger.LogMsg("Network Thread", "Client: " + _currentPort + " tried to complete a job but is was invalid");
                                }
                                catch (UnboundNameException) //If the code has variable names issue
                                {
                                    serverConnection.FailJob(newJob);
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        MessageBox.Show("Invalid Python Code Input! - Ensure all variables have names");
                                    });
                                    
                                    _logger.LogMsg("Network Thread", "Client: " + _currentPort + " tried to complete a job but is was invalid");
                                }
                                catch (NullReferenceException) //If the code doesn't have a return value
                                {
                                    serverConnection.FailJob(newJob);
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        MessageBox.Show("Invalid Python Code Input! - Ensure python script has a return value");
                                    });
                                    _logger.LogMsg("Network Thread", "Client: " + _currentPort + " tried to complete a job but is was invalid");
                                }
                            }
                        }
                    }
                }
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Before window closes, ensures client is not on client list anymore (Removes "Dead" clients)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            serverThread.Abort();
            networkThread.Abort();
            _windowClosed = true; //Makes bool value true so server thread can end
            RestRequest request = new RestRequest("api/ClientAPI/RemoveClient/" + _currentPort.ToString());
            _client.Post(request); //Removes current exiting client as they will no longer be active
            _logger.LogMsg("Server Thread", "Client: " + _currentPort + " is leaving");

        }

        /// <summary>
        /// File upload Button to upload file contents into python code input textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileChooser = new OpenFileDialog();

            if (fileChooser.ShowDialog() == true) //If client has selected a file from the dialog box
            {
                try
                {
                    PyInputBox.Text = File.ReadAllText(fileChooser.FileName); //Reads chosen file into textbox
                    _logger.LogMsg("Main Program", "Client: " + _currentPort + " uploaded a python file");
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Post Job Button Click to upload a job for other client's to see and solve
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostJob_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(PyInputBox.Text) && PyInputBox.Text.StartsWith("def main():") && PyInputBox.Text.Contains("return"))
            {
                //Encodes Python Code In Base64
                byte[] bytes = Encoding.UTF8.GetBytes(PyInputBox.Text);
                string encodedPyCode = Convert.ToBase64String(bytes);

                //Computes hash for python code
                SHA256 hash = SHA256.Create();
                byte[] hashBytes = Encoding.UTF8.GetBytes(encodedPyCode);
                byte[] hashedData = hash.ComputeHash(hashBytes);
                Job job = new Job
                {
                    ID = JobList.Jobs.Count + 1,
                    Hash = hashedData,
                    PyCode = encodedPyCode,
                    HostPort = _currentPort.ToString(),
                    Requested = false,
                    Failed = false,
                    Completed = false
                };

                // Adds Job into Job List
                JobList.Jobs.Add(job);
                _logger.LogMsg("Main Program", "Client: " + _currentPort + " posted a python job script to other clients");
            }
            else //Python code may be empty, doesn't start with 'def main():' or doesn't have return value
            {
                this.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Invalid Python Code Input! - Ensure the code begins with 'def main():' with a return value");
                });
            }
        }
    }
}
