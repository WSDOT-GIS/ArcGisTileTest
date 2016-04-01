using System.Runtime.Serialization;

namespace ArcGisTileTest
{
    [DataContract]
    public class Tile
    {
        [DataMember]
        public int Row { get; set; }
        [DataMember]
        public int Column { get; set; }
    }
}
