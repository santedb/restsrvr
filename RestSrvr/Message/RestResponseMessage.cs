using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

            if(!this.m_content.CanSeek)
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
