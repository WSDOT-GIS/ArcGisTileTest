using System.Runtime.Serialization;
namespace ArcGisTileTest
{
    [DataContract]
    public class Extent
    {
        [DataMember]
        public double xmin { get; set; }
        [DataMember]
        public double ymin { get; set; }
        [DataMember]
        public double xmax { get; set; }
        [DataMember]
        public double ymax { get; set; }
    }
}
