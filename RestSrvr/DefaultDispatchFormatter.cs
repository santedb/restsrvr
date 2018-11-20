using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSrvr.Message;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace RestSrvr
{
    /// <summary>
    /// Default dispatch formatter
    /// </summary>
    internal class DefaultDispatchFormatter : IDispatchMessageFormatter
    {

        // Trace source
        private TraceSource m_traceSource = new TraceSource(TraceSources.MessageTraceSourceName);

        /// <summary>
        /// Serialize the request for the operation
        /// </summary>
        public void DeserializeRequest(EndpointOperation operation, RestRequestMessage request, object[] parameters)
        {
            try
            {
                var httpRequest = RestOperationContext.Current.IncomingRequest;
                string contentType = httpRequest.Headers["Content-Type"];

                for (int pNumber = 0; pNumber < parameters.Length; pNumber++)
                {
                    var parm = operation.Description.InvokeMethod.GetParameters()[pNumber];

                    // Simple parameter
                    if (parameters[pNumber] != null)
                    {
                        continue; // dispatcher already populated
                    }
                    // Use XML Serializer
                    else if (contentType?.StartsWith("application/xml") == true)
                    {
                        XmlSerializer serializer = new XmlSerializer(parm.ParameterType);
                        var requestObject = serializer.Deserialize(request.Body);
                        parameters[pNumber] = requestObject;
                    }
                    else if (contentType?.StartsWith("application/json") == true)
                    {
                        using (var sr = new StreamReader(request.Body))
                        {
                            JsonSerializer jsz = new JsonSerializer()
                            {
                                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                                TypeNameHandling = TypeNameHandling.All
                            };
                            jsz.Converters.Add(new StringEnumConverter());
                            var dserType = parm.ParameterType;
                            parameters[pNumber] = jsz.Deserialize(sr, dserType);
                        }
                    }
                    else if (contentType == "application/octet-stream")
                    {
                        parameters[pNumber] = request.Body;
                    }
                    else if (contentType == "application/x-www-urlform-encoded")
                    {
                        NameValueCollection nvc = new NameValueCollection();
                        using (var sr = new StreamReader(request.Body))
                        {
                            var ptext = sr.ReadToEnd();
                            var parms = ptext.Split('&');
                            foreach (var p in parms)
                            {
                                var parmData = p.Split('=');
                                nvc.Add(WebUtility.UrlDecode(parmData[0]), WebUtility.UrlDecode(parmData[1]));
                            }
                        }
                        parameters[pNumber] = nvc;
                    }
                    else if (contentType != null)
                        throw new InvalidOperationException("Invalid request format");
                }
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
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