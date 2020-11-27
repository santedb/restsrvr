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
using RestSrvr.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{

    [ServiceContract(Name = "MySampleRESTContract")]
    interface ISampleContract
    {
        [Get("/a")]
        void GetA();

        [Get("/b")]
        String GetB();

        [Get("/add/{a}/{b}")]
        Int32 Add(Int32 a, Int32 b);

        [Get("/somewhere/*")]
        Stream GetStream();

        [Get("/{objectType}")]
        ServiceObjectSample GetC(String objectType);
        
        [Get("/{objectType}/{id}")]
        ServiceObjectSample GetC(String objectType, String id);

        [Post("/{objectType}")]
        ServiceObjectSample CreateC(String objectType, ServiceObjectSample data);

        [Put("/{objectType}/{name}")]
        ServiceObjectSample UpdateC(String objectType, String name, ServiceObjectSample data);

        [Delete("/{objectType}/{name}")]
        void DeleteC(String objectType, String name);

    }
}
