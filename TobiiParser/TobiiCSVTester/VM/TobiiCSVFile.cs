namespace TobiiCSVTester.VM
{
    public class TobiiCSVFile
    {
        public TobiiCSVFile(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

    }
}