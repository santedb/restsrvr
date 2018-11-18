using SanteDB.RestSrv.Message;

namespace SanteDB.RestSrv
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