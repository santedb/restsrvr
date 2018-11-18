using SanteDB.RestSrv.Exceptions;
using SanteDB.RestSrv.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.RestSrv
{
    /// <summary>
    /// Represents a dispatcher that can call / invoke requests
    /// </summary>
    public class ServiceDispatcher
    {

        // The service this is applied to
        private RestService m_service;
        private List<IServiceErrorHandler> m_errorHandlers;
        private List<IServicePolicy> m_servicePolicies;

        /// <summary>
        /// Gets or sets the authorization provider
        /// </summary>
        public IEnumerable<IServicePolicy> Policies => this.m_servicePolicies.AsReadOnly();

        /// <summary>
        /// Gets the error handlers on this service
        /// </summary>
        public List<IServiceErrorHandler> ErrorHandlers => this.m_errorHandlers;

        /// <summary>
        /// Get the behavior instance
        /// </summary>
        internal object BehaviorInstance => this.m_service.Instance;

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
                // Apply the policy on the specified context
                foreach (var pol in this.m_servicePolicies)
                    pol.Apply(requestMessage);

                // Endpoint 
                var ep = this.m_service.Endpoints.FirstOrDefault(o => o.Dispatcher.CanDispatch(requestMessage));

                // Find the endpoint 
                if (ep == null)
                    throw new FaultException<String>(HttpStatusCode.NotFound, "Specified endpoint not found");

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
