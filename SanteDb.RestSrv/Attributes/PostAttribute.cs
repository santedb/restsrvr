using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// Indicates POST can be used to access a contract
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : RestInvokeAttribute
    {
        /// <summary>
        /// Creates a GET invokation attribute on the method
        /// </summary>
        public PostAttribute(String urlTemplate) : base("POST", urlTemplate)
        {
        }

    }
}
