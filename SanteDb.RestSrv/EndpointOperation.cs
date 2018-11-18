using SanteDB.RestSrv.Description;

namespace SanteDB.RestSrv
{
    public class EndpointOperation
    {
        // The desccription of the operation
        private OperationDescription m_description;
        private OperationDispatcher m_dispatcher;

        /// <summary>
        /// Gets the dispatcher
        /// </summary>
        internal OperationDispatcher Dispatcher => this.m_dispatcher;

        /// <summary>
        /// Gets the description of this operation
        /// </summary>
        public OperationDescription Description => this.m_description;

        /// <summary>
        /// Creates a new instance of the operation with the specified description
        /// </summary>
        /// <param name="operationDescription"></param>
        public EndpointOperation(OperationDescription operationDescription)
        {
            this.m_description = operationDescription;
            this.m_dispatcher = new OperationDispatcher(this);
        }
    }
}