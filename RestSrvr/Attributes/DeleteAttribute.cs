using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// Represents a contract can be invoked with DELETE
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DeleteAttribute : RestInvokeAttribute
    {
        /// <summary>
        /// Delete attribute
        /// </summary>
        public DeleteAttribute(String urlTemplate) : base("DELETE", urlTemplate)
        {

        }
    }
}
