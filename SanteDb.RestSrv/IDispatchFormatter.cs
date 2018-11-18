using RestSrvr.Message;

namespace RestSrvr
{
    public interface IDispatchFormatter
    {

        /// <summary>
        /// Serialize the request 
        /// </summary>
        void SerializeRequest(EndpointOperation operation, RestRequestMessage request, object[] parameters);

        /// <summary>
        /// Serialize the response
        /// </summary>
        void SerializeResponse(EndpointOperation operation, RestResponseMessage response, object[] parameters, object result);

    }
}