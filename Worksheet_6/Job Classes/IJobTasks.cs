// Filename: IJobTasksInterface.cs
// Project:  DC Assignment 2 (Practical 6)
// Purpose:  Job Tasks Interface 
// Author:   George Aziz (19765453)
// Date:     16/05/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Job_Classes
{
    [ServiceContract]
    public interface IJobTasks
    {  
        [OperationContract]
        Job GetJob();

        [OperationContract]
        void UpdateJobResult(Job job, string result);

        [OperationContract]
        void FailJob(Job job);
    }
}
