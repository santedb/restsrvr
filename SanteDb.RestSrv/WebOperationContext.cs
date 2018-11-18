using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.RestSrv
{
    /// <summary>
    /// Represents the current operation context for the rest service thread
    /// </summary>
    public class WebOperationContext
    {
        // Current reference for thread
        [ThreadStatic]
        private static WebOperationContext m_current;
        // Context
        private HttpListenerContext m_context;

        /// <summary>
        /// Creates a new operation context
        /// </summary>
        internal WebOperationContext(HttpListenerContext context)
        {
            this.m_context = context;
        }

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
        public static WebOperationContext Current => m_current;
    }
}
