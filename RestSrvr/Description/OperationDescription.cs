using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RestSrvr.Attributes;
using System.Diagnostics;

namespace RestSrvr.Description
{
    /// <summary>
    /// Represents an operation description
    /// </summary>
    public sealed class OperationDescription
    {

        // Trace source
        private TraceSource m_traceSource = new TraceSource(TraceSources.DescriptionTraceSourceName);
        private List<IOperationBehavior> m_operationBehaviors = new List<IOperationBehavior>();

        /// <summary>
        /// Operation description
        /// </summary>
        public OperationDescription(MethodInfo operationMethod)
        {
            this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Enter OperationDescription CTOR ({0})", operationMethod);
            this.InvokeMethod = operationMethod;
            var restAttribute = operationMethod.GetCustomAttribute<RestInvokeAttribute>();
            if (restAttribute == null)
                throw new InvalidOperationException($"Method {operationMethod.Name} does not have [RestInvokeAttribute] and cannot be used as an operation");
            this.Method = restAttribute.Method;
            this.UriTemplate = restAttribute.UriTemplate;
        }

        /// <summary>
        /// Add an operation behavior to the description
        /// </summary>
        public void AddOperationBehavior(IOperationBehavior behavior)
        {
            this.m_operationBehaviors.Add(behavior);
        }

        /// <summary>
        /// Gets the operation behaviors on this behavior
        /// </summary>
        public IEnumerable<IOperationBehavior> Behaviors => this.m_operationBehaviors.AsReadOnly();

        /// <summary>
        /// Invoke the specified method
        /// </summary>
        public MethodInfo InvokeMethod { get; private set; }

        /// <summary>
        /// Gets the method that triggers this operation to be fired
        /// </summary>
        public String Method { get; private set; }

        /// <summary>
        /// The template that should be used to invoke the method
        /// </summary>
        public String UriTemplate { get; private set; }


    }
}
