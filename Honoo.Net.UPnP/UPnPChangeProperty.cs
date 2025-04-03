using System.Collections.Generic;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP event LastChange property.
    /// </summary>
    public sealed class UPnPChangeProperty
    {
        #region Members

        private readonly Dictionary<string, string> _attributes = new Dictionary<string, string>();
        private readonly string _name;

        /// <summary>
        /// gets property attributes.
        /// </summary>
        public Dictionary<string, string> Attributes => _attributes;

        /// <summary>
        /// gets property name.
        /// </summary>
        public string Name => _name;

        #endregion Members

        internal UPnPChangeProperty(string name)
        {
            _name = name;
        }
    }
}