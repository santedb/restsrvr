using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSrvr.Message;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace RestSrvr
{
    /// <summary>
    /// Default dispatch formatter
    /// </summary>
    internal class DefaultDispatchFormatter : IDispatchFormatter
    {
        /// <summary>
        /// Serialize the request for the operation
        /// </summary>
        public void SerializeRequest(EndpointOperation operation, RestRequestMessage request, object[] parameters)
        {

        }

        /// <summary>
        /// Default serialization of the response
        /// </summary>
        public void SerializeResponse(RestResponseMessage responseMessage, object[] parameters, object result)
        {
            // By default unless Accept is application/json , we always prefer application/xml
            if(result == null)
            {
                responseMessage.StatusCode = 204;
            }
            else if(result is Stream)
            {
                responseMessage.ContentType = responseMessage.ContentType ?? "application/octet-stream";
                responseMessage.Body = result as Stream;
            }
            else if(result.GetType().IsPrimitive || result is string ||
                result is Guid) 
            {
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(result.ToString()));
                responseMessage.ContentType = responseMessage.ContentType ?? "text/plain";
                responseMessage.Body = ms;
            }
            else if (RestOperationContext.Current.IncomingRequest.Headers["Accept"]?.StartsWith("application/json") == true)
            {
                // Prepare the serializer
                JsonSerializer jsz = new JsonSerializer();
                var ms = new MemoryStream();
                using (var tms = new MemoryStream()) 
                using (StreamWriter sw = new StreamWriter(tms, Encoding.UTF8))
                using (JsonWriter jsw = new JsonTextWriter(sw))
                {
                    jsz.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    jsz.NullValueHandling = NullValueHandling.Ignore;
                    jsz.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    jsz.TypeNameHandling = TypeNameHandling.Auto;
                    jsz.Converters.Add(new StringEnumConverter());
                    jsz.Serialize(jsw, result);
                    jsw.Flush();
                    sw.Flush();
                    tms.Seek(0, SeekOrigin.Begin);
                    tms.CopyTo(ms);
                }
                ms.Seek(0, SeekOrigin.Begin);
                responseMessage.ContentType = responseMessage.ContentType ?? "application/json";
                responseMessage.Body = ms;
            }
            else
            {
                var xsz = new XmlSerializer(result.GetType());
                var ms = new MemoryStream();
                xsz.Serialize(ms, result);
                ms.Seek(0, SeekOrigin.Begin);
                responseMessage.ContentType = responseMessage.ContentType ?? "application/xml";
                responseMessage.Body = ms;
            }
        }
    }
}