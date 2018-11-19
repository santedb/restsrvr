namespace RestSrvr
{
    /// <summary>
    /// Represents a behavior on an endpoint
    /// </summary>
    public interface IOperationBehavior
    {

        /// <summary>
        /// Allows the behavior to modify the dispatcher or operation as needed to implement the 
        /// behavior
        /// </summary>
        /// <param name="endpoint">The endpoint to which the behavior is to be applied</param>
        /// <param name="dispatcher">The dispatcher that the endpoint will be using</param>
        void ApplyOperationBehavior(EndpointOperation operation, OperationDispatcher dispatcher);
    }
}