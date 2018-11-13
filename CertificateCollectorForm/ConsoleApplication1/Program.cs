using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    using System.IO;
    using System.Reflection;

    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo d = new DirectoryInfo(@"C:\GIT\Pulse\Services\PulseService\src\dependencies\Dealogic Dependencies");

            var list = new Dictionary<string, string>();
            foreach (FileInfo file in d.GetFiles("*.dll"))
            {
                Assembly asm = Assembly.LoadFile(file.FullName);
                Module m = asm.GetModules()[0];
                var cert = m.GetSignerCertificate();

                if (cert != null)
                {
                    list.Add(file.Name, cert.GetExpirationDateString());
                }
                else
                {
                    list.Add(file.Name, "NINCS ALÁÍRVA");
                }

            }

            foreach (var key in list)
            {
                Console.WriteLine(key.Key+":"+key.Value);
            }

            Console.ReadKey();
        }
    }
}
