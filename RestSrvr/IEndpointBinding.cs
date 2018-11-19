using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr
{
    /// <summary>
    /// Represents an endpoint binding for the specific endpoint
    /// </summary>
    public interface IEndpointBinding
    {
        /// <summary>
        /// Attach the service context's endpoint to the binding
        /// </summary>
        void AttachEndpoint(ServiceDispatcher dispatcher, ServiceEndpoint endpoint);

        /// <summary>
        /// Detach the endpoint
        /// </summary>
        void DetachEndpoint(ServiceEndpoint endpoint);
    }
}
