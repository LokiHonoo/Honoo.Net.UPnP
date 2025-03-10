﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Honoo.Net.UPnP
{
    /// <summary>
    /// UPnP DLNA media server.
    /// <br/>Administrator privileges are required.
    /// </summary>
    public sealed class UPnPDlnaServer : IDisposable
    {
        #region Properties

        private readonly Dictionary<string, UPnPEventCallback> _eventSubscribers = new Dictionary<string, UPnPEventCallback>();
        private readonly HashAlgorithm _hash = HashAlgorithm.Create("SHA256");
        private readonly string _host;
        private readonly Dictionary<string, string> _media = new Dictionary<string, string>();
        private int _counter;
        private bool _disposed;
        private HttpListener _listener;

        /// <summary>
        /// Gets a value that indecates whether HttpListener has been started.
        /// </summary>
        public bool IsListening => _listener.IsListening;

        #endregion Properties

        #region Construction

        /// <summary>
        /// Initializes a new instance of the UPnPDlnaServer class. Need setup firewall. Administrator privileges are required.
        /// </summary>
        /// <param name="localHost">Create HttpListener by the local host used external address:port. e.g. <see langword="http://192.168.1.100:8080"/>.</param>
        /// <param name="start">Start HttpListener at now.</param>
        public UPnPDlnaServer(Uri localHost, bool start = true)
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
            if (start)
            {
                _listener.Start();
                _listener.BeginGetContext(GottenContext, null);
            }
        }

        /// <summary>
        /// Releases resources at the instance.
        /// </summary>
        ~UPnPDlnaServer()
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
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _eventSubscribers.Clear();
                    _media.Clear();
                }
                _hash.Dispose();
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
        /// Add a event subscriber and gets event subscriber url.
        /// </summary>
        /// <param name="callback">UPnP event updated callback.</param>
        /// <returns></returns>
        public string AddEventSubscriber(UPnPEventCallback callback)
        {
            Uri uri = new Uri(_host + "subscriber" + _counter);
            string url = uri.AbsoluteUri;
            _eventSubscribers.Add(url, callback);
            _counter++;
            return url;
        }

        /// <summary>
        /// Add a media file and gets transport url.
        /// </summary>
        /// <param name="file">Local file full path to play.</param>
        /// <param name="checkFileExists">Check file exists.</param>
        /// <returns></returns>
        public string AddMedia(string file, bool checkFileExists = true)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentException($"The invalid argument - {nameof(file)}.");
            }
            if (checkFileExists && !File.Exists(file))
            {
                throw new IOException("File not exists.");
            }
            string id = Encoding.ASCII.GetString(_hash.ComputeHash(Encoding.UTF8.GetBytes(file)));
            Uri uri = new Uri(_host + id);
            string url = uri.AbsoluteUri;
            if (!_media.ContainsKey(url))
            {
                _media.Add(url, file);
            }
            return url;
        }

        /// <summary>
        /// Removes all event subscribers.
        /// </summary>
        public void ClearEventSubscribers()
        {
            _eventSubscribers.Clear();
        }

        /// <summary>
        /// Removes all media.
        /// </summary>
        public void ClearMedia()
        {
            _media.Clear();
        }

        /// <summary>
        /// Shuts down.
        /// </summary>
        public void Close()
        {
            _listener.Close();
        }

        /// <summary>
        /// Removes specified event subscriber.
        /// </summary>
        /// <param name="url">The url of the element to remove.</param>
        public void RemoveEventSubscriber(string url)
        {
            _eventSubscribers.Remove(url);
        }

        /// <summary>
        /// Removes specified media.
        /// </summary>
        /// <param name="url">The url of the element to remove.</param>
        public void RemoveMedia(string url)
        {
            _media.Remove(url);
        }

        /// <summary>
        /// Allows this instance to receive incoming requests.
        /// </summary>
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

        private void GottenContext(IAsyncResult ar)
        {
            HttpListenerContext context = _listener.EndGetContext(ar);
            _listener.BeginGetContext(GottenContext, null);
            string url = context.Request.Url.AbsoluteUri;
            if (_eventSubscribers.TryGetValue(url, out UPnPEventCallback callback))
            {
                byte[] buffer = new byte[context.Request.ContentLength64];
                _ = context.Request.InputStream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer);
                XmlDocument doc = new XmlDocument() { XmlResolver = null };
                doc.LoadXml(response.Replace("&lt;", "<").Replace("&quot;", "\"").Replace("&gt;", ">"));
                //StringReader sreader = new StringReader(response.Replace("&lt;", "<").Replace("&quot;", "\"").Replace("&gt;", ">"));
                //using (XmlReader reader = XmlReader.Create(sreader, new XmlReaderSettings() { XmlResolver = null }))
                //{
                //    doc.Load(reader);
                //}
                XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("e", "urn:schemas-upnp-org:event-1-0");
                ns.AddNamespace("avt", "urn:schemas-upnp-org:metadata-1-0/AVT/");
                XmlNodeList instances = doc.SelectNodes("/propertyset/property/LastChange/avt:Event/avt:InstanceID", ns);
                List<UPnPEventMessage> messages = new List<UPnPEventMessage>();
                foreach (XmlNode instance in instances)
                {
                    uint instanceID = uint.Parse(instance.Attributes["val"].InnerText, CultureInfo.InvariantCulture);
                    Dictionary<string, string> changes = new Dictionary<string, string>();
                    foreach (XmlNode node in instance.ChildNodes)
                    {
                        changes.Add(node.LocalName, node.Attributes["val"].InnerText);
                    }
                    messages.Add(new UPnPEventMessage(instanceID, changes));
                }
                callback?.Invoke(messages.ToArray());
            }
            else if (_media.TryGetValue(url, out string file))
            {
                using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    string range = context.Request.Headers["Range"];
                    if (range != null)
                    {
                        range = range.ToUpperInvariant().Replace("BYTES=", string.Empty).TrimEnd('-');
                        long position = long.Parse(range, CultureInfo.InvariantCulture);
                        context.Response.StatusCode = 206;
                        context.Response.Headers.Add("Cache-Control: no-store");
                        context.Response.Headers.Add("Pragma: no-cache");
                        context.Response.Headers.Add("Connection: Keep=Alive");
                        context.Response.Headers.Add("transferMode.dlna.org: Streaming");
                        //context.Response.Headers.Add("Content-Type: application/octet-stream");
                        context.Response.Headers.Add("Accept-Ranges: bytes");
                        context.Response.ContentLength64 = stream.Length - position;
                        context.Response.Headers.Add($"Content-Range: bytes {range}/{stream.Length}");
                        stream.Seek(position, SeekOrigin.Begin);
                        stream.CopyTo(context.Response.OutputStream);
                    }
                    else
                    {
                        context.Response.StatusCode = 200;
                        context.Response.Headers.Add("Cache-Control: no-store");
                        context.Response.Headers.Add("Pragma: no-cache");
                        context.Response.Headers.Add("Connection: Keep=Alive");
                        context.Response.Headers.Add("transferMode.dlna.org: Streaming");
                        //context.Response.Headers.Add("Content-Type: application/octet-stream");
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.CopyTo(context.Response.OutputStream);
                    }
                }
                context.Response.OutputStream.Flush();
                context.Response.OutputStream.Close();
                context.Response.Close();
            }
        }
    }
}