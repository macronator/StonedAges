using System;
using System.IO;

namespace Engine;

public class HPFCompression
{
    public static byte[] Decompress(string file)
    {
        uint num = 7u;
        uint num2 = 0u;
        uint num3 = 0u;
        uint num4 = 0u;
        byte[] array = File.ReadAllBytes(file);
        byte[] array2 = new byte[array.Length * 10];
        uint[] array3 = new uint[256];
        uint[] array4 = new uint[256];
        byte[] array5 = new byte[513];
        for (uint num5 = 0u; num5 < 256; num5++)
        {
            array3[num5] = 2 * num5 + 1;
            array4[num5] = 2 * num5 + 2;
            array5[num5 * 2 + 1] = (byte)num5;
            array5[num5 * 2 + 2] = (byte)num5;
        }
        while (num2 != 256)
        {
            uint num6;
            for (num6 = 0u; num6 <= 255; num6 = (((array[4 + num3 - 1] & (1 << (int)num)) == 0) ? array3[num6] : array4[num6]))
            {
                if (num == 7)
                {
                    num3++;
                    num = 0u;
                }
                else
                {
                    num++;
                }
            }
            uint num7 = num6;
            uint num8 = array5[num6];
            while (num7 != 0 && num8 != 0)
            {
                uint num9 = array5[num8];
                uint num10 = array3[num9];
                if (num10 == num8)
                {
                    num10 = array4[num9];
                    array4[num9] = num7;
                }
                else
                {
                    array3[num9] = num7;
                }
                if (array3[num8] == num7)
                {
                    array3[num8] = num10;
                }
                else
                {
                    array4[num8] = num10;
                }
                array5[num7] = (byte)num9;
                array5[num10] = (byte)num8;
                num7 = num9;
                num8 = array5[num7];
            }
            num2 = num6 + 4294967040u;
            if (num2 != 256)
            {
                array2[num4] = (byte)num2;
                num4++;
            }
        }
        byte[] array6 = new byte[num4];
        Buffer.BlockCopy(array2, 0, array6, 0, (int)num4);
        return array6;
    }

    public static byte[] Decompress(byte[] hpfBytes)
    {
        uint num = 7u;
        uint num2 = 0u;
        uint num3 = 0u;
        uint num4 = 0u;
        byte[] array = new byte[hpfBytes.Length * 10];
        uint[] array2 = new uint[256];
        uint[] array3 = new uint[256];
        byte[] array4 = new byte[513];
        for (uint num5 = 0u; num5 < 256; num5++)
        {
            array2[num5] = 2 * num5 + 1;
            array3[num5] = 2 * num5 + 2;
            array4[num5 * 2 + 1] = (byte)num5;
            array4[num5 * 2 + 2] = (byte)num5;
        }
        while (num2 != 256)
        {
            uint num6;
            for (num6 = 0u; num6 <= 255; num6 = (((hpfBytes[4 + num3 - 1] & (1 << (int)num)) == 0) ? array2[num6] : array3[num6]))
            {
                if (num == 7)
                {
                    num3++;
                    num = 0u;
                }
                else
                {
                    num++;
                }
            }
            uint num7 = num6;
            uint num8 = array4[num6];
            while (num7 != 0 && num8 != 0)
            {
                uint num9 = array4[num8];
                uint num10 = array2[num9];
                if (num10 == num8)
                {
                    num10 = array3[num9];
                    array3[num9] = num7;
                }
                else
                {
                    array2[num9] = num7;
                }
                if (array2[num8] == num7)
                {
                    array2[num8] = num10;
                }
                else
                {
                    array3[num8] = num10;
                }
                array4[num7] = (byte)num9;
                array4[num10] = (byte)num8;
                num7 = num9;
                num8 = array4[num7];
            }
            num2 = num6 + 4294967040u;
            if (num2 != 256)
            {
                array[num4] = (byte)num2;
                num4++;
            }
        }
        byte[] array5 = new byte[num4];
        Buffer.BlockCopy(array, 0, array5, 0, (int)num4);
        return array5;
    }
}
