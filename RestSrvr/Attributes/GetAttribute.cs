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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr.Attributes
{
    /// <summary>
    /// Identifies the GET operation can be used to invoke a method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : RestInvokeAttribute
    {
        /// <summary>
        /// Creates a GET invokation attribute on the method
        /// </summary>
        /// <param name="urlTemplate"></param>
        public GetAttribute(String urlTemplate) : base("GET", urlTemplate)
        {

        }
    }
}
