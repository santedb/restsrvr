using System;
using System.Collections.Generic;

namespace SanteDB.RestSrv.Description
{
    /// <summary>
    /// Represents a contract description
    /// </summary>
    public class ContractDescription
    {

        /// <summary>
        /// Operations on the contract
        /// </summary>
        private List<OperationDescription> m_operations = new List<OperationDescription>();

        /// <summary>
        /// Gets the operations
        /// </summary>
        public IEnumerable<OperationDescription> Operations => this.m_operations.AsReadOnly();

        /// <summary>
        /// Create a new contract description from the specified contract
        /// </summary>
        public ContractDescription(Type contractType)
        {
            if (!contractType.IsInterface)
                throw new InvalidOperationException("Contract type must be an interface");
        }
    }
}