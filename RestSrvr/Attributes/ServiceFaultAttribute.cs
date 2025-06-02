/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
    /// Represents a service fault attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true)]
    [ExcludeFromCodeCoverage]
    public class ServiceFaultAttribute : Attribute
    {

        /// <summary>
        /// Creates a new service fault
        /// </summary>
        public ServiceFaultAttribute(int status, Type faultType, String condition)
        {
            this.StatusCode = status;
            this.FaultType = faultType;
            this.Condition = condition;
        }

        /// <summary>
        /// Gets or sets the status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or set the type of fault
        /// </summary>
        public Type FaultType { get; set; }

        /// <summary>
        /// Gets or sets the condition of the fault
        /// </summary>
        public String Condition { get; set; }

    }
}
