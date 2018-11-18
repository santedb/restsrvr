using SanteDB.RestSrv.Message;
using System;

namespace SanteDB.RestSrv
{
    /// <summary>
    /// Represents a class which can handle service faults
    /// </summary>
    public interface IServiceErrorHandler
    {

        /// <summary>
        /// Returns true if the error handler can handle the error
        /// </summary>
        bool HandleError(Exception error);

        /// <summary>
        /// Provides a fault message back to the pipeline
        /// </summary>
        bool ProvideFault(Exception error, RestResponseMessage response);
    }
}