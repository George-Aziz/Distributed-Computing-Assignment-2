// Filename: IndexOutOfRangeFault.cs
// Project:  DC Assignment 2 (Practical 2)
// Purpose:  FaultContract for IndexOutOfRange Exceptions
// Author:   George Aziz (19765453)
// Date:     22/04/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DBDLL
{
    [DataContract]
    public class IndexOutOfRangeFault
    {
        [DataMember]
        public string Issue { get; set; }
    }
}
