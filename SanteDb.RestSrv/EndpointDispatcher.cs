using SanteDB.RestSrv.Exceptions;
using SanteDB.RestSrv.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SanteDB.RestSrv
{
    /// <summary>
    /// The service dispatcher is responsible for linking the HTTP listener to the
    /// RestService
    /// </summary>
    public class EndpointDispatcher
    {
        // The rest service that is attached to this dispatcher
        private ServiceEndpoint m_serviceEndpoint;
        private Regex m_endpointRegex;
        private List<IMessageInspector> m_messageInspector = new List<IMessageInspector>();

        /// <summary>
        /// Gets the message inspectors
        /// </summary>
        public List<IMessageInspector> MessageInspectors => this.m_messageInspector;

        /// <summary>
        /// Creates a new endpoint dispatcher for the specified endpoint
        /// </summary>
        public EndpointDispatcher(ServiceEndpoint serviceEndpoint)
        {
            this.m_serviceEndpoint = serviceEndpoint;
            this.m_endpointRegex = new Regex($"^{serviceEndpoint.Description.ListenUri.Scheme}://.*?:{serviceEndpoint.Description.ListenUri.Port}{serviceEndpoint.Description.ListenUri.AbsolutePath}/?.*");
        }

        /// <summary>
        /// Determines if the dispatcher can dispatch the supplied request message
        /// </summary>
        internal bool CanDispatch(RestRequestMessage requestMessage)
        {
            // Match the path
            return this.m_endpointRegex.IsMatch(requestMessage.Url.ToString()) &&
                this.m_serviceEndpoint.Operations.Any(o=>o.Dispatcher.CanDispatch(requestMessage));
        }

        /// <summary>
        /// Dispatch the HttpRequest message to the appropriate service
        /// </summary>
        internal bool Dispatch(ServiceDispatcher serviceDispatcher, RestRequestMessage requestMessage, RestResponseMessage responseMessage)
        {

            // Allow message inspectors to inspect the message before next stage
            try
            {
                foreach (var mfi in this.m_messageInspector)
                    mfi.AfterReceiveRequest(requestMessage);

                var op = this.m_serviceEndpoint.Operations.FirstOrDefault(o => o.Dispatcher.CanDispatch(requestMessage));
                if (op == null)
                    throw new FaultException<String>(HttpStatusCode.NotFound, "Specified resource was not found");
                op.Dispatcher.Dispatch(serviceDispatcher, requestMessage, responseMessage);

                // Allow message inspectors to inspect before sending response
                foreach (var mfi in this.m_messageInspector)
                    mfi.BeforeSendResponse(responseMessage);

                return true;
            }
            catch(Exception e)
            {
                return serviceDispatcher.HandleFault(e);
            }
        }

    }
}
