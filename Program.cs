using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace ovsTakt
{
    class Program
    {
        static void Main(string[] args)
        {
            BasicComputer bc = new BasicComputer();
            ArrayList TestInput = new ArrayList();

            // Open the file to read from.
            string path = @"/Users/vsafonov/Documents/Progs/Dmitry/ovsyanka/TestAssembly";
            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                    Assembler(ref TestInput,s);
                }
            }
            
            Test test = new Test(TestInput);
            int StartAtAdress = test.HexIntParse("200");

            Console.ReadLine();

            for (int i = 0; i < test.lstInt.Count; i++)
            {
                
                bc.Memory[i + StartAtAdress] = test.lstInt[i];

                //Console.WriteLine(bc.getHex(4, test.lstInt[i]));
            }

            bc.StartComputer(StartAtAdress);
        }


        // TODO: Does not work with non-direct adressing 
        private static void Assembler(ref ArrayList list, string input)
        {

            int i = 0;
            string Word ="";
            string Addr ="";



                while(input[i] != ' ')
                {
                    Console.WriteLine("DEBUG i:{0}",i);
                    Word = Word + input[i];
                    i++;
                    if(i == input.Length)
                        break;
                }
                while(i != input.Length)
                {
                    Console.WriteLine("DEBUG i:{0}",i);
                    Addr = Addr + input[i];
                    i++;
                }
                Console.WriteLine("DEBUG Word,Addr:{0};{1}",Word, Addr);


                switch(Word)
                {
                    case "CLA":
                    {
                        list.Add("F200");
                        break;
                    }
                    case "ADD":
                    {
                        list.Add("8" + Addr);
                        break;
                    }
                    case "HLT":
                    {
                        list.Add("F000");
                        break;
                    }
                }


            // while(list[i] != ',')
            // {
                
            //     Word = Word + list[i];
                
            // }
            // switch(Word)
            // {
            //     case 
            // }

        
        }
    }
}
