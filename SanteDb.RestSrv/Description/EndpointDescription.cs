using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.RestSrv.Description
{
    /// <summary>
    /// Describes a single endpoint
    /// </summary>
    public class EndpointDescription
    {

        private ContractDescription m_contract;
        private Uri m_listenUri;

        /// <summary>
        /// Creates a new endpoint description with the specified base URI and contract
        /// </summary>
        public EndpointDescription(Uri baseUri, ContractDescription contract)
        {
            this.m_contract = contract;
            this.m_listenUri = baseUri;
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
        public Uri ListenUri => this.m_listenUri;

    }
}
