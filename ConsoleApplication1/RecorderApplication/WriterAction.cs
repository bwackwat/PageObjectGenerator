using System.Runtime.Serialization;

namespace ConsoleApplication1
{
    [DataContract]
    public abstract class WriterAction
    {
        public new abstract string ToString();
    }
}