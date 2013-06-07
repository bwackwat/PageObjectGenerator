namespace ConsoleApplication1
{
    internal class PageObjectAction : WriterAction
    {
        public PageObjectAction(string Name)
        {
            this.Name = Name;
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return "   Start new page object named: " + Name;
        }
    }
}