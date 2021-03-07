using System;
using System.Collections.Generic;
using System.Text;

namespace MindMiners.MiniProjeto.Domains.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(string message) : base(message)
        {
        }

        public CustomException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
