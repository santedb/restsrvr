using RestSrvr.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{

    [RestContract(Name = "MySampleRESTContract")]
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
