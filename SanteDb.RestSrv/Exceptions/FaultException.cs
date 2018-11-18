using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Exceptions
{
    /// <summary>
    /// Represents a fault
    /// </summary>
    public class FaultException<TBody> : Exception
    {

        /// <summary>
        /// Status code to supply
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The body object of the fault
        /// </summary>
        public TBody Body { get; set; }

        /// <summary>
        /// Represents the fault exception
        /// </summary>
        public FaultException(HttpStatusCode codeToSupply, TBody data)
        {
            this.StatusCode = codeToSupply;
        }

    }
}
