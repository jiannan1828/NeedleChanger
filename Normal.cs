using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectorInspector
{
    internal class Normal
    {
        //Example
        //static void Main()
        //{
        // 測試數值
        //    int inputValue = 500000; // 可以改成任何你想測試的數值
        //    double mappedValue = Map(inputValue, -1048576, 1048576, -500.000, 500.000);
        //    Console.WriteLine($"Mapped Value: {mappedValue}");
        //}

        // 映射函數
        public double Map(int Input, int in_min, int in_max, double out_min, double out_max)
        {
            // 計算映射
            return (double)(Input - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        // 轉換方法
        public string ParseToBinaryString(ref byte[] BInput, int isize)
        {
            var sb = new StringBuilder();

            for (int i = isize - 1; i >= 0; i--)
            {
                byte[] data = new byte[1] { BInput[i] };
                sb.AppendFormat("{0} ", data.ToBinary());
            }

            return sb.ToString();
        }

        public string ParseToHexBinaryString(ref byte[] BInput, int isize)
        {
            var sb = new StringBuilder();

            for (int i = isize - 1; i >= 0; i--)
            {
                byte[] data = new byte[1] { BInput[i] };
                sb.AppendFormat("{0}:{1} ", data.ToHex(), data.ToBinary());
            }

            return sb.ToString();
        }

    }
}
