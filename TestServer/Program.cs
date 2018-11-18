using SanteDB.RestSrv;
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

            var httpServer = new RestService(typeof(SampleBehavior));
        }
    }
}
