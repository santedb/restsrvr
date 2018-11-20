using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Message
{
    /// <summary>
    /// Represents a restful message
    /// </summary>
    public class RestRequestMessage
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
        /// Gets the Operational path
        /// </summary>
        public string OperationPath { get; internal set; }

        /// <summary>
        /// Initialize the rest message with the contents of the request message
        /// </summary>
        public RestRequestMessage(HttpListenerRequest request)
        {
            if(request.HasEntityBody)
            {
                this.m_messageContents = new MemoryStream();
                request.InputStream.CopyTo(this.m_messageContents);
                this.m_messageContents.Seek(0, SeekOrigin.Begin);
            }

            this.m_request = request;
        }

    }
}
