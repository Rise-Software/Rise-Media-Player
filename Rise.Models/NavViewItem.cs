using System.Runtime.Serialization;

namespace Rise.Models
{
    [DataContract]
    public class NavViewItem
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string AccessKey { get; set; }

        [DataMember]
        public string HeaderGroup { get; set; }

        [DataMember]
        public string Icon { get; set; }

        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public bool IsVisible { get; set; }

        [DataMember]
        public int ItemType { get; set; }
    }
}
