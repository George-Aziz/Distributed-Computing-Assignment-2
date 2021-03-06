using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDLL
{
    [Serializable]
    public class OutOfRangeException : Exception
    {
        public OutOfRangeException() { }

        public OutOfRangeException(string message)
            : base(message) { }

        public OutOfRangeException(string message, Exception inner)
            : base(message, inner) { }
    }
}
