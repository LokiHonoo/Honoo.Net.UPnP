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
        private readonly string _propertyName;

        /// <summary>
        /// Attributes.
        /// </summary>
        public Dictionary<string, string> Attributes => _attributes;

        /// <summary>
        /// Property name.
        /// </summary>
        public string PropertyName => _propertyName;

        #endregion Members

        internal UPnPChangeProperty(string propertyName)
        {
            _propertyName = propertyName;
        }
    }
}