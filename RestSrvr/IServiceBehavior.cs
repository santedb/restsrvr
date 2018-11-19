namespace RestSrvr
{
    /// <summary>
    /// Represents a behavior on an service
    /// </summary>
    public interface IServiceBehavior
    {

        /// <summary>
        /// Allows the behavior to modify the dispatcher or service as needed to implement the 
        /// behavior
        /// </summary>
        /// <param name="service">The service to which the behavior is to be applied</param>
        /// <param name="dispatcher">The dispatcher that the service will be using</param>
        void ApplyServiceBehavior(RestService service, ServiceDispatcher dispatcher);
    }
}