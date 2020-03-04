/*
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2019-11-27
 */
using RestSrvr.Attributes;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace RestSrvr.Message
{
    /// <summary>
    /// Represents a response message
    /// </summary>
    public class RestResponseMessage : IDisposable
    {
        // Contents
        private Stream m_content;

        // Response context
        private HttpListenerResponse m_response;
        
        /// <summary>
        /// Gets the headers on the response
        /// </summary>
        public NameValueCollection Headers => this.m_response.Headers;
        
        /// <summary>
        /// Gets or sets teh body to write
        /// </summary>
        public Stream Body { get => this.m_content; set => this.m_content = value; }

        /// <summary>
        /// Gets or sets the format of this message response type
        /// </summary>
        public MessageFormatType Format { get; internal set; }


        /// <summary>
        /// Gets or sets the content type
        /// </summary>
        public String ContentType
        {
            get { return this.m_response.ContentType; }
            set { this.m_response.ContentType = value; }
        }

        /// <summary>
        /// Gets or sets the response status code
        /// </summary>
        public int StatusCode
        {
            get => this.m_response.StatusCode;
            set => this.m_response.StatusCode = value;
        }

        /// <summary>
        /// Gets or sets the status description
        /// </summary>
        public String StatusDescription
        {
            get => this.m_response.StatusDescription;
            set => this.m_response.StatusDescription = value;
        }

        /// <summary>
        /// Gets the cookies to set on the response
        /// </summary>
        public CookieCollection Cookies => this.m_response.Cookies;

        /// <summary>
        /// Creates a new rest response message instance
        /// </summary>
        public RestResponseMessage(HttpListenerResponse response)
        {
            this.m_response = response;
        }
        
        /// <summary>
        /// Writes the contents to the response
        /// </summary>
        internal void FlushResponseStream()
        {
            this.m_response.ContentEncoding = Encoding.UTF8;

            if(!this.m_content?.CanSeek == true)
            {
                var ms = new MemoryStream();

                this.m_content?.CopyTo(ms);
                this.m_content?.Dispose();
                this.m_content = ms;
                ms.Seek(0, SeekOrigin.Begin);
            }
            this.m_response.ContentLength64 = this.m_content?.Length ?? 0;

            this.m_content?.CopyTo(this.m_response.OutputStream);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~RestResponseMessage()
        {
            this.Dispose();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.m_content?.Dispose();
        }
    }
}
