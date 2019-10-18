# RestSrvr
RestSrvr is a lightweight replacement for the SanteDB's WCF layer. RestSrvr is used
by SanteDB services whenever they are run on Linux or environments where WCF 
service implementation is lacking or does not function (like Mono or Xamarin). It also allows for easy deployment and runtime
operation as no external web-services are required to function (like Apache, NGINX, IIS, etc.) however should hide behind those
services for production use (for TLS, certificate management, etc.).

The RestSrvr library is primarily designed to also allow easy modification of and inspection of messages, this allows for rapid and easy implementation of :

* Custom operation policies to apply permissions consistently across an entire service
* Custom serialization and message formatters to support custom wire-level serialization
* Custom message-logging and inspection

## Limitations
RestSrvr currently only supports basic HTTP transmission and routing of RESTful messages.

* Only HTTP is supported. If you're using SanteDB or another service on a server which will have to allow external access please use NginX to secure the service for the time being.
* Implementation of authentication must be performed by the hosting code via an implementation of IServicePolicy
* Only basic routes are supported
   * Using static routes: 
   ```
    [Get("/Patient")]
    public List<Patient> GetPatients();
   ```
   * Using parameters: 
   ```
    [Get("/Patient/{id}")]
    public Patient GetPatients(id);
   ```
   * Using catch-all: 
   ```
    [Get("/html/*")]
    public Stream ServePage();
   ```
* The body of messages (for POST, PUT, etc.) must be the last parameter of the method being invoked.

## Design
The design principles of RestSrvr are very similar to WCF's WebHttpBinding. In short:

* **Service Contract**: The contract is an interface which dictates complaint behavior implementations
* **Service Behavior**: A behavior is an implementation of a contract
* **Service**: An instantiation of a contract through a behavior.
* **Endpoint**: An endpoint is an HTTP (or other) location where the service is accessed by callers.
* **Behavior**: A behavior is a modifier on either an endpoint or an operation which impacts how each component operates at runtime
* **Dispatcher**: A dispatcher is responsible for selecting the appropriate channel, endpoint and operation to execute in the execution context.

## Example Use

RestSrvr is nearly identical to WCF implementations:

```
using RestSrvr;
using RestSrvr.Attributes;
using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Text;

/// <summary>
/// RestSrvr will format requests in either JSON or XML depending on the Accept header passed to it
/// </summary>
[XmlRoot("Patient", Namespace = "http://test.com")]
[JsonObject]
public class Patient {

    [XmlElement("id"), JsonProperty("id")]
    public String Id { get; set; }

}

/// <summary>
/// This is the contract which is decorated to inform routing of the service
/// </summary>
[RestContract(Name = "MyContract")]
public interface IMyContract {
    
    [Get("/index.html")]
    Stream Index();

    [Get("/Patient/{id}")]
    Patient GetPatient(String id);

}

/// <summary>
/// The behavior implements one or more contracts
/// </summary>
[RestBehavior(Name = "DefaultMyBehavior")]
public class MyBehavior : IMyContract {

    public Stream Index() {
        RestOperationContext.Current.OutgoingResponse.ContentType = "text/html";
        return new MemoryStream(Encoding.UTF.GetBytes("<html><body>Hello World!</body></html>"));
    }

    public Patient GetPatient(String id) {
        return new Patient() { Id = id }; // Echo back the id
    }
}

/// <summary>
/// The main program adds an endpoint and then runs the service
/// </summary>
class Program {

    static void Main(string[] args)
    {
        var myservce = new RestService(typeof(MyBehavior));
        myservice.AddServiceEndpoint(new Uri("http://0.0.0.0:8080/myservice"), typeof(IMyService), new RestHttpBinding());
        myservice.Start();
        Console.ReadKey();
    }
}

```

You may get an access denied exception on Windows. In overcome this:

* Use a user port (usually above port 10000)
* Reserve the port using ```netsh http add urlacl url="http://+:8080/myservice" user=username```
