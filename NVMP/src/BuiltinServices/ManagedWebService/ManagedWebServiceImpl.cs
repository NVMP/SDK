using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NVMP.BuiltinServices.ManagedWebService
{
    internal class ManagedWebServiceImpl : IManagedWebService
    {
        /// <summary>
        /// Port in use
        /// </summary>
        public int ListeningPort;

        internal HttpListener InternalServer;
        internal Thread InternalThread;

        internal bool Running;

        internal string ActiveURL;
        internal string InternalHostname;
        internal int InternalPortOverride;

        internal static int MaxConcurrentConnections = 16;

        // Dictionaries of methods -> resolvers -> response handlers so that each request can filter down into the appropriate bucket
        internal Dictionary<IManagedWebService.Method, Dictionary<
            string, Func<HttpListenerRequest, HttpListenerResponse, Task>
            >> ResponseHandlers = new Dictionary<IManagedWebService.Method, Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, Task>>>();

        internal Dictionary<IManagedWebService.Method, Dictionary<
            string, Func<HttpListenerRequest, HttpListenerResponse, Task>
            >> RootHandlers = new Dictionary<IManagedWebService.Method, Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, Task>>>();

        internal string CORSUri = "*";

        internal IManagedWebService.Method HttpMethodToEnumMethod(string methodStr)
        {
            if (Enum.TryParse(methodStr, ignoreCase: true, out IManagedWebService.Method result))
            {
                return result;
            }

            throw new Exception("Invalid HTTP Method");
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ManagedWebServiceImpl(string hostname, int portOverride)
        {
            if (portOverride != 0)
            {
                InternalPortOverride = portOverride;
            }

            InternalHostname = hostname;
            ActiveURL = $"{InternalHostname}:{InternalPortOverride}";
        }

        public void SetAccessControlOrigin(string remoteServerUri)
        {
            CORSUri = remoteServerUri;
        }

        public string FullURL
        {
            get
            {
                return ActiveURL;
            }
        }

        private void AttemptServerCreation()
        {
            Debugging.Warn("Attempting Discord listener server creation on port " + InternalPortOverride);

            InternalServer = new HttpListener()
            {
                Prefixes = { $"{ActiveURL}/" }
            };
            InternalServer.Start();
        }

        public void AddPathResolver(string url, Func<HttpListenerRequest, HttpListenerResponse, Task> fn, IManagedWebService.Method method)
        {
            ResponseHandlers[method].Add(url, fn);
        }

        public void AddRootResolver(string root, Func<HttpListenerRequest, HttpListenerResponse, Task> fn, IManagedWebService.Method method)
        {
            RootHandlers[method].Add(root, fn);
        }

        private async Task Receive()
        {
            var requests = new HashSet<Task>();
            for (var i = 0; i < MaxConcurrentConnections; ++i)
            {
                requests.Add(InternalServer.GetContextAsync());
            }

            while (Running)
            {
                var t = await Task.WhenAny(requests);
                requests.Remove(t);

                if (t is Task<HttpListenerContext>)
                {
                    var context = (t as Task<HttpListenerContext>).Result;
                    requests.Add(ProcessRequestAsync(context));
                    requests.Add(InternalServer.GetContextAsync());
                }

            }
        }

        internal async Task ProcessRequestAsync(HttpListenerContext ctx)
        {

            try
            {
                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                if (req.HttpMethod == "OPTIONS")
                {
                    resp.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
                    resp.AddHeader("Access-Control-Allow-Methods", "GET, POST");
                    resp.AddHeader("Access-Control-Allow-Credentials", "true");
                    resp.AddHeader("Access-Control-Max-Age", "1728000");
                }

                if (req.HttpMethod == "OPTIONS" || req.HttpMethod == "GET")
                {
                    if (CORSUri != null)
                    {
                        resp.AppendHeader("Access-Control-Allow-Origin", CORSUri);
                        resp.AppendHeader("Access-Control-Allow-Credentials", "true");
                    }
                }

                try
                {
                    int EndOrNextDivPos = req.Url.AbsolutePath.Substring(1).IndexOf("/");
                    if (EndOrNextDivPos == -1)
                    {
                        EndOrNextDivPos = req.Url.AbsolutePath.Length - 1;
                    }

                    IManagedWebService.Method method = HttpMethodToEnumMethod(req.HttpMethod);
                    var methodRootHandler = RootHandlers[method];
                    var methodResponseHandler = ResponseHandlers[method];

                    string handlerName = req.Url.AbsolutePath.Substring(1);
                    string rootName = req.Url.AbsolutePath.Substring(1, EndOrNextDivPos);

                    if (req.Url.AbsolutePath.Length >= 1 && methodRootHandler.ContainsKey(rootName))
                    {
                        await methodRootHandler[rootName](req, resp);
                    }
                    else if (req.Url.AbsolutePath.Length >= 1 && methodResponseHandler.ContainsKey(handlerName))
                    {
                        await methodResponseHandler[handlerName](req, resp);
                    }
                    else
                    {
                        // Write the response info
                        byte[] data = Encoding.UTF8.GetBytes("<!doctype html><html><head><title>NV:MP</title></head><body><div><b>NV:MP Authentication</b></div>Something bad happened to this request.</body></html>");
                        resp.ContentType = "text/html";
                        resp.ContentEncoding = Encoding.UTF8;
                        resp.ContentLength64 = data.LongLength;

                        // Write out to the response stream (asynchronously), then close it
                        resp.OutputStream.Write(data, 0, data.Length);
                        resp.Close();

                        Debugging.Error($"Failed to handle '{handlerName}' {rootName}");
                    }
                }
                catch (Exception ex)
                {
                    Debugging.Error(ex.Message);
                    resp.Close();
                }
            } catch (Exception ex)
            {
                Debugging.Error(ex.Message);
            }
        }

        public void Initialize()
        {
            int attempts = 5;

            ResponseHandlers.Clear();
            RootHandlers.Clear();
            CORSUri = "*";

            // populate method buckets
            foreach (IManagedWebService.Method method in (IManagedWebService.Method[])Enum.GetValues(typeof(IManagedWebService.Method)))
            {
                ResponseHandlers[method] = new Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, Task>>();
                RootHandlers[method] = new Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, Task>>();
            }

            while (attempts > 0)
            {
                try
                {
                    AttemptServerCreation();

                    Running = true;
                    InternalThread = new Thread(() => Receive().GetAwaiter().GetResult());
                    InternalThread.Start();

                    ListeningPort = InternalPortOverride;
                    Debugging.Write("WebService initialized");
                    break;
                }
                catch (Exception ex)
                {
                    Debugging.Error(ex.Message);
                }

                attempts--;
            }

            if (attempts == 0)
            {
                throw new Exception("Could not start up the Discord Tcp Listener! You may need to grant this user with hosting privillages to start up a HTTPListener context. See https://stackoverflow.com/a/1987758 and specify ");
            }
        }

        public void Shutdown()
        {
            Running = false;

            Debugging.Write("Shutting down HTTPListener...");
            if (InternalServer != null)
            {
                InternalServer.Stop();
            }

            Debugging.Write("Synchronising web thread...");
            if (InternalThread != null)
            {
                InternalThread.Abort();
                InternalThread.Join();
                InternalThread = null;
            }
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

}
