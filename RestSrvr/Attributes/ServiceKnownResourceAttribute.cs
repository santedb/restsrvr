using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// Indicates a known rest resource on the rest service 
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple =true)]
    public class ServiceKnownResourceAttribute : Attribute
    {

        /// <summary>
        /// Create a new rest resource type
        /// </summary>
        public ServiceKnownResourceAttribute(Type t)
        {
            this.Type = t;
        }

        /// <summary>
        /// Gets the type 
        /// </summary>
        public Type Type { get; set; }
    }
}

