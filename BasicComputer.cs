using System;

namespace ovsTakt
{
    class BasicComputer
    {
        bool Stop = false; //if HLT => stop = true;

        public int[] Memory = new int[2048];

        int RA;
        int RD;
        int RI; // instruction register

        int PC; // program counter

        int BR; // buffer register
        int A;  // accumulator

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
            RI = 0;

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

                Console.WriteLine("PC: " + GetHex(3, PC) + "\t\tRA: " + GetHex(3, RA) + "\t\tRI: " + GetHex(4, RI) + "\tRD: " + GetHex(4, RD) + "\tA: " + GetHex(4, A) + "\t\tC: " + C);
            }
        }

        public string GetHex(int k, int N)
        {
            char[] res = new char[k];

            for (int i = 0; i < k; i++)
            {
                int t = N % 16;
                N /= 16;

                if(t < 10) res[i] = t.ToString()[0];

                if(t == 10) res[i] = 'A';
                if(t == 11) res[i] = 'B';
                if(t == 12) res[i] = 'C';
                if(t == 13) res[i] = 'D';
                if(t == 14) res[i] = 'E';
                if(t == 15) res[i] = 'F';
            }

            Array.Reverse(res);
            return new string(res);
        }

        void CycleInstructionFetch()
        {
            BR = PC; //01
            RA = BR; //02

            RD = Memory[RA]; //03
            BR = PC + 1;

            PC = BR; //04

            BR = RD; //05
            RI = BR; //06

            // Identifying the type of instruction

            // If there is no address => GoTo CycleExecutionAddresslessInstruction
            if (Bit(15, RI) == 1 && Bit(14, RI) == 1 && Bit(13, RI) == 1 && Bit(12, RI) == 1)
            {
                CycleExecutionAddresslessInstruction();

                return;
            }

            // Identifying the type of addressing
            if (Bit(11, RI) == 1)
                CycleSamplingOperandAddress();

            CycleExecutionAddressInstruction();

        }

        void CycleSamplingOperandAddress()
        {
            BR = RD; //0D
            BR %= (int)(1 << 11);

            RA = BR; //0E
            RD = Memory[RA]; //0F

            // For index cells
            if (
                Bit(3, RI) == 1 &&
                Bit(4, RI) == 0 &&
                Bit(5, RI) == 0 &&
                Bit(6, RI) == 0 &&
                Bit(7, RI) == 0 &&
                Bit(8, RI) == 0 &&
                Bit(9, RI) == 0 &&
                Bit(10, RI) == 0)
            {
                BR = RD+1; //18
                BR %= (int)(1 << 17);

                RD = BR; //19
                Memory[RA] = RD; //1A
                BR = RD - 1; //1B ! - unprotected place!
                RD = BR; //1
            }

        }

        // Precondition: RD contains the address of the operand
        void CycleExecutionAddressInstruction()
        {
            // Decoding of address instructions

            if (Bit(15, RI) == 1) //1D
            {
                // Jump instructions

                if (Bit(14, RI) == 0) //2D
                {
                    if (Bit(13, RI) == 0) //30
                    {
                        if (Bit(12, RI) == 0) //33
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
                        if (Bit(12, RI) == 0)
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
                    if (Bit(12, RI) == 0)
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
                // Other address instructions

                BR = RD;
                BR %= (int)(1 << 11); //Low-order 11 bits

                RA = BR; //1F

                if (Bit(14, RI) == 1) //20
                {
                    RD = Memory[RA]; //27

                    if (Bit(13, RI) == 0) //28
                    {
                        if (Bit(12, RI) == 0) //2B
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
                        if (Bit(12, RI) == 0)
                        {
                            SUB();
                        }
                        else //2A
                        {
                            // GOTO Р - А
                            // Arithmetic instruction 7###
                        }
                    }
                }
                else //21
                {
                    if (Bit(13, RI) == 0)
                    {
                        RD = Memory[RA]; //24

                        if (Bit(12, RI) == 0) //25
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
                        if (Bit(12, RI) == 0)
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
        }

        //Execution of address instructions:

        void AND()
        {
            BR = RD & A; //35
            BR %= (int)(1<<16); // Low-order 16 bits

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
            BR %= (int)Math.Pow(2, 17); // Low-order 17 bits

            A = BR; //3D
        }

        void ADC()
        {
            if (Bit(0, C) == 0) ADD(); //3F

            BR = A + RD + 1; //40
            BR %= (int)Math.Pow(2, 17); // Low-order 17 bits

            A = BR; //41
        }

        void SUB()
        {
            BR = A + COM(RD) + 1; //43
            BR %= (int)Math.Pow(2, 17); // Low-order 17 bits

            A = BR; //44
        }

        void BReak()
        {
            BR = RD; //47
            BR %= (int)Math.Pow(2, 11); // Low-order 11 bits

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
            BR %= (int)(1 << 16); // Low-order 16 bits

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
            BR %= (int)(1<<16); // Low-order 16 bits 

            RI = BR; //58 (the address of the next instruction is put in the RI)

            // Save the return address
            BR = PC; //59
            RD = BR; //5A
            Memory[RA] = RD; //5B

            // Save the address of the next command
            BR = RI;
            PC = BR; //5C
            PC = BR % (int)(1<<11);
        }

        void CycleExecutionAddresslessInstruction()
        {
            // Decoding of addressless instruction
            if (Bit(11, RI) == 0) //5E
            {
                if (Bit(10, RI) == 0) //61
                {
                    if (Bit(9, RI) == 0) //67
                    {
                        if (Bit(8, RI) == 0) //6A
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
                        if (Bit(8, RI) == 0) //68
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
                    if (Bit(9, RI) == 0)
                    {
                        if (Bit(8, RI) == 0) //65
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
                        if (Bit(8, RI) == 0)
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
                if (Bit(10, RI) == 0) //5F
                {
                    if (Bit(9, RI) == 0) //6C
                    {
                        if (Bit(8, RI) == 0) //6F
                        {
                            INC();
                        }
                    }
                    else //6D
                    {
                        if (Bit(8, RI) == 0)
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
                    // GOTO Р-Б
                    // Addressless instructions FC## - FF##
                }
            }
        }

        // Executing addressless instructions:

        void DEC()
        {
            BR = A + COM(0); //70
            BR %= (int)(1<<17); // Low-order 17 bits

            A = BR; //71
        }

        void INC()
        {
            BR = A + 1; //73
            BR %= (int)(1<<17); // Low-order 17 bits

            A = BR; //74
        }

        void CLA()
        {
            // Clear buffer register

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
            //Inversion А

            BR = COM(A); //7B
            BR %= (int)(1 << 16);

            A = C * (int)(1 << 16) + BR; //7C
        }

        void CMC()
        {
            //Inversion C //7E
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

        // Not implemented:
        /*
         * NOP - no operation
         * EI - interrupt enable
         * DI - interrupt disable
         * 
         * I/O
         * Any interruptions
         * Remote controller operations
         */

        //Sub-methods:

        int COM(int n)
        {
            // Only 16 bit
            n %= (int)(1 << 16);

            return ((int)(1 << 16) - 1 - n) % (int)(1 << 16);
        }

        int Bit(int i, int n)
        {
            return (n >> i) % 2;
        }
    }
}
