using RestSrvr.Exceptions;
using RestSrvr.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr
{
    /// <summary>
    /// Represents a dispatcher that can call / invoke requests
    /// </summary>
    public sealed class ServiceDispatcher
    {

        // The service this is applied to
        private RestService m_service;
        private List<IServiceErrorHandler> m_errorHandlers = new List<IServiceErrorHandler>() { new DefaultErrorHandler() };
        private List<IServicePolicy> m_servicePolicies = new List<IServicePolicy>();
        private TraceSource m_traceSource = new TraceSource(TraceSources.DispatchTraceSourceName);

        /// <summary>
        /// Gets or sets the authorization provider
        /// </summary>
        public IEnumerable<IServicePolicy> Policies => this.m_servicePolicies.AsReadOnly();

        /// <summary>
        /// Add service policy
        /// </summary>
        public void AddServiceDispatcherPolicy(IServicePolicy policy)
        {
            this.m_servicePolicies.Add(policy);
        }

        /// <summary>
        /// Gets the error handlers on this service
        /// </summary>
        public List<IServiceErrorHandler> ErrorHandlers => this.m_errorHandlers;

        /// <summary>
        /// Get the behavior instance
        /// </summary>
        internal RestService Service => this.m_service;

        /// <summary>
        /// Creates a new service dispatcher
        /// </summary>
        public ServiceDispatcher(RestService service)
        {
            this.m_service = service;
        }

        /// <summary>
        /// Determine whether the specified service can dispatch given the HTTP context
        /// </summary>
        internal bool CanDispatch(RestRequestMessage requestMessage)
        {
            return this.m_service.Endpoints.Any(o => o.Dispatcher.CanDispatch(requestMessage));
        }

        /// <summary>
        /// Perform the dispatching function of the service
        /// </summary>
        internal bool Dispatch(RestRequestMessage requestMessage, RestResponseMessage responseMessage)
        {
        
            try
            {
                this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Begin service dispatch of {0} {1} > {2}", requestMessage.Method, requestMessage.Url, this.m_service.Name);
                // Apply the policy on the specified context
                foreach (var pol in this.m_servicePolicies)
                    pol.Apply(requestMessage);

                // Endpoint 
                var ep = this.m_service.Endpoints.FirstOrDefault(o => o.Dispatcher.CanDispatch(requestMessage));

                // Find the endpoint 
                if (ep == null)
                    throw new FaultException(404, "Resource not Found");

                return ep.Dispatcher.Dispatch(this, requestMessage, responseMessage);
            }
            catch(Exception e)
            {
                return this.HandleFault(e, responseMessage);
            }
        }

        /// <summary>
        /// Handle the provided fault
        /// </summary>
        internal bool HandleFault(Exception e, RestResponseMessage responseMessage)
        {
            var erh = this.m_errorHandlers.FirstOrDefault(o => o.HandleError(e));
            erh?.ProvideFault(e, responseMessage);
            return true;
        }
    }
}
