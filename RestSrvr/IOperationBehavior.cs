﻿/*
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
 * Date: 2018-11-19
 */
namespace RestSrvr
{
    /// <summary>
    /// Represents a behavior on an endpoint
    /// </summary>
    public interface IOperationBehavior
    {

        /// <summary>
        /// Allows the behavior to modify the dispatcher or operation as needed to implement the 
        /// behavior
        /// </summary>
        /// <param name="endpoint">The endpoint to which the behavior is to be applied</param>
        /// <param name="dispatcher">The dispatcher that the endpoint will be using</param>
        void ApplyOperationBehavior(EndpointOperation operation, OperationDispatcher dispatcher);
    }
}