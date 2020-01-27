using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{

    /// <summary>
    /// Message format 
    /// </summary>
    public enum MessageFormatType
    {
        Unspecified = 0,
        Json = 1,
        Xml = 2
    }

    /// <summary>
    /// Message formatting attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
    public class MessageFormatAttribute : Attribute
    {

        /// <summary>
        /// Creates a new instance of the message format attribute
        /// </summary>
        public MessageFormatAttribute(MessageFormatType format)
        {
            this.MessageFormat = format;
        }

        /// <summary>
        /// Gets or sets the message format type
        /// </summary>
        public MessageFormatType MessageFormat { get; set; }

    }
}
