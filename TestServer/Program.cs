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
 * Date: 2020-5-1
 */
using RestSrvr;
using RestSrvr.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {

            var rservice = new RestService(typeof(SampleBehavior));
            rservice.AddServiceEndpoint(new Uri("http://127.0.0.1:9200/fhir"), typeof(ISampleContract), new RestHttpBinding());
            rservice.AddServiceEndpoint(new Uri("http://127.0.0.1:9200/"), typeof(ISampleContract), new RestHttpBinding());
            foreach (var spe in rservice.Endpoints)
                spe.AddEndpointBehavior(new EndpointConsoleLogBehavior());
            rservice.Start();
            //oservice.Start();
            Console.ReadKey();

        }
    }
}
