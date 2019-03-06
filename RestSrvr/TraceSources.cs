﻿/*
 * Copyright 2015-2019 Mohawk College of Applied Arts and Technology
 * Copyright 2019-2019 SanteSuite Contributors (See NOTICE)
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
 * User: justi
 * Date: 2019-1-12
 */
namespace RestSrvr
{
    /// <summary>
    /// Rest service constants
    /// </summary>
    internal static class TraceSources
    {

        /// <summary>
        /// Main trace source name
        /// </summary>
        public const string TraceSourceName = "RestSrvr";

        /// <summary>
        /// Dispatch events
        /// </summary>
        public const string DispatchTraceSourceName = TraceSourceName + ".Dispatch";

        /// <summary>
        /// Description events
        /// </summary>
        public const string DescriptionTraceSourceName = TraceSourceName + ".Description";

        /// <summary>
        /// Http events
        /// </summary>
        public const string HttpTraceSourceName = TraceSourceName + ".Http";

        public const string MessageTraceSourceName = TraceSourceName + ".Messaging";

        public const string ThreadingTraceSourceName = TraceSourceName + ".Threading";
    }
}
