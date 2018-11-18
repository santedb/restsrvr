using RestSrvr.Message;

namespace RestSrvr
{
    /// <summary>
    /// Represents a message inspector
    /// </summary>
    public interface IMessageInspector
    {

        /// <summary>
        /// Request message
        /// </summary>
        void AfterReceiveRequest(RestRequestMessage request);

        /// <summary>
        /// Called before sending the response
        /// </summary>
        void BeforeSendResponse(RestResponseMessage response);
    }
}