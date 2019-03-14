using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// Represents a MIME encoding that a particular service understands for consumption
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true)]
    public class ServiceConsumesAttribute : ServiceContentTypeAttribute
    {
        /// <summary>
        /// Service consumption attribute
        /// </summary>
        public ServiceConsumesAttribute(String mimeType) : base (mimeType)
        {
        }
    }

    /// <summary>
    /// Represents a MIME encoding that a particular service understands for consumption
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true)]
    public class ServiceProducesAttribute : ServiceContentTypeAttribute
    {
        /// <summary>
        /// Service consumption attribute
        /// </summary>
        public ServiceProducesAttribute(String mimeType) : base(mimeType)
        {
        }
    }

    /// <summary>
    /// Base for content type attributes
    /// </summary>
    public abstract class ServiceContentTypeAttribute : Attribute
    {

        /// <summary>
        /// Creates a service content type
        /// </summary>
        public ServiceContentTypeAttribute(String mimeType)
        {
            this.MimeType = mimeType;
        }

        /// <summary>
        /// Gets or set sthe mime type
        /// </summary>
        public String MimeType { get; set; }

    }
}
