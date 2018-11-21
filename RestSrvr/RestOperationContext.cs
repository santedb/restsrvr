using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr
{
    /// <summary>
    /// Represents the current operation context for the rest service thread
    /// </summary>
    public sealed class RestOperationContext
    {
        // Current reference for thread
        [ThreadStatic]
        private static RestOperationContext m_current;

        // Context
        private HttpListenerContext m_context;

        /// <summary>
        /// Creates a new operation context
        /// </summary>
        internal RestOperationContext(HttpListenerContext context)
        {
            this.m_context = context;
        }

        /// <summary>
        /// Gets the service of the context
        /// </summary>
        public ServiceEndpoint ServiceEndpoint { get; internal set; }

        /// <summary>
        /// Endpoint operation
        /// </summary>
        public EndpointOperation EndpointOperation { get; internal set; }

        /// <summary>
        /// Incoming request
        /// </summary>
        public HttpListenerRequest IncomingRequest => this.m_context.Request;

        /// <summary>
        /// Outgoing resposne
        /// </summary>
        public HttpListenerResponse OutgoingResponse => this.m_context.Response;
        
        /// <summary>
        /// Gets the current operation context
        /// </summary>
        public static RestOperationContext Current
        {
            get { return m_current; }
            internal set { m_current = value; }
        }
    }
}
