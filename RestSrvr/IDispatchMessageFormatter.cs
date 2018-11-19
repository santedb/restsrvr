using RestSrvr.Message;

namespace RestSrvr
{
    public interface IDispatchMessageFormatter
    {

        /// <summary>
        /// Serialize the request 
        /// </summary>
        void DeserializeRequest(EndpointOperation operation, RestRequestMessage request, object[] parameters);

        /// <summary>
        /// Serialize the response
        /// </summary>
        void SerializeResponse(RestResponseMessage response, object[] parameters, object result);

    }
}