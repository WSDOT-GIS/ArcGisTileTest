using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ArcGisTileTest
{
    [DataContract]
    public class MapService
    {
        [DataMember]
        public object currentVersion { get; set; }
        [DataMember]
        public string serviceDescription { get; set; }
        [DataMember]
        public string mapName { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string copyrightText { get; set; }
        [DataMember]
        public Layer[] layers { get; set; }
        [DataMember]
        public Table[] tables { get; set; }
        [DataMember]
        public SpatialReference spatialReference { get; set; }
        [DataMember]
        public bool SingleFusedMapCache { get; set; }
        [DataMember]
        public TileInfo tileInfo { get; set; }
        [DataMember]
        public Extent initialExtent { get; set; }
        [DataMember]
        public Extent fullExtent { get; set; }
        [DataMember]
        public Dictionary<string, object> timeInfo { get; set; }
        [DataMember]
        public string units { get; set; }
        [DataMember]
        public string supportedImageFormatTypes { get; set; }
        [DataMember]
        public Dictionary<string, object> documentInfo { get; set; }
        [DataMember]
        public string capabilities { get; set; }

        /// <summary>
        /// Calculates the width of a given tile in a level of detail.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public double GetTileWidthInMapUnits(int level)
        {
            var tileInfo = this.tileInfo;
            var lod = tileInfo.lods[level];
            return tileInfo.rows * lod.resolution;
        }

        /// <summary>
        /// Calculates the height of a given tile in a level of detail.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public double GetTileHeightInMapUnits(int level)
        {
            var tileInfo = this.tileInfo;
            var lod = tileInfo.lods[level];
            return tileInfo.cols * lod.resolution;
        }

        /// <summary>
        /// Calculates the extent of the specified tile in the specified level of detail.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public Extent GetTileExtent(int level, int row, int column)
        {
            var tileWidth = this.GetTileWidthInMapUnits(level);
            var tileHeight = this.GetTileHeightInMapUnits(level);

            var extent = new Extent
            {
                xmin = column * tileWidth + this.tileInfo.origin.x,
                ymax = (row * tileHeight - this.tileInfo.origin.y) / -1,
            };

            extent.xmax = extent.xmin + tileWidth;
            extent.ymin = extent.ymax - tileWidth;
            return extent;
        }

        /// <summary>
        /// Determine which tile at a specified level of detail contains the given point.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public Tile GetTileContainingPoint(int level, Point point)
        {
            var tileInfo = this.tileInfo;
            var origin = tileInfo.origin;
            return new Tile
            {
                Column = (int)Math.Floor((point.x - origin.x) / this.GetTileWidthInMapUnits(level)),
                Row = (int)Math.Floor((origin.y - point.y) / this.GetTileHeightInMapUnits(level))
            };
        }

        /// <summary>
        /// Gets the first tile in a map service at the specified level of detail.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public Tile GetFirstTile(int level)
        {
            var fullExtent = this.fullExtent;
            return this.GetTileContainingPoint(level, new Point { x = fullExtent.xmin, y = fullExtent.ymax });
        }

        /// <summary>
        /// Gets the last tile in a map service at the specified level of detail.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public Tile GetLastTile(int level)
        {
            var fullExtent = this.fullExtent;
            return this.GetTileContainingPoint(level, new Point { x = fullExtent.xmax, y = fullExtent.ymin });
        }
    }
}
