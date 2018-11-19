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
    public class FaultException : Exception
    {

        /// <summary>
        /// Creates a new fault
        /// </summary>
        public FaultException(Int32 statusCode) : this(statusCode, null, null)
        {
        }

        /// <summary>
        /// Creates a new fault
        /// </summary>
        public FaultException(Int32 statusCode, String message) : this(statusCode, message, null)
        {
        }

        /// <summary>
        /// Creates a new fault
        /// </summary>
        public FaultException(Int32 statusCode, String message, Exception innerException) : base(message, innerException)
        {
            this.StatusCode = statusCode;
        }
        /// <summary>
        /// Status code to supply
        /// </summary>
        public Int32 StatusCode { get; set; }

    }
    /// <summary>
    /// Represents a fault with a type of response body to be sent
    /// </summary>
    public class FaultException<TBody> : FaultException
    {

       
        /// <summary>
        /// The body object of the fault
        /// </summary>
        public TBody Body { get; set; }

        /// <summary>
        /// Represents the fault exception
        /// </summary>
        public FaultException(Int32 statusCode, TBody data) : this(statusCode, data, null, null)
        {
        }

        /// <summary>
        /// Represents the fault exception
        /// </summary>
        public FaultException(Int32 statusCode, TBody data, String message) : this(statusCode, data, message, null)
        {
        }

        /// <summary>
        /// Represents the fault exception
        /// </summary>
        public FaultException(Int32 statusCode, TBody data, String message, Exception innerException) : base(statusCode, message, innerException)
        {
            this.Body = data;
        }


    }
}
