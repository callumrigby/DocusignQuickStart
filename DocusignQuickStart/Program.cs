using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocusignQuickStart
{
    class Program
    {
        static void Main(string[] args)
        {
            SignatureRequest.SignatureRequest.Execute();
            //EmbeddedSigning.EmbeddedSigning.Main();

            Console.WriteLine("Press and key to continue...");
            Console.ReadKey();
        }
    }
}
