using SanteDB.RestSrv.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.RestSrv
{
    /// <summary>
    /// Represents a simple HttpRestServer
    /// </summary>
    public class RestService
    {

        // The instance
        private object m_instance;

        // Contracts on this rest server
        private List<ServiceEndpoint> m_endpoints = new List<ServiceEndpoint>();
        
        /// <summary>
        /// Gets the endpoints for this rest server host
        /// </summary>
        public IEnumerable<ServiceEndpoint> Endpoints => this.m_endpoints.AsReadOnly();

        /// <summary>
        /// Get the instance
        /// </summary>
        internal object Instance => this.m_instance;

        /// <summary>
        /// Add a service endpoint 
        /// </summary>
        /// <param name="endpoint">The endpoint to add</param>
        public void AddServiceEndpoint(ServiceEndpoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            this.m_endpoints.Add(endpoint);
        }

        /// <summary>
        /// Registers the service behavior <paramref name="contractType"/> at base Uri <paramref name="baseUri"/>
        /// </summary>
        public void AddServiceEndpoint(Uri baseUri, Type contractType)
        {
            if (baseUri == null)
                throw new ArgumentNullException(nameof(baseUri));
            else if (contractType == null)
                throw new ArgumentNullException(nameof(contractType));
            else if (this.m_endpoints.Any(o => o.Description.ListenUri == baseUri))
                throw new InvalidOperationException("Another endpoint has already been registered with this base URI");
            else if (contractType.IsAssignableFrom(this.m_instance.GetType()))
                throw new InvalidOperationException($"{this.m_instance.GetType().FullName} does not implement contract {contractType.FullName}");
            this.m_endpoints.Add(new ServiceEndpoint(new EndpointDescription(baseUri, contractType)));
        }

        /// <summary>
        /// Creates the specified HttpHostContext
        /// </summary>
        public RestService(Type behaviorType)
        {
            this.m_instance = Activator.CreateInstance(behaviorType);
        }
        
    }
}
