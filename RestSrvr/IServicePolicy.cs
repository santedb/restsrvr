using RestSrvr.Message;

namespace RestSrvr
{
    /// <summary>
    /// Represents a policy on the specified service. Policies can be used to authenticate
    /// users, etc.
    /// </summary>
    public interface IServicePolicy
    {

        /// <summary>
        /// Apply the policy to the request
        /// </summary>
        void Apply(RestRequestMessage request);
    }
}