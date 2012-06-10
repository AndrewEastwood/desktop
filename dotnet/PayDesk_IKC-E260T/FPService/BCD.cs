using System;
using System.Collections.Generic;
using System.Text;

namespace FPService
{
    class BCD
    {
        private byte[] c_BCDNumber;
        private byte[] uc_BCDNumber;

        public BCD(int number)
        {
            string nLine = number.ToString();
            byte i = 0;

            //for uncompressed
            uc_BCDNumber = new byte[nLine.Length];
            for (i = 0; i < uc_BCDNumber.Length; i++)
                uc_BCDNumber[i] = byte.Parse(nLine[i].ToString());



            //for compressed
            if (nLine.Length % 2 == 1)
                nLine = '0' + nLine;
            c_BCDNumber = new byte[nLine.Length / 2];

            byte part1 = 0x00;
            byte part2 = 0x00;

            byte j = 0;
            for (i = 0; i < c_BCDNumber.Length; i++, j += 2)
            {
                part1 = byte.Parse(nLine[j].ToString());
                part1 <<= 4;
                part2 = byte.Parse(nLine[j + 1].ToString());
                c_BCDNumber[i] = (byte)(part1 | part2);
            }

        }

        public static BCD operator +(BCD a, BCD b)
        {
            return new BCD(a.DecimalNumber + b.DecimalNumber);
        }
        public static BCD operator -(BCD a, BCD b)
        {
            return new BCD(a.DecimalNumber + b.DecimalNumber);
        }

        public int DecimalNumber
        {
            get
            {
                int _dnumber = 0;
                int idx = 1;

                for (byte i = 0; i < uc_BCDNumber.Length; i++)
                {
                    idx = (int)Math.Pow(10, (uc_BCDNumber.Length - 1) - i);
                    _dnumber += ((int)uc_BCDNumber[i]) * idx;
                }

                return _dnumber;
            }
        }
        public byte[] CompressedBCD
        { get { return c_BCDNumber; } }
        public byte[] UncompressedBCD
        { get { return uc_BCDNumber; } }
    }
}