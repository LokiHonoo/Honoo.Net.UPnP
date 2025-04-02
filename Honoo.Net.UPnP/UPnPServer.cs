using System;
using System.Net;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP server. Need setup port open for firewall. Administrator privileges are required.
    /// </summary>
    public abstract class UPnPServer : IDisposable
    {
        #region Members

        private readonly string _host;
        private bool _disposed;
        private HttpListener _listener;

        /// <summary>
        /// Gets the HttpListener host.
        /// </summary>
        public string Host => _host;

        /// <summary>
        /// Gets a value that indecates whether HttpListener has been started.
        /// </summary>
        public bool IsListening => _listener.IsListening;

        #endregion Members

        #region Events

        /// <summary>
        /// Executes after client request failed.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1003:使用泛型事件处理程序实例", Justification = "<挂起>")]
        public event UPnPRequestFailedEventHandler RequestFailed;

        /// <summary>
        /// Executes after client request failed.
        /// </summary>
        /// <param name="server">The sender who raised the event.</param>
        /// <param name="request">Dlna client request.</param>
        /// <param name="exception">Exception.</param>
        protected virtual void OnRequestFailed(UPnPServer server, HttpListenerRequest request, Exception exception)
        {
            RequestFailed?.Invoke(server, request, exception);
        }

        #endregion Events

        #region Construction

        /// <summary>
        /// Initializes a new instance of the UPnPServer class. Need setup port open for firewall. Administrator privileges are required.
        /// </summary>
        /// <param name="localHost">Create HttpListener by the local host used external address:port. e.g. <see langword="http://192.168.1.100:8080"/>.</param>
        /// <exception cref="Exception"/>
        protected UPnPServer(Uri localHost)
        {
            if (localHost == null)
            {
                throw new ArgumentNullException(nameof(localHost));
            }
            _host = localHost.AbsoluteUri;
            _listener = new HttpListener
            {
                AuthenticationSchemes = AuthenticationSchemes.Anonymous
            };
            _listener.Prefixes.Add(_host);
        }

        /// <summary>
        /// Releases resources at the instance.
        /// </summary>
        ~UPnPServer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases resources at the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources at the instance.
        /// </summary>
        /// <param name="disposing">Releases managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
                try
                {
                    _listener.Close();
                    _listener = null;
                }
                catch
                {
                }
                _disposed = true;
            }
        }

        #endregion Construction

        /// <summary>
        /// Shuts down, discarding all currently queued requests.
        /// </summary>
        public void Abort()
        {
            _listener.Abort();
        }

        /// <summary>
        /// Shuts down.
        /// </summary>
        public void Close()
        {
            _listener.Close();
        }

        /// <summary>
        /// Allows this instance to receive incoming requests.
        /// </summary>
        /// <exception cref="Exception"/>
        public void Start()
        {
            if (!_listener.IsListening)
            {
                _listener.Start();
                _listener.BeginGetContext(GottenContext, null);
            }
        }

        /// <summary>
        /// This instance is not allowed to receive incoming requests.
        /// </summary>
        public void Stop()
        {
            if (_listener.IsListening)
            {
                _listener.Stop();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        /// <param name="exception"></param>
        /// <param name="handled"></param>
        /// <exception cref="Exception"></exception>
        protected abstract void HandleContext(HttpListenerContext context, string url, out Exception exception, out bool handled);

        private void GottenContext(IAsyncResult ar)
        {
            try
            {
                HttpListenerContext context = _listener.EndGetContext(ar);
                _listener.BeginGetContext(GottenContext, null);
                string url = context.Request.Url.AbsoluteUri;
                HandleContext(context, url, out Exception exception, out bool handled);
                context.Response.Close();
                if (!handled)
                {
                    exception = new HttpListenerException(404, $"Unknown request url - \"{url}\"");
                    OnRequestFailed(this, context.Request, exception);
                }
                else if (exception != null)
                {
                    OnRequestFailed(this, context.Request, exception);
                }
            }
            catch (Exception ex)
            {
                OnRequestFailed(this, null, ex);
            }
        }
    }
}