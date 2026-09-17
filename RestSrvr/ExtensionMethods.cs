using RestSrvr.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace RestSrvr
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class ExtensionMethods
    {

        /// <summary>
        /// Get most preferred content type header that the client supports
        /// </summary>
        /// <param name="me">The http request</param>
        /// <param name="supportedMediaTypes">The supported media types</param>
        /// <returns></returns>
        public static ContentType GetMostPreferredResponseContentType(this HttpListenerRequest me, params string[] supportedMediaTypes) => 
            me.AcceptTypes?
                .SelectMany(o=>o.Split(',')) // HACK: For Mono
                .Where(o => !String.IsNullOrEmpty(o)) // HACK: For Mono
                .Select(x => new System.Net.Mime.ContentType(x.Trim()))
                .Where(x=> supportedMediaTypes?.Length == 0 || supportedMediaTypes.Contains(x.MediaType))
                .OrderByDescending(x => float.TryParse(x.Parameters["q"], out var q) ? q : 1.0f)
                .FirstOrDefault() ??
                (!String.IsNullOrEmpty(me.ContentType) ?
                    new System.Net.Mime.ContentType(me.ContentType) :
                    null);   
    }
}
