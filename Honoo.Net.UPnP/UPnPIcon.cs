using System.Xml.Linq;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP icon.
    /// </summary>
    public sealed class UPnPIcon
    {
        #region Members

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
        public string Url => _url;

        /// <summary>
        /// Width.
        /// </summary>
        public string Width => _width;

        #endregion Members

        /// <summary>
        /// Initializes a new instance of the UPnPIcon class.
        /// </summary>
        /// <param name="iconElement">Icon element.</param>
        /// <param name="parentDevice">The UPnPDevice to which the device belongs.</param>
        /// <param name="rootDevice">The UPnPRootDevice to which the root device belongs.</param>
        internal UPnPIcon(XElement iconElement, UPnPDevice parentDevice, UPnPRootDevice rootDevice)
        {
            XNamespace nm = iconElement.GetDefaultNamespace();
            _depth = iconElement.Element(nm + "depth")?.Value.Trim();
            _height = iconElement.Element(nm + "height")?.Value.Trim();
            _mimetype = iconElement.Element(nm + "mimetype")?.Value.Trim();
            _url = iconElement.Element(nm + "url")?.Value.Trim();
            _width = iconElement.Element(nm + "width")?.Value.Trim();
            _parentDevice = parentDevice;
            _rootDevice = rootDevice;
        }
    }
}