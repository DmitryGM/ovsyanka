using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ovsTakt
{
    class Program
    {
        static void Main(string[] args)
        {
            BasicComputer bc = new BasicComputer();

            Test test = new Test();

            Console.ReadLine();

            for (int i = 0; i < test.lstInt.Count; i++)
            {
                //58C

                bc.Memory[i + 0x58C] = test.lstInt[i];

                //Console.WriteLine(bc.getHex(4, test.lstInt[i]));
            }

            bc.StartComputer(0x58F);
        }
    }
}
