using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fruixion.bebop.Exceptions
{

    /// <summary>
    /// Command Exception
    /// </summary>
    public class CommandException : Exception
    {
        public CommandException() : base("The Previous command was not sent.") { }

        public CommandException(string message) : base(message) { }

    }
}
