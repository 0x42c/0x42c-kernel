using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diskimg
{
    class Program
    {
        static void Main(string[] args)
        {
            string driverPath = null;
            string rootPath = null;
            string outputPath = null;

            // Parse args
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.StartsWith("-"))
                {
                    switch (arg)
                    {
                        case "--driver":
                            driverPath = args[++i];
                            break;
                        default:
                            Console.WriteLine("Invalid usage. Refer to the manual for help.");
                            return;
                    }
                }
                else
                {
                    if (rootPath == null)
                        rootPath = arg;
                    else if (outputPath == null)
                        outputPath = arg;
                    else
                    {
                        Console.WriteLine("Invalid usage. Refer to the manual for help.");
                        return;
                    }
                }
            }

            if (rootPath == null)
            {
                Console.WriteLine("No root path specified.");
                return;
            }
            if (outputPath == null)
                outputPath = "output.img";
            byte[] driver;
            if (driverPath != null)
                driver = File.ReadAllBytes(driverPath);
        }
    }
}
