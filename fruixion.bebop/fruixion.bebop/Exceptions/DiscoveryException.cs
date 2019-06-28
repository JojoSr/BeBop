using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fruixion.bebop.Exceptions
{
    /// <summary>
    /// Discovery Exception
    /// </summary>
    /// <remarks>
    /// Occurs when an exception happens in the discovery process
    /// </remarks>
    public class DiscoveryException : Exception
    {

        public DiscoveryException() : base("Discovery Failed: No Drones Found.") { }

        public DiscoveryException(string message) : base(message) { }
    }
}
