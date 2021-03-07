using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindMiners.MiniProjeto.Applications.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
