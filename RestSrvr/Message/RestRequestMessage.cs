/*
 * Copyright (C) 2021 - 2026, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-6-21
 */
using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace RestSrvr.Message
{
    /// <summary>
    /// Represents a restful message
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RestRequestMessage : IDisposable
    {
        // Headers of the message
        private HttpListenerRequest m_request;

        /// <summary>
        /// The message contents
        /// </summary>
        private Stream m_messageContents;

        /// <summary>
        /// Body of the message
        /// </summary>
        public Stream Body
        {
            get { return this.m_messageContents; }
            set { this.m_messageContents = value; }
        }

        /// <summary>
        /// Gets the headers of this message
        /// </summary>
        public NameValueCollection Headers => this.m_request.Headers;

        /// <summary>
        /// Gets the original request information
        /// </summary>
        public CookieCollection Cookies => this.m_request.Cookies;

        /// <summary>
        /// Gets the URL of the message
        /// </summary>
        public Uri Url => this.m_request.Url;

        /// <summary>
        /// Gets the content type
        /// </summary>
        public String ContentType => this.m_request.ContentType;

        /// <summary>
        /// Gets the HTTP method
        /// </summary>
        public String Method => this.m_request.HttpMethod;

        /// <summary>
        /// Client certificate
        /// </summary>
        public X509Certificate2 ClientCertificate => this.m_request.GetClientCertificate();

        /// <summary>
        /// Client certificate error code (if present)
        /// </summary>
        public int ClientCertificateError => this.m_request.ClientCertificateError;

        /// <summary>
        /// True if the connection is secure
        /// </summary>
        public bool IsSecure => this.m_request.IsSecureConnection;

        /// <summary>
        /// Gets the Operational path
        /// </summary>
        public string OperationPath { get; internal set; }

        /// <summary>
        /// Gets the User agent
        /// </summary>
        public string UserAgent => this.m_request.UserAgent;

        /// <summary>
        /// Initialize the rest message with the contents of the request message
        /// </summary>
        public RestRequestMessage(HttpListenerRequest request)
        {
            if (request.HasEntityBody)
            {
                this.m_messageContents = new MemoryStream();
                request.InputStream.CopyTo(this.m_messageContents);
                this.m_messageContents.Seek(0, SeekOrigin.Begin);
            }


            this.m_request = request;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.m_messageContents?.Dispose();
        }

    }
}
