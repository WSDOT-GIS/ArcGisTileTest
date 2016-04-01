using System.Runtime.Serialization;
namespace ArcGisTileTest
{
    [DataContract]
    public class Point
    {
        [DataMember]
        public double x { get; set; }
        [DataMember]
        public double y { get; set; }
    }
}
