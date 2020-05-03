using System;

namespace app
{
    class Program
    {
        static void Main(string[] args)
        {
            DataParser dataParser = new DataParser();
            var parsedData = dataParser.ParseData();
        }
    }
}
