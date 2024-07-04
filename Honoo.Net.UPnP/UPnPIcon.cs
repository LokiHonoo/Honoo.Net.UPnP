using System.Xml;

namespace Honoo.Net.UPnP
{
    /// <summary>
    /// UPnP icon.
    /// </summary>
    public sealed class UPnPIcon
    {
        #region Properties

        private readonly string _depth;
        private readonly string _height;
        private readonly string _mimetype;
        private readonly UPnPDevice _parentDevice;
        private readonly UPnPRootDevice _rootDevice;
        private readonly string _url;
        private readonly string _width;

        /// <summary>
        /// Depth.
        /// </summary>
        public string Depth => _depth;

        /// <summary>
        /// Height.
        /// </summary>
        public string Height => _height;

        /// <summary>
        /// Mimetype.
        /// </summary>
        public string Mimetype => _mimetype;

        /// <summary>
        /// Parent device.
        /// </summary>
        public UPnPDevice ParentDevice => _parentDevice;

        /// <summary>
        /// Root device.
        /// </summary>
        public UPnPRootDevice RootDevice => _rootDevice;

        /// <summary>
        /// Url.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:类 URI 属性不应是字符串", Justification = "<挂起>")]
        public string Url => _url;

        /// <summary>
        /// Width.
        /// </summary>
        public string Width => _width;

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the UPnPIcon class.
        /// </summary>
        /// <param name="iconNode">Icon XmlNode.</param>
        /// <param name="nm">XmlNamespaceManager.</param>
        /// <param name="parentDevice">The UPnPDevice to which the device belongs.</param>
        /// <param name="rootDevice">The UPnPRootDevice to which the root device belongs.</param>
        internal UPnPIcon(XmlNode iconNode, XmlNamespaceManager nm, UPnPDevice parentDevice, UPnPRootDevice rootDevice)
        {
            _depth = iconNode.SelectSingleNode("default:depth", nm)?.InnerText.Trim();
            _height = iconNode.SelectSingleNode("default:height", nm)?.InnerText.Trim();
            _mimetype = iconNode.SelectSingleNode("default:mimetype", nm)?.InnerText.Trim();
            _url = iconNode.SelectSingleNode("default:url", nm)?.InnerText.Trim();
            _width = iconNode.SelectSingleNode("default:width", nm)?.InnerText.Trim();
            _parentDevice = parentDevice;
            _rootDevice = rootDevice;
        }
    }
}