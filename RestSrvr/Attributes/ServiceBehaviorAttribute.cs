using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{

    /// <summary>
    /// Service instance mode
    /// </summary>
    public enum ServiceInstanceMode
    {
        /// <summary>
        /// New instance is made per call
        /// </summary>
        PerCall,
        /// <summary>
        /// Only one instance is ever created
        /// </summary>
        Singleton
    }

    /// <summary>
    /// Represents a rest service behavior
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceBehaviorAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the behavior
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the instance mode
        /// </summary>
        public ServiceInstanceMode InstanceMode { get; set; }
    }
}
