/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 */
using System;
using System.Diagnostics.CodeAnalysis;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// A single query parameter which is exposed by the method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [ExcludeFromCodeCoverage]
    public class UrlParameterAttribute : Attribute
    {

        /// <summary>
        /// Creates a new query parameter attribute
        /// </summary>
        public UrlParameterAttribute(String name, Type type, String description)
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
