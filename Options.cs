using System.Runtime.Serialization;
namespace ArcGisTileTest
{
    [DataContract]
    public class Options
    {
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string OutputDirectory { get; set; }
        [DataMember]
        public int? StartLevel { get; set; }
        [DataMember]
        public int? StartRow { get; set; }
        [DataMember]
        public int? EndLevel { get; set; }
        [DataMember]
        public int? EndRow { get; set; }
        [DataMember]
        public int? MaxDegreeOfParallelism { get; set; }
        [DataMember]
        public bool? WriteErrorsOnly { get; set; }
        [DataMember]
        public int? MinimumValidContentLength { get; set; }
    }
}
