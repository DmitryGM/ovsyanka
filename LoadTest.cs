using System;

namespace ovsTakt
{
    class LoadTest
    {
        static public void LoadHexCode(string TestInput, ref BasicComputer BasicComp_ref, int WriteAtAddr)
        {
            Console.WriteLine("DEBUG2: HEX - {0}, Addr - {1}",TestInput,WriteAtAddr);   
            BasicComp_ref.Memory[WriteAtAddr] = HexIntParse(TestInput);

        }

        static public int HexIntParse(string str)
        {
            Console.WriteLine(str);
            int res = 0;

            for (int i = 0; i < str.Length; i++)
            {
                res *= 16;

                if (str[i] == '1') res += 1;
                if (str[i] == '2') res += 2;
                if (str[i] == '3') res += 3;
                if (str[i] == '4') res += 4;
                if (str[i] == '5') res += 5;
                if (str[i] == '6') res += 6;
                if (str[i] == '7') res += 7;
                if (str[i] == '8') res += 8;
                if (str[i] == '9') res += 9;
                if (str[i] == 'A') res += 10;
                if (str[i] == 'B') res += 11;
                if (str[i] == 'C') res += 12;
                if (str[i] == 'D') res += 13;
                if (str[i] == 'E') res += 14;
                if (str[i] == 'F') res += 15;
            }

            return res;
        }
    }
}
