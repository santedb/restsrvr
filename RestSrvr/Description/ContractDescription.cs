using RestSrvr.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace RestSrvr.Description
{
    /// <summary>
    /// Represents a contract description
    /// </summary>
    public sealed class ContractDescription
    {
        // Trace source
        private TraceSource m_traceSource = new TraceSource(TraceSources.DescriptionTraceSourceName);

        /// <summary>
        /// Operations on the contract
        /// </summary>
        private List<OperationDescription> m_operations = new List<OperationDescription>();

        /// <summary>
        /// Gets the operations
        /// </summary>
        public IEnumerable<OperationDescription> Operations => this.m_operations.AsReadOnly();

        /// <summary>
        /// Gets the name of the contract
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Create a new contract description from the specified contract
        /// </summary>
        public ContractDescription(Type contractType)
        {
            this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Enter ContractDescription CTOR ({0})", contractType);

            if (!contractType.IsInterface)
                throw new InvalidOperationException("Contract type must be an interface");

            // Get the name of the contract
            this.Name = contractType.GetCustomAttribute<ServiceContractAttribute>()?.Name ?? contractType.FullName;
            this.m_operations = contractType.GetRuntimeMethods().Where(m => m.GetCustomAttribute<RestInvokeAttribute>() != null).Select(m => new OperationDescription(m)).ToList();
        }
    }
}