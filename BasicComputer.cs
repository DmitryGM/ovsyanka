using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ovsTakt
{
    class BasicComputer
    {
        bool Stop = false; //if HLT => stop = true;


        public int[] Memory = new int[2048];

        int RA;
        int RD;
        int RC;

        int PC; //program counter

        int BR; //buffer register
        int A;  //accumulator

        int C
        {
            get
            {
                return Bit(16, A);
            }
            set
            {
                A = A % (int)(1 << 16) + value * (int)(1 << 16);
            }
        }

        public BasicComputer()
        {
            RA = 0;
            RD = 0;
            RC = 0;

            PC = 0;

            BR = 0;
            A = 0;
        }

        public void StartComputer(int PC)
        {
            this.PC = PC;

            StartComputer();
        }

        public void StartComputer()
        {
            while (!Stop)
            {
                CycleInstructionFetch();

                //Можно посмотреть, что получилось 

                Console.WriteLine("PC: " + GetHex(3, PC) + "\t\tRA: " + GetHex(3, RA) + "\t\tRC: " + GetHex(4, RC) + "\tRD: " + GetHex(4, RD) + "\tA: " + GetHex(4, A) + "\t\tC: " + C);
                //Console.ReadLine();

            }
        }

        public string GetHex(int k, int N)
        {
            string res = "";

            for (int i = 0; i < k; i++)
            {
                int t = N % 16;
                N /= 16;

                if(t < 10) res += t;

                switch(t)
                {
                    case 10:
                    {
                        res += "A";
                        break;
                    }
                    case 11:
                    {
                        res += "B";
                        break;
                    }
                    case 12:
                    {
                        res += "C";
                        break;
                    }
                    case 13:
                    {
                        res += "D";
                        break;
                    }
                    case 14:
                    {
                        res += "E";
                        break;
                    }
                    case 15:
                    {
                        res += "F";
                        break;
                    }

                }
                // if(t == 10) res += "A";
                // if(t == 11) res += "B";
                // if(t == 12) res += "C";
                // if(t == 13) res += "D";
                // if(t == 14) res += "E";
                // if(t == 15) res += "F";
            }

            string resRev = "";

            for (int i = 0; i < k; i++) //Переворот (реверс) строки
            {
                resRev += res[k - 1 - i];
            }

            return resRev;
        }


        //Цикл выборки команды + контракт
        void CycleInstructionFetch()
        {
            BR = PC; //01
            RA = BR; //02

            RD = Memory[RA]; //03
            BR = PC + 1;

            PC = BR; //04

            BR = RD; //05
            RC = BR; //06

            //Определение типа команды

            //Если безадресная => Перейти
            if (Bit(15, RC) == 1 && Bit(14, RC) == 1 && Bit(13, RC) == 1 && Bit(12, RC) == 1)
            {
                CycleExecutionAddresslessInstruction();

                return;
            }

            //Определение вида адресации
            if (Bit(11, RC) == 1)
                CycleSamplingOperandAddress();

            CycleExecutionAddressInstruction();

        }

        //Цикл выборки адреса операнда
        void CycleSamplingOperandAddress()
        {
            BR = RD; //0D
            BR %= (int)(1 << 11);

            RA = BR; //0E
            RD = Memory[RA]; //0F

            //Для индексных ячеек
            if (
                Bit(3, RC) == 1 &&
                Bit(4, RC) == 0 &&
                Bit(5, RC) == 0 &&
                Bit(6, RC) == 0 &&
                Bit(7, RC) == 0 &&
                Bit(8, RC) == 0 &&
                Bit(9, RC) == 0 &&
                Bit(10, RC) == 0)
            {
                BR = RD+1; //18
                BR %= (int)(1 << 17); //На всякий-всякий)

                RD = BR; //19
                Memory[RA] = RD; //1A
                BR = RD - 1; //1B ! - не защищенное место
                RD = BR; //1
            }

        }

        //Цикл исполнения адресных команд
        void CycleExecutionAddressInstruction()
        {
            //Декодирование адресных команд

            if (Bit(15, RC) == 1) //1D
            {
                //=> Команды перехода

                if (Bit(14, RC) == 0) //2D
                {
                    if (Bit(13, RC) == 0) //30
                    {
                        if (Bit(12, RC) == 0) //33
                        {
                            BCS();
                        }
                        else //34
                        {
                            BPL();
                        }
                    }
                    else //31
                    {
                        if (Bit(12, RC) == 0)
                        {
                            BMI();
                        }
                        else //32
                        {
                            BEQ();
                        }
                    }
                }
                else //2E
                {
                    if (Bit(12, RC) == 0)
                    {
                        BReak();
                    }
                    else //2F
                    {
                        //GO Р-П
                    }
                }
            }
            else //1E
            {
                //Остальные адр. команды
                BR = RD;
                BR %= (int)(1 << 11); //Младшие 11 бит

                RA = BR; //1F

                if (Bit(14, RC) == 1) //20
                {
                    RD = Memory[RA]; //27

                    if (Bit(13, RC) == 0) //28
                    {
                        if (Bit(12, RC) == 0) //2B
                        {
                            ADD();
                        }
                        else //2C
                        {
                            ADC();
                        }
                    }
                    else //29
                    {
                        if (Bit(12, RC) == 0)
                        {
                            SUB();
                        }
                        else //2A
                        {
                            //GOTO Р - А
                            //Арифметическая команда 7###
                        }
                    }
                }
                else //21
                {
                    if (Bit(13, RC) == 0)
                    {
                        RD = Memory[RA]; //24

                        if (Bit(12, RC) == 0) //25
                        {
                            ISZ();
                        }
                        else //26
                        {
                            AND();
                        }
                    }
                    else //22
                    {
                        if (Bit(12, RC) == 0)
                        {
                            JSR();
                        }
                        else //23
                        {
                            MOV();
                        }
                    }
                }
            }

            //Исполнение адресных команд
        }

        void AND()
        {
            BR = RD & A; //35
            BR %= (int)(1<<16); //Берем младшие 16 бит

            A = C * (int)(1 << 16) + BR; //36
        }

        void MOV()
        {
            BR = A; //38
            RD = BR; //39
            Memory[RA] = RD; //3A
        }

        void ADD()
        {
            BR = A + RD; //3C
            BR %= (int)Math.Pow(2, 17); //Берем младшие 17 бит

            A = BR; //3D
        }

        void ADC()
        {
            if (Bit(0, C) == 0) ADD(); //3F

            BR = A + RD + 1; //40
            BR %= (int)Math.Pow(2, 17); //Берем младшие 17 бит

            A = BR; //41
        }

        void SUB()
        {
            BR = A + COM(RD) + 1; //43
            BR %= (int)Math.Pow(2, 17); //Берем младшие 17 бит

            A = BR; //44
        }

        void BReak()
        {
            BR = RD; //47
            BR %= (int)Math.Pow(2, 11); //Берем младшие 11 бит

            PC = BR; //48 
        }

        void BCS()
        {
            if (Bit(0, C) == 1) //46
            {
                BReak();
            }
        }

        void BPL()
        {
            if (Bit(A, 15) == 0) //4A
            {
                BReak(); //4B
            }
        }

        void BMI()
        {
            if (Bit(A, 15) == 1) //4C
            {
                BReak(); //4D
            }
        }

        void BEQ()
        {
            bool zero = true;

            for (int i = 0; i < 16; i++)
            {
                zero = zero && Bit(i, A) == 0;
            }

            if (zero) BReak(); //4E, 4F
        }

        void ISZ()
        {
            BR = RD + 1; //50
            BR %= (int)(1 << 16); //Берем младшие 16 бит

            RD = BR; //51
            Memory[RA] = RD; //52

            if (Bit(15, RD) == 0) //53
            {
                BR = PC + 1; //54
                PC = BR; //55
            }
        }

        void JSR()
        {
            BR = RD + 1; //57
            BR %= (int)Math.Pow(2, 16); //Берем младшие 16 бит

            RC = BR; //58
            BR = PC; //59
            RD = BR; //5A

            RD = Memory[RA]; //5B
            BR = RC;

            PC = BR; //5C

        }

        //Цикл исполнения безадресных команд
        void CycleExecutionAddresslessInstruction()
        {
            //Декодирование безадресных команд
            if (Bit(11, RC) == 0) //5E
            {
                if (Bit(10, RC) == 0) //61
                {
                    if (Bit(9, RC) == 0) //67
                    {
                        if (Bit(8, RC) == 0) //6A
                        {
                            HLT();
                        }
                        else
                        {
                            //NOP
                        }
                    }
                    else
                    {
                        if (Bit(8, RC) == 0) //68
                        {
                            CLA();
                        }
                        else //69
                        {
                            CLC();
                        }
                    }
                }
                else //62
                {
                    if (Bit(9, RC) == 0)
                    {
                        if (Bit(8, RC) == 0) //65
                        {
                            CMA();
                        }
                        else //66
                        {
                            CMC();
                        }
                    }
                    else //63
                    {
                        if (Bit(8, RC) == 0)
                        {
                            ROL();
                        }
                        else //64
                        {
                            ROR();
                        }
                    }
                }
            }
            else //5F
            {
                if (Bit(10, RC) == 0) //5F
                {
                    if (Bit(9, RC) == 0) //6C
                    {
                        if (Bit(8, RC) == 0) //6F
                        {
                            INC();
                        }
                    }
                    else //6D
                    {
                        if (Bit(8, RC) == 0)
                        {
                            //EI
                        }
                        else //6E
                        {
                            //DI
                        }
                    }
                }
                else //60
                {
                    //GOTO Р-Б
                    //Безадресные команды FC## - FF##
                }
            }

            //Исполнение безадресных команд (см. методы ниже)
        }

        void DEC()
        {
            BR = A + COM(0); //70
            BR %= (int)Math.Pow(2, 17); //Берем младшие 17 бит

            A = BR; //71
        }

        void INC()
        {
            BR = A + 1; //73
            BR %= (int)Math.Pow(2, 17); //Берем младшие 17 бит

            A = BR; //74
        }

        void CLA()
        {
            //Очистка акумулятор
            
            BR = 0; //76

            //A = BR //77:
            A = C * (int)(1<<16);
        }

        void CLC()
        {
            C = 0; //79
        }

        void CMA()
        {
            //Инверсия А

            BR = COM(A); //7B
            BR %= (int)(1 << 16); //На всякий

            A = C * (int)(1 << 16) + BR; //7C
        }

        void CMC()
        {
            //Инверсия бита C //7E
            if (C == 0)
                C = 1;
            else
                C = 0;
        }

        void ROL()
        {
            BR = (int)(A << 1); //82
            BR += Bit(16, A);
            BR %= (int)(1 << 17);

            A = BR; //83
        }

        void ROR()
        {
            BR = (int)(A >> 1); //85
            BR += (1 << 16) * Bit(0, A);
            BR %= (int)(1 << 17);

            A = BR; //86
        }

        void HLT()
        {
            Stop = true;
        }
        
        //Не реализованы:
        /*
         * NOP
         * EI - разрешение прерывания
         * DI - запрещение прерывания
         * 
         * В\В
         * Всякие прерывания
         * Пультовые операции
         */

        //Подметоды:

        //void SKP //54 - пока обошёлся без него

        int COM(int n)
        {
            //Only 16 (bit)
            n %= (int)(1 << 16);

            return ((int)(1 << 16) - 1 - n) % (int)(1 << 16);
        }

        //+контракт
        int Bit(int i, int n)
        {
            return (n >> i) % 2;
        }

    }
}
