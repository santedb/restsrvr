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
            rservice.Start();
            Console.ReadKey();

        }
    }
}
