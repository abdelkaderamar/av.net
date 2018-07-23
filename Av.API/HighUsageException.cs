using System;

namespace Av.API
{
    public class HighUsageException : Exception
    {
        public HighUsageException() : base() {}

        public HighUsageException(string msg) : base(msg)
        {
        }
    }
}
