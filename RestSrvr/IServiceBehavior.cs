/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-5
 */
namespace RestSrvr
{
    /// <summary>
    /// Represents a behavior on an service
    /// </summary>
    public interface IServiceBehavior
    {

        /// <summary>
        /// Allows the behavior to modify the dispatcher or service as needed to implement the 
        /// behavior
        /// </summary>
        /// <param name="service">The service to which the behavior is to be applied</param>
        /// <param name="dispatcher">The dispatcher that the service will be using</param>
        void ApplyServiceBehavior(RestService service, ServiceDispatcher dispatcher);
    }
}