using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP event subscribing server. Need setup port open for firewall. Administrator privileges are required.
    /// </summary>
    public class UPnPEventServer : UPnPServer
    {
        #region Members

        private readonly Dictionary<string, Tuple<UPnPEventRaisedCallback, object>> _eventSubscribers = new Dictionary<string, Tuple<UPnPEventRaisedCallback, object>>();
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
        /// <param name="userState">Pass user state.</param>
        /// <returns></returns>
        public string AddEventSubscriber(UPnPEventRaisedCallback callback, object userState)
        {
            Uri uri = new Uri(base.Host + "subscriber" + _counter);
            string url = uri.AbsoluteUri;
            _eventSubscribers.Add(url, new Tuple<UPnPEventRaisedCallback, object>(callback, userState));
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
            else if (_eventSubscribers.TryGetValue(url, out Tuple<UPnPEventRaisedCallback, object> callback))
            {
                try
                {
                    byte[] buffer = new byte[context.Request.ContentLength64];
                    _ = context.Request.InputStream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer);
                    XDocument doc = XDocument.Parse(response);
                    string lastChangeString = doc.Root.Element("property").Element("LastChange").Value;
                    lastChangeString = lastChangeString.Replace("&lt;", "<").Replace("&quot;", "\"").Replace("&gt;", ">");
                    UPnPEventMessage message;
                    if (lastChangeString.Contains("urn:schemas-upnp-org:metadata-1-0/AVT/"))
                    {
                        message = HandleMediaRenderer(url, lastChangeString);
                    }
                    else if (lastChangeString.Contains("urn:schemas-upnp-org:metadata-1-0/RCS/"))
                    {
                        message = HandleMediaRenderer(url, lastChangeString);
                    }
                    else
                    {
                        message = new UPnPUnknownEventMessage(url, lastChangeString);
                    }
                    callback?.Item1?.Invoke(this, message, callback.Item2);
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

        private static UPnPMediaRendererEventMessage HandleMediaRenderer(string url, string lastChangeString)
        {
            var message = new UPnPMediaRendererEventMessage(url, lastChangeString);
            XElement root = XElement.Parse(lastChangeString);
            foreach (XElement instanceElement in root.Elements())
            {
                uint instanceID = uint.Parse(instanceElement.Attribute("val").Value, CultureInfo.InvariantCulture);
                var instance = new UPnPChangeInstance(instanceID);
                foreach (XElement propertyElement in instanceElement.Elements())
                {
                    var property = new UPnPChangeProperty(propertyElement.Name.LocalName);
                    foreach (XAttribute att in propertyElement.Attributes())
                    {
                        property.Attributes.Add(att.Name.LocalName, att.Value);
                    }
                    instance.Properties.Add(property.PropertyName, property);
                }
                message.Instances.Add(instanceID, instance);
            }
            return message;
        }
    }
}