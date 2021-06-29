using System;
using System.Collections.Generic;
using System.Text;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// A single query parameter which is exposed by the method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class UrlParameterAttribute : Attribute
    {

        /// <summary>
        /// Creates a new query parameter attribute
        /// </summary>
        public UrlParameterAttribute(String name , Type type, String description)
        {
            this.Name = name;
            this.Type = type;
            this.Description = description;
        }

        /// <summary>
        /// Gets the name of the query parameter
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Gets the type of data in the parmater
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets whether the parameter is required or not
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// True if the query parmaeter can be repeated
        /// </summary>
        public bool Multiple { get; set; }
    }
}
