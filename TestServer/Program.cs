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
