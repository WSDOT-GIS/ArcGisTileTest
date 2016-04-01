using System;
using System.Linq;

namespace ArcGisTileTest
{
    public static class Extensions
    {
        /// <summary>
        /// Returns the URL for the tile of this map service at the specified level of detail, row, and column.
        /// </summary>
        /// <param name="mapServiceUrl"></param>
        /// <param name="level"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetTileUrl(this string mapServiceUrl, int level, int row, int column)
        {
            if (string.IsNullOrWhiteSpace(mapServiceUrl))
            {
                throw new ArgumentException("URL cannot be null, empty, or white space", "mapServiceUrl");
            }
            // Trim the query string off if applicable.
            if (mapServiceUrl.Contains('?'))
            {
                int qPos = mapServiceUrl.IndexOf('?');
                mapServiceUrl = mapServiceUrl.Substring(0, qPos);
            }

            return string.Format("{0}/tile/{1}/{2}/{3}", mapServiceUrl, level, row, column);
        }
    }
}
