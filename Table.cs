using System.Runtime.Serialization;
namespace ArcGisTileTest
{
    [DataContract]
    public class Table
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string name { get; set; }
    }
}
