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
using System.Diagnostics.CodeAnalysis;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// Represents a MIME encoding that a particular service understands for consumption
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true)]
    [ExcludeFromCodeCoverage]
    public class ServiceConsumesAttribute : ServiceContentTypeAttribute
    {
        /// <summary>
        /// Service consumption attribute
        /// </summary>
        public ServiceConsumesAttribute(String mimeType) : base(mimeType)
        {
        }
    }

    /// <summary>
    /// Represents a MIME encoding that a particular service produces in the response body if present.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true)]
    [ExcludeFromCodeCoverage]
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
    [ExcludeFromCodeCoverage]
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
