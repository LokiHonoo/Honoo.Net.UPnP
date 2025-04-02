using System.Collections.Generic;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP event LastChange instance.
    /// </summary>
    public sealed class UPnPChangeInstance
    {
        #region Members

        private readonly uint _instanceID;
        private readonly Dictionary<string, UPnPChangeProperty> _properties = new Dictionary<string, UPnPChangeProperty>();

        /// <summary>
        /// Instance ID.
        /// </summary>
        public uint InstanceID => _instanceID;

        /// <summary>
        /// Instances.
        /// </summary>
        public Dictionary<string, UPnPChangeProperty> Properties => _properties;

        #endregion Members

        internal UPnPChangeInstance(uint instanceID)
        {
            _instanceID = instanceID;
        }
    }
}