using SanteDB.RestSrv.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SanteDB.RestSrv
{
    /// <summary>
    /// Represents an endpoint on an HTTP Rest Server
    /// </summary>
    public class ServiceEndpoint
    {

        // Description of this endpoint
        private EndpointDescription m_description;
        private EndpointDispatcher m_dispatcher ;
        private List<IEndpointBehavior> m_behaviors = new List<IEndpointBehavior>();
        private List<EndpointOperation> m_operations = new List<EndpointOperation>();

        /// <summary>
        /// Gets the description of this
        /// </summary>
        public EndpointDescription Description => this.m_description;

        /// <summary>
        /// Gets the endpoint behaviors
        /// </summary>
        public IEnumerable<IEndpointBehavior> Behaviors => this.m_behaviors.AsReadOnly();

        /// <summary>
        /// The dispatcher attached to the endpoint
        /// </summary>
        internal EndpointDispatcher Dispatcher => this.m_dispatcher;

        /// <summary>
        /// Gets the endpoint operations
        /// </summary>
        public IEnumerable<EndpointOperation> Operations => this.m_operations.AsReadOnly();

        /// <summary>
        /// Add an endpoint behavior
        /// </summary>
        public void AddEndpointBehavior(IEndpointBehavior behavior)
        {
            this.m_behaviors.Add(behavior);
        }

        /// <summary>
        /// Creates a new service endpoint
        /// </summary>
        public ServiceEndpoint(EndpointDescription description)
        {
            this.m_description = description;
            this.m_dispatcher = new EndpointDispatcher(this);
            this.m_operations = description.Contract.Operations.Select(o => new EndpointOperation(o)).ToList();
        }
        
    }
}
