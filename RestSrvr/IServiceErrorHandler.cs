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
using RestSrvr.Message;
using System;

namespace RestSrvr
{
    /// <summary>
    /// Represents a class which can handle service faults
    /// </summary>
    public interface IServiceErrorHandler
    {

        /// <summary>
        /// Returns true if the error handler can handle the error
        /// </summary>
        bool HandleError(Exception error);

        /// <summary>
        /// Provides a fault message back to the pipeline
        /// </summary>
        bool ProvideFault(Exception error, RestResponseMessage response);
    }
}