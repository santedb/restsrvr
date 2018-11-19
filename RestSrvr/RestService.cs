using RestSrvr.Attributes;
using RestSrvr.Description;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr
{
    /// <summary>
    /// Represents a simple HttpRestServer
    /// </summary>
    public sealed class RestService
    {

        // The instance
        private object m_instance;

        // Trace source
        private TraceSource m_traceSource = new TraceSource(TraceSources.TraceSourceName);

        // Service behaviors
        private List<IServiceBehavior> m_serviceBehaviors = new List<IServiceBehavior>();

        /// <summary>
        /// Gets the current service behaviors
        /// </summary>
        public IEnumerable<IServiceBehavior> ServiceBehaviors => this.m_serviceBehaviors.AsReadOnly();

        /// <summary>
        /// Get whether the service is running
        /// </summary>
        public bool IsRunning { get; private set;  }

        /// <summary>
        /// Start this service
        /// </summary>
        public void Start()
        {
            if (this.IsRunning)
                throw new InvalidOperationException("Already running");
            this.IsRunning = true;
            this.m_traceSource.TraceInformation("Starting RestService {0}", this.Name);

            var dispatcher = new ServiceDispatcher(this);
            foreach (var bhvr in this.m_serviceBehaviors)
                bhvr.ApplyServiceBehavior(this, dispatcher);

            // Apply behaviors
            foreach (var ep in this.Endpoints)
            {
                foreach (var bhvr in ep.Behaviors)
                    bhvr.ApplyEndpointBehavior(ep, ep.Dispatcher);
                foreach (var op in ep.Operations)
                    foreach (var bhvr in op.Description.Behaviors)
                        bhvr.ApplyOperationBehavior(op, op.Dispatcher);
                ep.Binding.AttachEndpoint(dispatcher, ep);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            foreach (var ep in this.Endpoints)
                ep.Binding.DetachEndpoint(ep);
            this.IsRunning = false;
        }

        /// <summary>
        /// Gets the name of the rest service
        /// </summary>
        public string Name { get; }

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
        public void AddServiceEndpoint(Uri baseUri, Type contractType, IEndpointBinding binding)
        {
            if (baseUri == null)
                throw new ArgumentNullException(nameof(baseUri));
            else if (contractType == null)
                throw new ArgumentNullException(nameof(contractType));
            else if (this.m_endpoints.Any(o => o.Description.ListenUri == baseUri.ToString()))
                throw new InvalidOperationException("Another endpoint has already been registered with this base URI");
            else if (!contractType.IsAssignableFrom(this.m_instance.GetType()))
                throw new InvalidOperationException($"{this.m_instance.GetType().FullName} does not implement contract {contractType.FullName}");

            this.m_endpoints.Add(new ServiceEndpoint(new EndpointDescription(baseUri, contractType), binding));
        }
        
        /// <summary>
        /// Creates the specified HttpHostContext
        /// </summary>
        public RestService(Type behaviorType)
        {
            this.m_instance = Activator.CreateInstance(behaviorType);
            this.Name = behaviorType.GetCustomAttribute<RestBehaviorAttribute>()?.Name ?? behaviorType.FullName;
        }

        /// <summary>
        /// Adds a service behavior to this instance
        /// </summary>
        public void AddServiceBehavior(IServiceBehavior behavior)
        {
            if (this.IsRunning)
                throw new InvalidOperationException("Cannot add policy when service is running");
            this.m_serviceBehaviors.Add(behavior);
        }
    }
}
