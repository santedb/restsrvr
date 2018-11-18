using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr
{
    /// <summary>
    /// Rest service constants
    /// </summary>
    internal static class TraceSources
    {

        /// <summary>
        /// Main trace source name
        /// </summary>
        public const string TraceSourceName = "RestSrvr";

        /// <summary>
        /// Dispatch events
        /// </summary>
        public const string DispatchTraceSourceName = TraceSourceName + ".Dispatch";

        /// <summary>
        /// Description events
        /// </summary>
        public const string DescriptionTraceSourceName = TraceSourceName + ".Description";

        /// <summary>
        /// Http events
        /// </summary>
        public const string HttpTraceSourceName = TraceSourceName + ".Http";

        public const string MessageTraceSourceName = TraceSourceName + ".Messaging";

        public const string ThreadingTraceSourceName = TraceSourceName + ".Threading";
    }
}
