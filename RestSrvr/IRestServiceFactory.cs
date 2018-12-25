/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 *
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
 * User: justin
 * Date: 2018-12-1
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr
{
    /// <summary>
    /// Represents a class which can be used to create new services
    /// </summary>
    public interface IRestServiceFactory
    {

        /// <summary>
        /// Create the specified REST service (from configuration)
        /// </summary>
        /// <param name="serviceType">The service behavior</param>
        /// <returns>The created rest service</returns>
        RestService CreateService(Type serviceType);

        /// <summary>
        /// Get service capabilities
        /// </summary>
        int GetServiceCapabilities(RestService service);
    }
}
