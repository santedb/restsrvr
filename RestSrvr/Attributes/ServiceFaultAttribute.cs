/*
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2019-11-27
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// Represents a service fault attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true)]
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
