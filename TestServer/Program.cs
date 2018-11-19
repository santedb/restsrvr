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
            rservice.AddServiceEndpoint(new Uri("http://0.0.0.0:8080/fhir"), typeof(ISampleContract), new RestHttpBinding());
            foreach (var spe in rservice.Endpoints)
                spe.AddEndpointBehavior(new EndpointConsoleLogBehavior());

            var oservice = new RestService(typeof(SampleBehavior));
            oservice.AddServiceEndpoint(new Uri("http://0.0.0.0:8080/auth"), typeof(ISampleContract), new RestHttpBinding());
            oservice.AddServiceEndpoint(new Uri("http://0.0.0.0:8080/hdsi"), typeof(ISampleContract), new RestHttpBinding());
            rservice.Start();
            oservice.Start();
            Console.ReadKey();

        }
    }
}
