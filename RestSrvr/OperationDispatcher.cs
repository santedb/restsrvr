using RestSrvr.Exceptions;
using RestSrvr.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RestSrvr
{
    /// <summary>
    /// Represents a dispatcher that routes messages to a particular method
    /// </summary>
    public sealed class OperationDispatcher
    {

        private TraceSource m_traceSource = new TraceSource(TraceSources.DispatchTraceSourceName);

        /// <summary>
        /// Represents the operation path regex
        /// </summary>
        private readonly Regex m_templateParser = new Regex(@"([\w\{\}\*\.\-_]+)");

        // Endpoint operation
        private EndpointOperation m_endpointOperation;
        private Regex m_dispatchRegex;
        private string[] m_regexGroupNames;

        /// <summary>
        /// Gets or sets the dispatch formatter
        /// </summary>
        public IDispatchMessageFormatter DispatchFormatter { get; set; }

        /// <summary>
        /// Creats a new operation dispatcher
        /// </summary>
        public OperationDispatcher(EndpointOperation endpointOperation)
        {
            this.m_endpointOperation = endpointOperation;
            var match = this.m_templateParser.Match(endpointOperation.Description.UriTemplate);
            var regexBuilder = new StringBuilder("^");

            this.DispatchFormatter = new DefaultDispatchFormatter();

            Type[] parmTypes = endpointOperation.Description.InvokeMethod.GetParameters().Select(o => o.ParameterType).ToArray();
            m_regexGroupNames = new string[parmTypes.Length];
            int parmCount = 0;

            while (match.Success)
            {
                switch (match.Groups[1].Value[0])
                {
                    case '{': // parameter
                        if (parmTypes.Length < parmCount) throw new InvalidOperationException($"REST method accepts {parmTypes.Length} parameters but route specifies more");
                        m_regexGroupNames[parmCount] = match.Groups[1].Value;
                        var ptype = parmTypes[parmCount++];
                        switch(ptype.Name.ToLowerInvariant())
                        {
                            case "string":
                                regexBuilder.Append(@"([A-Za-z0-9_\-%]*?)");
                                break;
                            case "int32":
                                regexBuilder.Append("(\\d*?)");
                                break;
                            case "guid":
                                regexBuilder.Append("([a-f0-9]{8}-(?:[a-f0-9]{4}-){3}[a-f0-9]{12})");
                                break;
                            default:
                                throw new InvalidOperationException($"Cannot use parameter type {ptype.Name} on route");
                        }
                        break;
                    case '*': // anything 
                        regexBuilder.Append(".*");
                        break;
                    default:
                        regexBuilder.Append(match.Groups[1].Value);
                        break;
                }

                regexBuilder.Append("/");
                match = match.NextMatch();
            }
            regexBuilder.Append("?$");

            this.m_regexGroupNames = this.m_regexGroupNames.Where(o => o != null).ToArray();
            this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Operation {0} will be bound to {1}", endpointOperation.Description.InvokeMethod, regexBuilder);
            this.m_dispatchRegex = new Regex(regexBuilder.ToString(), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Determines if the message can be dispatched
        /// </summary>
        internal bool CanDispatch(RestRequestMessage requestMessage)
        {
            this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "OpertionDispatcher.CanDispatch -> {0} {1} (EPRx: {2}/{3})", requestMessage.Method, requestMessage.Url, this.m_endpointOperation.Description.Method, this.m_dispatchRegex);

            return this.m_dispatchRegex.IsMatch(requestMessage.OperationPath);
        }

        /// <summary>
        /// Dispatch the message
        /// </summary>
        internal bool Dispatch(ServiceDispatcher serviceDispatcher, RestRequestMessage requestMessage, RestResponseMessage responseMessage)
        {
            try
            {
                this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Begin operation dispatch of {0} {1} to {2}", requestMessage.Method, requestMessage.Url, this.m_endpointOperation.Description.InvokeMethod);

                var invoke = this.m_endpointOperation.Description.InvokeMethod;
                var parameters = new object[invoke.GetParameters().Length];

                // By default parameters are passed by name
                var parmMatch = this.m_dispatchRegex.Match(requestMessage.OperationPath);
                for(int i = 0; i < this.m_regexGroupNames.Length; i++)
                {
                    var pindex = Array.FindIndex(invoke.GetParameters(), o => $"{{{o.Name}}}" == this.m_regexGroupNames[i]);
                    var sparm = invoke.GetParameters()[pindex];
                    object sval = parmMatch.Groups[i+1].Value;
                    if (sparm.ParameterType == typeof(int))
                        sval = Int32.Parse(sval.ToString());
                    else if (sparm.ParameterType == typeof(Guid))
                        sval = Guid.Parse(sval.ToString());

                    parameters[pindex] = sval;
                }

                this.DispatchFormatter.DeserializeRequest(this.m_endpointOperation, requestMessage, parameters);

                this.m_traceSource.TraceData(TraceEventType.Verbose, 0, parameters);

                // Validate parameters
                if (!Enumerable.SequenceEqual(invoke.GetParameters().Select(o => o.ParameterType), parameters.Select(o => o.GetType())))
                    throw new FaultException(400, "Bad Request");

                // Gather instance 
                object instance = serviceDispatcher.Service.Instance;
                if (serviceDispatcher.Service.InstanceMode == Attributes.ServiceInstanceMode.PerCall)
                    instance = Activator.CreateInstance(serviceDispatcher.Service.BehaviorType);

                object result = invoke.Invoke(instance, parameters);

                this.DispatchFormatter.SerializeResponse(responseMessage, parameters, result);
                return true;
            }
            catch(TargetInvocationException e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                return serviceDispatcher.HandleFault(e.InnerException, responseMessage);
            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                return serviceDispatcher.HandleFault(e, responseMessage);
            }
        }
    }
}
