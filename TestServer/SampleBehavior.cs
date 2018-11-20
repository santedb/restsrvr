using RestSrvr;
using RestSrvr.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{

    [ServiceBehavior(Name = "SampleBehavior")]
    class SampleBehavior : ISampleContract
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public ServiceObjectSample CreateC(string objectType, ServiceObjectSample data)
        {
            throw new NotImplementedException();
        }

        public void DeleteC(string objectType, string name)
        {
            throw new NotImplementedException();
        }

        public void GetA()
        {
            throw new NotImplementedException();
        }

        public string GetB()
        {
            throw new NotImplementedException();
        }

        public ServiceObjectSample GetC(string objectType)
        {
            throw new NotImplementedException();
        }

        public ServiceObjectSample GetC(string objectType, string id)
        {
            return new ServiceObjectSample() { Name = objectType + "--ID--" + id, Value = (int)DateTime.Now.Ticks };
        }
        /// <summary>
        /// Get stream
        /// </summary>
        public Stream GetStream()
        {
            RestOperationContext.Current.OutgoingResponse.ContentType = "image/jpg";
            return typeof(SampleBehavior).Assembly.GetManifestResourceStream("TestServer.sample.jpg");
        }

        public ServiceObjectSample UpdateC(string objectType, string name, ServiceObjectSample data)
        {
            throw new NotImplementedException();
        }
    }
}
