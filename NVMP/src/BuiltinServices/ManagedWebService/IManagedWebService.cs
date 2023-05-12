using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NVMP.BuiltinServices.ManagedWebService
{
    /// <summary>
    /// Provides a built in web service that can have multiple path resolvers under the same port.
    /// This is used by authentication modules that require a callback to the server, but can be used by other plugins provided
    /// the path resolver is unique.
    /// </summary>
    public interface IManagedWebService
    {
        public enum Method
        {
            GET,
            HEAD,
            POST,
            PUT,
            DELETE,
            CONNECT,
            OPTIONS,
            TRACE,
            PATCH
        }

        public enum ExecutionType
        {
            Sync,
            Async
        }

        /// <summary>
        /// Adds a new path resolver to the web service.
        /// </summary>
        /// <param name="url">the path after the hostname to bind to, eg. "server" will be http://localhost:!234/server/</param>
        /// <param name="fn">the lambda to call when the service is hit</param>
        /// <param name="method"></param>
        /// <param name="executionType"></param>
        public void AddPathResolver(string url, Func<HttpListenerRequest, HttpListenerResponse, Task> fn, Method method = Method.GET, ExecutionType executionType = ExecutionType.Sync);

        /// <summary>
        /// Adds a new root path resolver to the web service.
        /// </summary>
        /// <param name="root">the root path after the hostname to bind to, eg. "server" will be http://localhost:!234/server/</param>
        /// <param name="fn">the lambda to call when the service is hit</param>
        /// <param name="method"></param>
        /// <param name="executionType"></param>
        public void AddRootResolver(string root, Func<HttpListenerRequest, HttpListenerResponse, Task> fn, Method method = Method.GET, ExecutionType executionType = ExecutionType.Sync);

        /// <summary>
        /// Sets the specified URI to the CORS origin.
        /// </summary>
        /// <param name="remoteServerUri"></param>
        public void SetAccessControlOrigin(string remoteServerUri);

        /// <summary>
        /// Initializes the listener
        /// </summary>
        public void Initialize();

        /// <summary>
        /// Shuts down the listener
        /// </summary>
        public void Shutdown();

        /// <summary>
        /// Returns the full base URL in use currently
        /// </summary>
        public string FullURL { get; }
    }
}
