using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// Represents a rest service behavior
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RestBehaviorAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the behavior
        /// </summary>
        public String Name { get; set; }
    }
}
