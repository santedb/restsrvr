using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// Indicates that a particular operation can be invoked using HTTP
    /// </summary>
    public class RestInvokeAttribute : Attribute
    {

        /// <summary>
        /// Creates a new rest invokation attribute
        /// </summary>
        public RestInvokeAttribute(String method, String urlTemplate)
        {
            this.Method = method;
            this.UrlTemplate = urlTemplate;
        }

        /// <summary>
        /// Gets or sets the method
        /// </summary>
        public String Method { get; set; }

        /// <summary>
        /// Gets or sets the URL Template
        /// </summary>
        public String UrlTemplate { get; set; }

    }
    
}
