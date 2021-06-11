// Filename: BusinessException.cs
// Project:  DC Assignment 2 (Practical 3)
// Purpose:  Any Business Exception
// Author:   George Aziz (19765453)
// Date:     25/04/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDLL
{
    [Serializable]
    public class BusinessException : Exception
    {
        public BusinessException() { }

        public BusinessException(string message)
            : base(message) { }

        public BusinessException(string message, Exception inner)
            : base(message, inner) { }
    }
}
