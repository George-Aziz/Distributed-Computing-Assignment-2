// Filename: Job.cs
// Project:  DC Assignment 2 (Practical 6)
// Purpose:  Job struct
// Author:   George Aziz (19765453)
// Date:     16/05/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job_Classes
{
    public class Job
    {
        public int ID;
        public string HostPort;
        public bool Requested;
        public bool Completed;
        public bool Failed;
        public string PyCode;
        public string PyResult;
        public byte[] Hash;
    }
}
