using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TestServer
{
    /// <summary>
    /// Service object sample
    /// </summary>
    [XmlRoot("sample", Namespace = "http://test.com")]
    [XmlType("sample", Namespace = "http://test.com")]
    [JsonObject]
    public class ServiceObjectSample
    {
        /// <summary>
        /// Name sample
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Value sample
        /// </summary>
        [XmlText(), JsonProperty("value")]
        public Int32 Value { get; set; }
    }
}
