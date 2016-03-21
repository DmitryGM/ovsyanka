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
            BasicComputer bc     = new BasicComputer();

            int MemoryOffset     = 0;
            int WriteAddr        = 0x000;
            int StartAtAdress    = 0x2BD;


            // Open the file to read from.
            string path = @"/Users/vsafonov/Documents/Progs/Dmitry/ovsyanka/TestAssembly";
            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    AssembleAndLoad(s, ref bc,ref WriteAddr,ref MemoryOffset);
                }
            }
            
            
            
            

            Console.ReadLine();
                        // for (int i = 0; i < test.lstInt.Count; i++)
            // {
                
                
            //     //Console.WriteLine(bc.getHex(4, test.lstInt[i]));

            // }

            bc.StartComputer(StartAtAdress);
            Console.WriteLine("ANSWER: {0}" ,bc.Memory[LoadTest.HexIntParse("2D4")]);
        }


        // FIXME: Does not work with non-direct adressing 
        private static void AssembleAndLoad(string input, ref BasicComputer BasicComp_ref,ref int WriteAtAddr,ref int MemOffset)
        {

            int i         = 0;
            string Word   ="";
            string Addr   ="";


            while(input[i] != ' ')
            {
                Console.WriteLine("DEBUG i:{0}",i);
                Word = Word + input[i];
                i++;
                if(i == input.Length)
                    break;

            }
            i++;
            while(i < input.Length)
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
                    LoadTest.LoadHexCode("F200",ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
                case "INC":
                {
                    LoadTest.LoadHexCode("F800",ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
                case "BR":
                {
                    LoadTest.LoadHexCode("C" + Addr,ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
                case "ADD":
                {
                    LoadTest.LoadHexCode("4" + Addr,ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
                case "HLT":
                {
                    LoadTest.LoadHexCode("F000",ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
                case "JSR":
                {
                    LoadTest.LoadHexCode("2" + Addr,ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
                case "ORG":
                {
                    MemOffset = 0;
                    WriteAtAddr = LoadTest.HexIntParse(Addr);
                    break;
                }
                case "WORD":
                {
                    LoadTest.LoadHexCode(Addr,ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
                case "BMI":
                {
                    LoadTest.LoadHexCode("A" + Addr,ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
                case "BPL":
                {
                    LoadTest.LoadHexCode("9" + Addr,ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
                case "ROL":
                {
                    LoadTest.LoadHexCode("F600",ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
                case "CLC":
                {
                    LoadTest.LoadHexCode("F300",ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
                case "MOV":
                {
                    LoadTest.LoadHexCode("3" + Addr,ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
                case "SUB":
                {
                    LoadTest.LoadHexCode("6" + Addr,ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
                case "DEC":
                {
                    LoadTest.LoadHexCode("F900",ref BasicComp_ref, WriteAtAddr + MemOffset);
                    MemOffset++;
                    break;
                }
            }
        
        }
    }
}
