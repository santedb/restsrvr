using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr
{
    /// <summary>
    /// Represents a class which can be used to create new services
    /// </summary>
    public interface IRestServiceFactory
    {

        /// <summary>
        /// Create the specified REST service (from configuration)
        /// </summary>
        /// <param name="serviceType">The service behavior</param>
        /// <returns>The created rest service</returns>
        RestService CreateService(Type serviceType);

        /// <summary>
        /// Get service capabilities
        /// </summary>
        int GetServiceCapabilities(RestService service);
    }
}
