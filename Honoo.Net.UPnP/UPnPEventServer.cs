using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP event subscribing server. Need setup port open for firewall. Administrator privileges are required.
    /// </summary>
    public class UPnPEventServer : UPnPServer
    {
        #region Members

        private readonly Dictionary<string, UPnPEventRaisedCallback> _eventSubscribers = new Dictionary<string, UPnPEventRaisedCallback>();
        private int _counter;
        private bool _disposed;

        /// <summary>
        /// Gets event subscriber count.
        /// </summary>
        public int EventSubscriberCount => _eventSubscribers.Count;

        #endregion Members

        #region Construction

        /// <summary>
        /// Initializes a new instance of the UPnPEventServer class. Need setup port open for firewall. Administrator privileges are required.
        /// </summary>
        /// <param name="localHost">Create HttpListener by the local host used external address:port. e.g. <see langword="http://192.168.1.100:8080"/>.</param>
        /// <exception cref="Exception"/>
        public UPnPEventServer(Uri localHost) : base(localHost)
        {
        }

        /// <summary>
        /// Releases resources at the instance.
        /// </summary>
        /// <param name="disposing">Releases managed resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _eventSubscribers.Clear();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        #endregion Construction

        /// <summary>
        /// Add a event subscriber and gets event subscriber url.
        /// </summary>
        /// <param name="callback">UPnP event updated callback.</param>
        /// <returns></returns>
        public string AddEventSubscriber(UPnPEventRaisedCallback callback)
        {
            Uri uri = new Uri(base.Host + "subscriber" + _counter);
            string url = uri.AbsoluteUri;
            _eventSubscribers.Add(url, callback);
            _counter++;
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
        /// Removes specified event subscriber.
        /// </summary>
        /// <param name="url">The url of the element to remove.</param>
        public void RemoveEventSubscriber(string url)
        {
            _eventSubscribers.Remove(url);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        /// <param name="exception"></param>
        /// <param name="handled"></param>
        protected override void HandleContext(HttpListenerContext context, string url, out Exception exception, out bool handled)
        {
            if (context is null)
            {
                exception = new ArgumentNullException(nameof(context));
                handled = true;
            }
            else if (_eventSubscribers.TryGetValue(url, out UPnPEventRaisedCallback callback))
            {
                try
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
                    callback?.Invoke(this, messages.ToArray());
                    exception = null;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                handled = true;
            }
            else
            {
                exception = null;
                handled = false;
            }
        }
    }
}