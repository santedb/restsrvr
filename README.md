# RestSrvr
RestSrvr is a lightweight replacement for the SanteDB's WCF layer. RestSrvr is used
by SanteDB services whenever they are run on Linux or environments where WCF 
service implementation is lacking or does not function (like Mono or Xamarin).

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