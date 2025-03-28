using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Honoo.Net
{
    /// <summary>
    /// UPnP DLNA media server. Need setup port open for firewall. Administrator privileges are required.
    /// </summary>
    public class UPnPDlnaServer : UPnPEventSubscriber
    {
        #region Members

        private readonly HashAlgorithm _hash = HashAlgorithm.Create("SHA256");
        private readonly Dictionary<string, Tuple<bool, string, Stream>> _media = new Dictionary<string, Tuple<bool, string, Stream>>();
        private bool _disposed;

        /// <summary>
        /// Gets media count.
        /// </summary>
        public int MediaCount => _media.Count;

        #endregion Members

        #region Construction

        /// <summary>
        /// Initializes a new instance of the UPnPDlnaServer class. Need setup firewall. Administrator privileges are required.
        /// </summary>
        /// <param name="localHost">Create HttpListener by the local host used external address:port. e.g. <see langword="http://192.168.1.100:8080"/>.</param>
        /// <param name="start">Start HttpListener at now.</param>
        public UPnPDlnaServer(Uri localHost, bool start = true) : base(localHost, start)
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
                    _media.Clear();
                }
                _hash.Dispose();
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        #endregion Construction

        /// <summary>
        /// Add a media file and gets transport url.
        /// </summary>
        /// <param name="mediaFile">Local file full path to play.</param>
        /// <param name="checkFileExists">Check file exists.</param>
        /// <returns></returns>
        public string AddMedia(string mediaFile, bool checkFileExists = true)
        {
            if (string.IsNullOrWhiteSpace(mediaFile))
            {
                throw new ArgumentException($"The invalid argument - {nameof(mediaFile)}.");
            }
            if (checkFileExists && !File.Exists(mediaFile))
            {
                throw new IOException("File not exists.");
            }
            string id = Encoding.ASCII.GetString(_hash.ComputeHash(Encoding.UTF8.GetBytes(mediaFile)));
            Uri uri = new Uri(base.Host + id);
            string url = uri.AbsoluteUri;
            if (!_media.ContainsKey(url))
            {
                _media.Add(url, new Tuple<bool, string, Stream>(false, mediaFile, null));
            }
            return url;
        }

        /// <summary>
        /// Add a media file and gets transport url.
        /// </summary>
        /// <param name="mediaStream">Media cache to play.</param>
        /// <returns></returns>
        public string AddMedia(Stream mediaStream)
        {
            if (mediaStream is null)
            {
                throw new ArgumentNullException(nameof(mediaStream));
            }
            string id = Encoding.ASCII.GetString(_hash.ComputeHash(mediaStream));
            Uri uri = new Uri(base.Host + id);
            string url = uri.AbsoluteUri;
            if (!_media.ContainsKey(url))
            {
                _media.Add(url, new Tuple<bool, string, Stream>(false, string.Empty, mediaStream));
            }
            return url;
        }

        /// <summary>
        /// Removes all media.
        /// </summary>
        public void ClearMedia()
        {
            _media.Clear();
        }

        /// <summary>
        /// Removes specified media.
        /// </summary>
        /// <param name="url">The url of the element to remove.</param>
        public void RemoveMedia(string url)
        {
            _media.Remove(url);
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
            base.HandleContext(context, url, out exception, out handled);
            if (!handled)
            {
                if (context is null)
                {
                    exception = new ArgumentNullException(nameof(context));
                    handled = true;
                }
                else if (_media.TryGetValue(url, out Tuple<bool, string, Stream> data))
                {
                    string range = context.Request.Headers["Range"];
                    try
                    {
                        if (data.Item1)
                        {
                            Transport(data.Item3, range, context.Response);
                        }
                        else
                        {
                            using (FileStream media = new FileStream(data.Item2, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                Transport(media, range, context.Response);
                            }
                        }
                        exception = null;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                    try
                    {
                        context.Response.OutputStream.Flush();
                        context.Response.OutputStream.Close();
                    }
                    catch
                    {
                        // exception = null;
                    }
                    handled = true;
                }
                else
                {
                    exception = null;
                    handled = false;
                }
            }
        }

        private static void Transport(Stream media, string range, HttpListenerResponse response)
        {
            if (range != null)
            {
                range = range.ToUpperInvariant().Replace("BYTES=", string.Empty).TrimEnd('-');
                long position = long.Parse(range, CultureInfo.InvariantCulture);
                response.StatusCode = 206;
                response.Headers.Add("Cache-Control: no-store");
                response.Headers.Add("Pragma: no-cache");
                response.Headers.Add("Connection: Keep=Alive");
                response.Headers.Add("transferMode.dlna.org: Streaming");
                //response.Headers.Add("Content-Type: application/octet-stream");
                response.Headers.Add("Accept-Ranges: bytes");
                response.ContentLength64 = media.Length - position;
                response.Headers.Add($"Content-Range: bytes {range}/{media.Length}");
                media.Seek(position, SeekOrigin.Begin);
                media.CopyTo(response.OutputStream);
            }
            else
            {
                response.StatusCode = 200;
                response.Headers.Add("Cache-Control: no-store");
                response.Headers.Add("Pragma: no-cache");
                response.Headers.Add("Connection: Keep=Alive");
                response.Headers.Add("transferMode.dlna.org: Streaming");
                //response.Headers.Add("Content-Type: application/octet-stream");
                media.Seek(0, SeekOrigin.Begin);
                media.CopyTo(response.OutputStream);
            }
        }
    }
}