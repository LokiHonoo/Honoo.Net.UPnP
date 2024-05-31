using System.Collections.Generic;

namespace Honoo.Net.UPnP
{
    /// <summary>
    /// UPnP event subscription response message.
    /// </summary>
    public sealed class UPnPEventMessage
    {
        #region Properties

        private readonly IDictionary<string, string> _changes;
        private readonly uint _instanceID;

        /// <summary>
        /// Changes.
        /// </summary>
        public IDictionary<string, string> Changes => _changes;

        /// <summary>
        /// Instance ID.
        /// </summary>
        public uint InstanceID => _instanceID;

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the UPnPEventMessage class.
        /// </summary>
        /// <param name="instanceID">Instance ID.</param>
        /// <param name="changes">Changes.</param>
        internal UPnPEventMessage(uint instanceID, IDictionary<string, string> changes)
        {
            _instanceID = instanceID;
            _changes = changes;
        }
    }
}