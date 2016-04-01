using System.Runtime.Serialization;

namespace ArcGisTileTest
{
    [DataContract]
    public class Layer
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public bool defaultVisibility { get; set; }
        [DataMember]
        public int[] subLayerIds { get; set; }
        [DataMember]
        public float minScale { get; set; }
        [DataMember]
        public float maxScale { get; set; }
    }
}
