using System.Runtime.Serialization;

namespace ArcGisTileTest
{
    [DataContract]
    public class LevelOfDetail
    {
        [DataMember]
        public int level { get; set; }
        [DataMember]
        public double resolution { get; set; }
        [DataMember]
        public double scale { get; set; }
    }
}
