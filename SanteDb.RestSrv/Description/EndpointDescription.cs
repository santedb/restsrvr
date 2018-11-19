using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Description
{
    /// <summary>
    /// Describes a single endpoint
    /// </summary>
    public sealed class EndpointDescription
    {
        // Trace source
        private TraceSource m_traceSource = new TraceSource(TraceSources.DescriptionTraceSourceName);

        private ContractDescription m_contract;
        private String m_listenUri;

        /// <summary>
        /// Creates a new endpoint description with the specified base URI and contract
        /// </summary>
        public EndpointDescription(Uri baseUri, ContractDescription contract)
        {
            this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Enter EndpointDescription CTOR ({0}, {1})", baseUri, contract);
            this.m_contract = contract;

            if (baseUri.Host == "0.0.0.0")
                this.m_listenUri = baseUri.ToString().Replace("://0.0.0.0", "://+");
            else 
                this.m_listenUri = baseUri.ToString();
            if (!this.m_listenUri.EndsWith("/"))
                this.m_listenUri += "/";
        }

        /// <summary>
        /// Create an endpoint description from the specified base URI and behavior
        /// </summary>
        /// <param name="baseUri">The base URI for listening</param>
        /// <param name="contractType">The contract type</param>
        public EndpointDescription(Uri baseUri, Type contractType) : this(baseUri, new ContractDescription(contractType))
        {
        }

        /// <summary>
        /// Gets the contract that this endpoint implements / uses
        /// </summary>
        public ContractDescription Contract => this.m_contract;

        /// <summary>
        /// The listening URI for the endpoint
        /// </summary>
        public String ListenUri => this.m_listenUri;

    }
}
