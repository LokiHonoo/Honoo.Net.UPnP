using System;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP port mapping entry.
    /// </summary>
    public sealed class UPnPPortMappingEntry
    {
        private readonly string _description;
        private readonly bool _enabled;
        private readonly ushort _externalPort;
        private readonly string _internalClient;
        private readonly ushort _internalPort;
        private readonly uint _leaseDuration;
        private readonly string _protocol;
        private readonly string _remoteHost;

        #region Members

        /// <summary>
        /// Description.
        /// </summary>
        public string Description => _description;

        /// <summary>
        /// Enabled.
        /// </summary>
        public bool Enabled => _enabled;

        /// <summary>
        /// External port.
        /// </summary>
        public ushort ExternalPort => _externalPort;

        /// <summary>
        /// Internal client.
        /// </summary>
        public string InternalClient => _internalClient;

        /// <summary>
        /// Internal port.
        /// </summary>
        public ushort InternalPort => _internalPort;

        /// <summary>
        /// Lease duration. This property unit is seconds.
        /// </summary>
        public uint LeaseDuration => _leaseDuration;

        /// <summary>
        /// Protocol. This property accepts the following: "TCP", "UDP".
        /// </summary>
        public string Protocol => _protocol;

        /// <summary>
        /// Remote host.
        /// </summary>
        public string RemoteHost => _remoteHost;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPPortMappingEntry class.
        /// </summary>
        /// <param name="element">Response element.</param>
        internal UPnPPortMappingEntry(XElement element)
        {
            _protocol = element.Element("NewProtocol").Value.Trim();
            _remoteHost = element.Element("NewRemoteHost").Value.Trim();
            _externalPort = ushort.Parse(element.Element("NewExternalPort").Value.Trim(), CultureInfo.InvariantCulture);
            _internalClient = element.Element("NewInternalClient").Value.Trim();
            _internalPort = ushort.Parse(element.Element("NewInternalPort").Value.Trim(), CultureInfo.InvariantCulture);
            _enabled = Convert.ToBoolean(int.Parse(element.Element("NewEnabled").Value.Trim(), CultureInfo.InvariantCulture));
            _description = element.Element("NewPortMappingDescription").Value.Trim();
            _leaseDuration = uint.Parse(element.Element("NewLeaseDuration").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Initializes a new instance of the UPnPPortMappingEntry class.
        /// </summary>
        /// <param name="protocol">Protocol.</param>
        /// <param name="remoteHost">Remote host.</param>
        /// <param name="externalPort">External port.</param>
        /// <param name="element">Response element.</param>
        internal UPnPPortMappingEntry(string protocol, string remoteHost, ushort externalPort, XElement element)
        {
            _protocol = protocol;
            _remoteHost = remoteHost;
            _externalPort = externalPort;
            _internalClient = element.Element("NewInternalClient").Value.Trim();
            _internalPort = ushort.Parse(element.Element("NewInternalPort").Value.Trim(), CultureInfo.InvariantCulture);
            _enabled = Convert.ToBoolean(int.Parse(element.Element("NewEnabled").Value.Trim(), CultureInfo.InvariantCulture));
            _description = element.Element("NewPortMappingDescription").Value.Trim();
            _leaseDuration = uint.Parse(element.Element("NewLeaseDuration").Value.Trim(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"NewProtocol:{_protocol}");
            builder.AppendLine($"NewRemoteHost:{_remoteHost}");
            builder.AppendLine($"NewExternalPort:{_externalPort}");
            builder.AppendLine($"NewInternalClient:{_internalClient}");
            builder.AppendLine($"NewInternalPort:{_internalPort}");
            builder.AppendLine($"NewEnabled:{(_enabled ? 1 : 0)}");
            builder.AppendLine($"NewPortMappingDescription:{_description}");
            builder.AppendLine($"NewLeaseDuration:{_leaseDuration}");
            return builder.ToString();
        }
    }
}