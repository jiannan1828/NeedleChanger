﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectorInspector
{
    internal class Normal
    {



        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        const double EPSILON = 1e-6;

        public struct Point {
            public double X;
            public double Y;

            public Point(double x, double y) {
                X = x;
                Y = y;
            }
        }

        public class MX {
            public double[,] Matrix { get; set; } = new double[3, 3];
        }
        //---------------------------------------------------------------------------------------
        //Example
        //public static void Main(string[] args) {
        //    Normal calculate = new Normal();
        //
        //    // 定義 PointA, PointB 的數據
        //    Normal.Point idealA = new Normal.Point(387.62823,  93.82427);
        //    Normal.Point idealB = new Normal.Point(419.62823, 107.82427);
        //    Normal.Point realA  = new Normal.Point(145.680,   616.221  );
        //    Normal.Point realB  = new Normal.Point(113.708,   602.094  );
        //
        //    // 宣告 PointForward 和 PointBackward 變數
        //    Normal.Point idealAForward  = new Normal.Point();
        //    Normal.Point idealABackward = new Normal.Point();
        //    Normal.Point realAForward   = new Normal.Point();
        //    Normal.Point realABackward  = new Normal.Point();
        //
        //    // 呼叫計算並傳遞相應的點作為參數
        //    CalculateAndPrintPlotData(idealA, idealB, out idealAForward, out idealABackward);
        //    CalculateAndPrintPlotData(realA, realB, out realAForward, out realABackward);
        //
        //
        //    // 計算PerspectiveTransform
        //    MX matrix = new MX();
        //
        //    double[,] idealCoords = { { idealA.X,         idealA.Y },
        //                              { idealAForward.X,  idealAForward.Y },
        //                              { idealB.X,         idealB.Y },
        //                              { idealABackward.X, idealABackward.Y } };
        //
        //    double[,] realCoords  = { { realA.X,         realA.Y },
        //                              { realAForward.X,  realAForward.Y },
        //                              { realB.X,         realB.Y },
        //                              { realABackward.X, realABackward.Y } };
        //
        //    ComputePerspectiveTransform(idealCoords, realCoords, matrix);
        //
        //    Console.WriteLine("透視變換矩陣:");
        //    for (int i = 0; i < 3; i++) {
        //        for (int j = 0; j < 3; j++) {
        //            Console.Write($"\t{matrix.Matrix[i, j]:F6} ");
        //        }
        //        Console.WriteLine();
        //    }
        //
        //
        //    // 求得映射轉換座標
        //    double X_In = idealA.X,
        //           Y_In = idealA.Y;
        //    Normal.Point pMapping = MapToCoords(matrix, X_In, Y_In);
        //    Console.WriteLine($"輸入座標 ({idealA.X:F2}, {idealA.Y:F2}) 映射到真實座標 ({pMapping.X:F2}, {pMapping.Y:F2})");
        //}
        //---------------------------------------------------------------------------------------
        public static void ComputePerspectiveTransform(double[,] idealCoords, double[,] realCoords, MX matrix) {
            double[,] A = new double[8, 8];
            double[] b = new double[8];
            double[] x = new double[8];

            for (int i = 0; i < 4; i++) {
                double XIdeal = idealCoords[i, 0];
                double YIdeal = idealCoords[i, 1];
                double xReal = realCoords[i, 0];
                double yReal = realCoords[i, 1];

                A[i, 0] = XIdeal;
                A[i, 1] = YIdeal;
                A[i, 2] = 1.0;
                A[i, 6] = -xReal * XIdeal;
                A[i, 7] = -xReal * YIdeal;
                b[i] = xReal;

                A[i + 4, 3] = XIdeal;
                A[i + 4, 4] = YIdeal;
                A[i + 4, 5] = 1.0;
                A[i + 4, 6] = -yReal * XIdeal;
                A[i + 4, 7] = -yReal * YIdeal;
                b[i + 4] = yReal;
            }

            // Gaussian elimination
            for (int i = 0; i < 8; i++) {
                int maxRow = i;
                for (int j = i + 1; j < 8; j++) {
                    if (Math.Abs(A[j, i]) > Math.Abs(A[maxRow, i])) {
                        maxRow = j;
                    }
                }

                // Swap rows
                for (int k = 0; k < 8; k++) {
                    double temp = A[i, k];
                    A[i, k] = A[maxRow, k];
                    A[maxRow, k] = temp;
                }

                double tempB = b[i];
                b[i] = b[maxRow];
                b[maxRow] = tempB;

                if (Math.Abs(A[i, i]) < EPSILON) {
                    Console.WriteLine("矩陣奇異，無法計算透視變換矩陣");
                    return;
                }

                // Eliminate
                for (int j = i + 1; j < 8; j++) {
                    double factor = A[j, i] / A[i, i];
                    for (int k = i; k < 8; k++)
                    {
                        A[j, k] -= factor * A[i, k];
                    }
                    b[j] -= factor * b[i];
                }
            }

            // Back substitution
            for (int i = 7; i >= 0; i--) {
                x[i] = b[i];
                for (int j = i + 1; j < 8; j++) {
                    x[i] -= A[i, j] * x[j];
                }
                x[i] /= A[i, i];
            }

            // Fill matrix
            matrix.Matrix[0, 0] = x[0];
            matrix.Matrix[0, 1] = x[1];
            matrix.Matrix[0, 2] = x[2];
            matrix.Matrix[1, 0] = x[3];
            matrix.Matrix[1, 1] = x[4];
            matrix.Matrix[1, 2] = x[5];
            matrix.Matrix[2, 0] = x[6];
            matrix.Matrix[2, 1] = x[7];
            matrix.Matrix[2, 2] = 1.0;
        }
        //---------------------------------------------------------------------------------------
        public static void CalculateAndPrintPlotData(Point PointA, Point PointB, out Point PointForward, out Point PointBackward)
        {
            // Midpoint for Point A B
            Point MidPoint = new Point((PointA.X + PointB.X) / 2, (PointA.Y + PointB.Y) / 2);

            // 計算 Point 線段的斜率
            double PointSlope = (PointB.Y - PointA.Y) / (PointB.X - PointA.X);
            double PointTheta = Math.Atan(PointSlope);

            // 計算 Point 線段長度和半徑
            double PointLength = Math.Sqrt(Math.Pow(PointB.X - PointA.X, 2) + Math.Pow(PointB.Y - PointA.Y, 2));
            double PointRadius = PointLength / 2;

            // 計算 Point 的 X, Y 偏移量
            double PointXOffset = PointRadius * Math.Sin(PointTheta);
            double PointYOffset = PointRadius * Math.Cos(PointTheta);

            // 計算 Point Forward 和 Backward
            PointForward = new Point(MidPoint.X + PointXOffset, MidPoint.Y - PointYOffset);
            PointBackward = new Point(MidPoint.X - PointXOffset, MidPoint.Y + PointYOffset);
        }
        //---------------------------------------------------------------------------------------
        public static Point MapToCoords(MX matrix, double x, double y) {
            Point result = new Point();

            double w = matrix.Matrix[2, 0] * x + matrix.Matrix[2, 1] * y + matrix.Matrix[2, 2];

            if (Math.Abs(w) < EPSILON) {
                Console.WriteLine("無效的透視變換矩陣");
            } else {
                result.X = (matrix.Matrix[0, 0] * x + matrix.Matrix[0, 1] * y + matrix.Matrix[0, 2]) / w;
                result.Y = (matrix.Matrix[1, 0] * x + matrix.Matrix[1, 1] * y + matrix.Matrix[1, 2]) / w;
            }

            return result;
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------



        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        //Example
        //static void Main()
        //{
        // 測試數值
        //    int inputValue = 500000; // 可以改成任何你想測試的數值
        //    double mappedValue = Map(inputValue, -1048576, 1048576, -500.000, 500.000);
        //    Console.WriteLine($"Mapped Value: {mappedValue}");
        //}
        //---------------------------------------------------------------------------------------
        // 映射函數
        public double Map(int Input, int in_min, int in_max, double out_min, double out_max)
        {
            // 計算映射 並強制轉型為 double
            return (double)(Input - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
        public double Map(double Input, double in_min, double in_max, double out_min, double out_max)
        {
            // 計算映射 並強制轉型為 double
            return (double)(Input - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
        public int Map(double Input, double in_min, double in_max, int out_min, int out_max)
        {
            // 計算映射 並強制轉型為 int
            return (int)((Input - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
        }
        public int Map(int Input, int in_min, int in_max, int out_min, int out_max)
        {
            // 計算映射 並強制轉型為 int
            return (int)((Input - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
        }
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------



        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------



    }
}
