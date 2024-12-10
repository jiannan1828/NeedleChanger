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
    }
}
