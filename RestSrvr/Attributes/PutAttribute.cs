using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// Represents a method can be invoked with PUT
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PutAttribute : RestInvokeAttribute
    {
        /// <summary>
        /// Creates a new instance of the PUT attribute
        /// </summary>
        public PutAttribute(String urlTemplate) : base("PUT", urlTemplate)
        {

        }
    }
}
