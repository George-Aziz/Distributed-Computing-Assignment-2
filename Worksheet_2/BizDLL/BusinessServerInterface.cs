// Filename: BusinessServerInterface.cs
// Project:  DC Assignment 2 (Practical 2)
// Purpose:  Business Server Interface for client interaction
// Author:   George Aziz (19765453)
// Date:     22/04/2021

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BizDLL
{
    [ServiceContract]
    public interface BusinessServerInterface
    {
        [OperationContract]
        int GetNumEntries();
        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap image);
        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        void SearchEntry(string name, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap image);
    }
}
