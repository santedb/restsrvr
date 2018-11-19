using RestSrvr;
using RestSrvr.Message;
using System;

namespace TestServer
{
    /// <summary>
    /// Console log message inspector
    /// </summary>
    internal class ConsoleLogMessageInspector : IMessageInspector
    {

        [ThreadStatic]
        private static Guid sessionId;

        public void AfterReceiveRequest(RestRequestMessage request)
        {
            sessionId = Guid.NewGuid();
            Console.WriteLine("RQO: {0} ({1} {2})", sessionId, request.Method, request.Url);
        }

        public void BeforeSendResponse(RestResponseMessage response)
        {
            Console.WriteLine("RSP: {0} ({1})", sessionId, response.StatusCode);
        }
    }
}