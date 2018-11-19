using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// Identifies the GET operation can be used to invoke a method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : RestInvokeAttribute
    {
        /// <summary>
        /// Creates a GET invokation attribute on the method
        /// </summary>
        /// <param name="urlTemplate"></param>
        public GetAttribute(String urlTemplate) : base("GET", urlTemplate)
        {

        }
    }
}
