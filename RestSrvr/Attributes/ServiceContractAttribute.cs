using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// Represents a REST contract
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceContractAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the contract
        /// </summary>
        public String Name { get; set; }

    }
}
