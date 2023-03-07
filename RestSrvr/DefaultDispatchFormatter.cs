/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2022-5-30
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSrvr.Attributes;
using RestSrvr.Message;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RestSrvr
{
    /// <summary>
    /// Default dispatch formatter
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class DefaultDispatchFormatter : IDispatchMessageFormatter
    {
        // Serializers
        private ConcurrentDictionary<Type, XmlSerializer> m_serializers = new ConcurrentDictionary<Type, XmlSerializer>();

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
                string contentTypeHeader = httpRequest.Headers["Content-Type"];
                ContentType contentType = null;
                if (!String.IsNullOrEmpty(contentTypeHeader))
                {
                    contentType = new ContentType(contentTypeHeader);
                }
                var methodparameters = operation.Description.InvokeMethod.GetParameters();
                for (int pNumber = 0; pNumber < parameters.Length; pNumber++)
                {
                    var parm = methodparameters[pNumber];

                    // TODO: Look for MessageFormatAttribute for override

                    // Simple parameter
                    if (parameters[pNumber] != null)
                    {
                        continue; // dispatcher already populated
                    }
                    else
                    {
                        switch (contentType?.MediaType)
                        {
                            case "application/xml":
                                if (!this.m_serializers.TryGetValue(parm.ParameterType, out XmlSerializer serializer))
                                {
                                    serializer = new XmlSerializer(parm.ParameterType);
                                    this.m_serializers.TryAdd(parm.ParameterType, serializer);
                                }
                                var requestObject = serializer.Deserialize(request.Body);
                                parameters[pNumber] = requestObject;
                                break;
                            case "application/json":
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
                                break;
                            case "application/octet-stream":
                                parameters[pNumber] = request.Body;
                                break;
                            case "application/x-www-form-urlencoded":
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
                                break;
                            case null:
                                parameters[pNumber] = null;
                                break;
                            default:
                                throw new InvalidOperationException("Invalid request format");

                        }
                    }
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
            var acceptHeader = RestOperationContext.Current.IncomingRequest.Headers["Accept"];
            ContentType contentType = null;
            if (!String.IsNullOrEmpty(acceptHeader))
            {
                contentType = acceptHeader.Split(',').Select(o => new ContentType(o)).First();
            }

            // By default unless Accept is application/json , we always prefer application/xml
            if (result == null)
            {
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    responseMessage.StatusCode = HttpStatusCode.NoContent;
                }
            }
            else if (result is Stream)
            {
                responseMessage.ContentType = responseMessage.ContentType ?? "application/octet-stream";
                responseMessage.Body = result as Stream;
            }
            else if (result.GetType().IsPrimitive || result is string ||
                result is Guid)
            {
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(result.ToString()));
                responseMessage.ContentType = responseMessage.ContentType ?? "text/plain";
                responseMessage.Body = ms;
            }
            else if (responseMessage.Format == MessageFormatType.Json ||
                contentType?.MediaType == "application/json" ||
                RestOperationContext.Current.IncomingRequest.Url.AbsolutePath.EndsWith(".json"))
            {
                // Prepare the serializer
                JsonSerializer jsz = new JsonSerializer();
                var ms = new MemoryStream();
                using (var tms = new MemoryStream())
                using (StreamWriter sw = new StreamWriter(tms, new UTF8Encoding(false)))
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
            else if (responseMessage.Format == MessageFormatType.Xml ||
                contentType?.MediaType == "application/xml" ||
                RestOperationContext.Current.IncomingRequest.Url.AbsolutePath.EndsWith(".xml"))
            {
                if (typeof(ExpandoObject).IsAssignableFrom(result.GetType()) ||
                    typeof(IEnumerable<ExpandoObject>).IsAssignableFrom(result.GetType()))
                {
                    // Custom serialization for XML of a dynamic
                    if (result.GetType() == typeof(ExpandoObject))
                    {
                        result = new List<ExpandoObject>() { result as ExpandoObject };
                    }

                    var ms = new MemoryStream();
                    using (var xw = XmlWriter.Create(ms, new XmlWriterSettings() { CloseOutput = false })) // Write dynamic
                    {
                        xw.WriteStartElement("ArrayOfDynamic", "http://tempuri.org");
                        // Iterate through objects
                        foreach (var itm in result as IEnumerable)
                        {
                            xw.WriteStartElement("item", "http://tempuri.org");
                            foreach (var prop in itm as ExpandoObject)
                            {
                                xw.WriteStartElement(prop.Key);
                                if (prop.Value is Guid)
                                {
                                    xw.WriteValue(prop.Value.ToString());
                                }
                                else if (prop.Value != null)
                                {
                                    xw.WriteValue(prop.Value);
                                }

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
                    if (!this.m_serializers.TryGetValue(result.GetType(), out XmlSerializer serializer))
                    {
                        serializer = new XmlSerializer(result.GetType());
                        this.m_serializers.TryAdd(result.GetType(), serializer);
                    }
                    var ms = new MemoryStream();
                    serializer.Serialize(ms, result);
                    ms.Seek(0, SeekOrigin.Begin);
                    responseMessage.ContentType = responseMessage.ContentType ?? "application/xml";
                    responseMessage.Body = ms;
                }
            }
            else
            {
                throw new ArgumentException($"Unsupported response format requested");
            }
        }
    }
}