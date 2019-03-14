using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// Represents a service fault attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true)]
    public class ServiceFaultAttribute : Attribute
    {

        /// <summary>
        /// Creates a new service fault
        /// </summary>
        public ServiceFaultAttribute(int status, Type faultType, String condition)
        {
            this.StatusCode = status;
            this.FaultType = faultType;
            this.Condition = condition;
        }

        /// <summary>
        /// Gets or sets the status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or set the type of fault
        /// </summary>
        public Type FaultType { get; set; }

        /// <summary>
        /// Gets or sets the condition of the fault
        /// </summary>
        public String Condition { get; set; }

    }
}
