/*
 * Copyright (c) 2011 Washington State Department of Transportation
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>
 *
 */
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
