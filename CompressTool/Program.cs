using System.Diagnostics;
using System.Reflection;

namespace CompressTool
{
    internal class Program
    {
        //[STAThread]
        //static void Main(string[] args)
        //{
        //    var app = new App();
        //    app.Run();
        //}

        private static void InteralCW(object value)
        {
            Console.WriteLine(value);
            Thread.Sleep(1000);
            Console.Clear();
        }
    }
}
