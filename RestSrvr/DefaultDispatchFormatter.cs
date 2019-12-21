/*
 * Copyright 2015-2019 Mohawk College of Applied Arts and Technology
 * Copyright 2019-2019 SanteSuite Contributors (See NOTICE)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: Justin Fyfe
 * Date: 2019-8-8
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RestSrvr.Description;
using RestSrvr.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
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
                    else if (contentType == "application/x-www-form-urlencoded")
                    {
                        NameValueCollection nvc = new NameValueCollection();
                        using (var sr = new StreamReader(request.Body))
                        {
                            var ptext = sr.ReadToEnd();
                            var parms = ptext.Split('&');
                            foreach (var p in parms)
                            {
                                var parmData = p.Split('=');
                                parmData[1] += new string('=', parmData.Length - 2);
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
                if(responseMessage.StatusCode == 200)
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
            else if (RestOperationContext.Current.IncomingRequest.Headers["Accept"]?.StartsWith("application/json") == true ||
                RestOperationContext.Current.IncomingRequest.Url.AbsolutePath.EndsWith(".json"))
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
            else if(typeof(ExpandoObject).IsAssignableFrom(result.GetType()) || 
                typeof(IEnumerable<ExpandoObject>).IsAssignableFrom(result.GetType()))
            {
                // Custom serialization for XML of a dynamic
                if (result.GetType() == typeof(ExpandoObject))
                    result = new List<ExpandoObject>() { result as ExpandoObject };
                var ms = new MemoryStream();
                using (var xw = XmlWriter.Create(ms, new XmlWriterSettings() { CloseOutput = false })) // Write dynamic
                {
                    xw.WriteStartElement("ArrayOfDynamic", "http://tempuri.org");
                    // Iterate through objects
                    foreach(var itm in result as IEnumerable)
                    {
                        xw.WriteStartElement("item", "http://tempuri.org");
                        foreach(var prop in itm as ExpandoObject)
                        {
                            xw.WriteStartElement(prop.Key);
                            if(prop.Value is Guid)
                                xw.WriteValue(prop.Value.ToString());
                            else if(prop.Value != null)
                                xw.WriteValue(prop.Value);
                            xw.WriteEndElement();
                        }
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                }

                ms.Seek(0, SeekOrigin.Begin);
                responseMessage.ContentType = responseMessage.ContentType ?? "application/xml";
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