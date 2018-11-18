using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.RestSrv.Description
{
    /// <summary>
    /// Represents an operation description
    /// </summary>
    public class OperationDescription
    {

        /// <summary>
        /// Invoke the specified method
        /// </summary>
        public MethodInfo InvokeMethod { get; private set; }

        /// <summary>
        /// Gets the method that triggers this operation to be fired
        /// </summary>
        public String Method { get; private set; }

        /// <summary>
        /// The template that should be used to invoke the method
        /// </summary>
        public String UriTemplate { get; private set; }

    }
}
