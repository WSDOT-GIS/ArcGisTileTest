using System.Runtime.Serialization;

namespace ArcGisTileTest
{
    [DataContract]
    public class SpatialReference
    {
        [DataMember]
        public int? wkid { get; set; }
        [DataMember]
        public string wkt { get; set; }
    }
}
