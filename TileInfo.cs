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
using System.Runtime.Serialization;
namespace ArcGisTileTest
{
    [DataContract]
    public class TileInfo
    {
        [DataMember]
        public int rows { get; set; }
        [DataMember]
        public int cols { get; set; }
        [DataMember]
        public float dpi { get; set; }
        [DataMember]
        public string format { get; set; }
        [DataMember]
        public float compressionQuality { get; set; }
        [DataMember]
        public Point origin { get; set; }
        [DataMember]
        public SpatialReference spatialReference { get; set; }
        [DataMember]
        public LevelOfDetail[] lods { get; set; }
    }
}
