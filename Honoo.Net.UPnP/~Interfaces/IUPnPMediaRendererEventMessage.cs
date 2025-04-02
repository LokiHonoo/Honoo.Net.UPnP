using System.Collections.Generic;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP event LastChange message interface.
    /// </summary>
    public interface IUPnPMediaRendererEventMessage : IUPnPEventMessage
    {
        /// <summary>
        /// Instances.
        /// </summary>
        Dictionary<uint, UPnPChangeInstance> Instances { get; }
    }
}