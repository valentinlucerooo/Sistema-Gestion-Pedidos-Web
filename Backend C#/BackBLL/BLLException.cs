using System;
using System.Collections.Generic;
using System.Text;

namespace BackBLL
{
    public class BLLException : Exception
    {
        public BLLException(string Exception) : base(Exception) { }
    }
}
