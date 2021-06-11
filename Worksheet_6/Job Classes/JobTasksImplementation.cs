// Filename: JobTasksImplementation.cs
// Project:  DC Assignment 2 (Practical 6)
// Purpose:  Implementation for JobTasks Interface
// Author:   George Aziz (19765453)
// Date:     16/05/2021

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Job_Classes
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    public class JobTasksImplementation : IJobTasks
    {
        /// <summary>
        /// Gets the next job on the list that hasn't been taken
        /// </summary>
        /// <returns>Job</returns>
        public Job GetJob()
        {
            Job retJob = null; //By default there is no job and will be null
            foreach(Job job in JobList.Jobs)
            {
                if(!job.Requested) //Can only take jobs that have not been taken yet
                {
                    job.Requested = true; //Sets current job as requested to not be picked by anyone else
                    retJob = job;
                    break; //Breaks moment a job is found
                }
            }
            return retJob;
        }

        /// <summary>
        /// Updates a job with a result and sets it to complete
        /// </summary>
        /// <param name="job">The Job object</param>
        /// <param name="result">The result string</param>
        public void UpdateJobResult(Job job, string result)
        {
            foreach (Job curJob in JobList.Jobs)
            {
                if (curJob.ID == job.ID)
                {
                    curJob.PyResult = result;
                    curJob.Completed = true;
                }
            }
        }

        /// <summary>
        /// Updates a job to have a failed state
        /// </summary>
        /// <param name="job">The Job object</param>
        public void FailJob(Job job)
        {
            foreach (Job curJob in JobList.Jobs)
            {
                if (curJob.ID == job.ID)
                {
                    Debug.WriteLine("\n\n\n\n\n\nTEST");
                    curJob.Failed = true;
                }
            }
        }
    }
}
