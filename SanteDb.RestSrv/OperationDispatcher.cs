using SanteDB.RestSrv.Exceptions;
using SanteDB.RestSrv.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SanteDB.RestSrv
{
    class OperationDispatcher
    {
        /// <summary>
        /// Represents the operation path regex
        /// </summary>
        private readonly Regex m_templateParser = new Regex(@"([\w\{\}\*]+)");

        // Endpoint operation
        private EndpointOperation m_endpointOperation;
        private Regex m_dispatchRegex;

        /// <summary>
        /// Gets or sets the dispatch formatter
        /// </summary>
        public IDispatchFormatter DispatchFormatter { get; set; }

        /// <summary>
        /// Creats a new operation dispatcher
        /// </summary>
        public OperationDispatcher(EndpointOperation endpointOperation)
        {
            this.m_endpointOperation = endpointOperation;

            var match = this.m_templateParser.Match(endpointOperation.Description.UriTemplate);
            var regexBuilder = new StringBuilder("^.*?/");
            while(match.Success)
            {
                int parmCount = 0;
                Type[] parmTypes = endpointOperation.Description.InvokeMethod.GetParameters().Select(o => o.ParameterType).ToArray();

                switch (match.Groups[1].Value[0])
                {
                    case '{': // parameter
                        if (parmTypes.Length < parmCount) throw new InvalidOperationException($"REST method accepts {parmTypes.Length} parameters but route specifies more");
                        var ptype = parmTypes[parmCount++];
                        switch(ptype.Name.ToLowerInvariant())
                        {
                            case "string":
                                regexBuilder.Append("(.*?)");
                                break;
                            case "int":
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

                regexBuilder.Append("/?");
            }
            regexBuilder.Append("$");

            this.m_dispatchRegex = new Regex(regexBuilder.ToString(), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Determines if the message can be dispatched
        /// </summary>
        internal bool CanDispatch(RestRequestMessage requestMessage)
        {
            return requestMessage.Method.ToLowerInvariant() == this.m_endpointOperation.Description.Method.ToLowerInvariant()
                && this.m_dispatchRegex.IsMatch(requestMessage.Url.ToString());
        }


        /// <summary>
        /// Dispatch the message
        /// </summary>
        internal bool Dispatch(ServiceDispatcher serviceDispatcher, RestRequestMessage requestMessage, RestResponseMessage responseMessage)
        {
            try
            {
                var invoke = this.m_endpointOperation.Description.InvokeMethod;
                var parameters = new object[invoke.GetParameters().Length];
                this.DispatchFormatter.SerializeRequest(this.m_endpointOperation, requestMessage, parameters);

                // Validate parameters
                if (!Enumerable.SequenceEqual(invoke.GetParameters().Select(o => o.ParameterType), parameters.Select(o => o.GetType())))
                    throw new FaultException<String>(HttpStatusCode.BadRequest, "Bad Request");

                object result = invoke.Invoke(serviceDispatcher.BehaviorInstance, parameters);

                this.DispatchFormatter.SerializeResponse(this.m_endpointOperation, responseMessage, parameters, result);
                return true;
            }
            catch(Exception e)
            {
                return serviceDispatcher.HandleFault(e, responseMessage);
            }
        }
    }
}
