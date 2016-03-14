using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;

namespace ovsTakt
{
    class Test
    {
        //List<string> lstSource = new List<string>();
        ArrayList lstSource = new ArrayList();
        public List<int> lstInt = new List<int>();

        public Test(ArrayList TestInput)
        {
            // lstSource.Add("05A3"); //58C
            // lstSource.Add("0005");
            // lstSource.Add("F500");
            // lstSource.Add("F200");
            // lstSource.Add("358E");//
            // lstSource.Add("458D");
            // lstSource.Add("F400");
            // lstSource.Add("F800");
            // lstSource.Add("3009");//
            // lstSource.Add("F200");
            // lstSource.Add("458C");
            // lstSource.Add("300A");
            // lstSource.Add("F200");
            // lstSource.Add("480A");

            // lstSource.Add("F700");//
            // lstSource.Add("F200");
            // lstSource.Add("F400");
            // lstSource.Add("158E");
            // lstSource.Add("F600");//
            // lstSource.Add("358E");
            // lstSource.Add("0009");
            // lstSource.Add("C598");
            // lstSource.Add("F000");//
            // lstSource.Add("B59E");
            // lstSource.Add("E101");
            // lstSource.Add("E301");
            // lstSource.Add("F800");
            // lstSource.Add("A593");

            // lstSource.Add("F200");
            // lstSource.Add("");
            // lstSource.Add("F000");

            lstSource = TestInput;
            GetIntList();
        }

        void GetIntList()
        {
            for (int i = 0; i < lstSource.Count; i++)
            {
                lstInt.Add(HexIntParse((string)lstSource[i]));
            }

        }

        public int HexIntParse(string str)
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
