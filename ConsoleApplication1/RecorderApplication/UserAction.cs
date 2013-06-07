using System.Runtime.Serialization;

namespace ConsoleApplication1
{
    [DataContract]
    public class UserAction : WriterAction
    {
        [DataMember(Name = "Name", IsRequired = true)]
        public string Name { get; set; }

        [DataMember(Name = "Page", IsRequired = true)]
        public string Page { get; set; }

        [DataMember(Name = "Node", IsRequired = true)]
        public string Node { get; set; }

        [DataMember(Name = "Type", IsRequired = true)]
        public string Type { get; set; }

        [DataMember(Name = "Path", IsRequired = true)]
        public string Path { get; set; }

        [DataMember(Name = "Text", IsRequired = true)]
        public string Text { get; set; }

        public override string ToString()
        {
            return Node + "(" + Type + "): \"" + Text + "\" @ " + Path;
        }
    }
}