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
