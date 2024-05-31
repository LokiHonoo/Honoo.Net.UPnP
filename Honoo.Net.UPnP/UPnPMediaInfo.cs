using System.Text;
using System.Xml;

namespace Honoo.Net.UPnP
{
    /// <summary>
    /// UPnP media info.
    /// </summary>
    public sealed class UPnPMediaInfo
    {
        #region Properties

        private readonly string _currentURI;
        private readonly string _currentURIMetaData;
        private readonly string _mediaDuration;
        private readonly string _nextURI;
        private readonly string _nextURIMetaData;
        private readonly uint _nrTracks;
        private readonly string _playMedium;
        private readonly string _recordMedium;
        private readonly string _writeStatus;

        /// <summary>
        /// Current audio/video transport uri.
        /// </summary>
        public string CurrentURI => _currentURI;

        /// <summary>
        /// Current audio/video transport uri meta data.
        /// </summary>
        public string CurrentURIMetaData => _currentURIMetaData;

        /// <summary>
        /// Current media duration.
        /// </summary>
        public string MediaDuration => _mediaDuration;

        /// <summary>
        /// Next audio/video transport uri.
        /// </summary>
        public string NextURI => _nextURI;

        /// <summary>
        /// Next audio/video transport uri meta data.
        /// </summary>
        public string NextURIMetaData => _nextURIMetaData;

        /// <summary>
        /// Number of tracks.
        /// </summary>
        public uint NrTracks => _nrTracks;

        /// <summary>
        /// Playback storage medium. Allowed value "NONE", "UNKNOWN", "CD-DA", "HDD", "NETWORK".
        /// </summary>
        public string PlayMedium => _playMedium;

        /// <summary>
        /// Record storage medium.
        /// </summary>
        public string RecordMedium => _recordMedium;

        /// <summary>
        /// Record medium write status.
        /// </summary>
        public string WriteStatus => _writeStatus;

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the UPnPMediaInfo class.
        /// </summary>
        /// <param name="node">Response node.</param>
        internal UPnPMediaInfo(XmlNode node)
        {
            _nrTracks = uint.Parse(node.SelectSingleNode("NrTracks").InnerText.Trim());
            _mediaDuration = node.SelectSingleNode("MediaDuration").InnerText.Trim();
            _currentURI = node.SelectSingleNode("CurrentURI").InnerText.Trim();
            _currentURIMetaData = node.SelectSingleNode("CurrentURIMetaData").InnerText.Trim();
            _nextURI = node.SelectSingleNode("NextURI").InnerText.Trim();
            _nextURIMetaData = node.SelectSingleNode("NextURIMetaData").InnerText.Trim();
            _playMedium = node.SelectSingleNode("PlayMedium").InnerText.Trim();
            _recordMedium = node.SelectSingleNode("RecordMedium").InnerText.Trim();
            _writeStatus = node.SelectSingleNode("WriteStatus").InnerText.Trim();
        }

        /// <summary>
        /// This method has been overridden. Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"NrTracks:{_nrTracks}");
            builder.AppendLine($"MediaDuration:{_mediaDuration}");
            builder.AppendLine($"CurrentURI:{_currentURI}");
            builder.AppendLine($"CurrentURIMetaData:{_currentURIMetaData}");
            builder.AppendLine($"NextURI:{_nextURI}");
            builder.AppendLine($"NextURIMetaData:{_nextURIMetaData}");
            builder.AppendLine($"PlayMedium:{_playMedium}");
            builder.AppendLine($"RecordMedium:{_recordMedium}");
            builder.AppendLine($"WriteStatus:{_writeStatus}");
            return builder.ToString();
        }
    }
}