/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-5-19
 */
using System;
using System.Diagnostics.CodeAnalysis;

namespace RestSrvr.Attributes
{

    /// <summary>
    /// Service instance mode
    /// </summary>
    public enum ServiceInstanceMode
    {
        /// <summary>
        /// Only one instance is ever created
        /// </summary>
        Singleton,
        /// <summary>
        /// New instance is made per call
        /// </summary>
        PerCall

    }

    /// <summary>
    /// Represents a rest service behavior
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [ExcludeFromCodeCoverage]
    public class ServiceBehaviorAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the behavior
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the instance mode
        /// </summary>
        public ServiceInstanceMode InstanceMode { get; set; }
    }
}
