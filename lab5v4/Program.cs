using System;

namespace lab5v4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №5 (Варіант 4) ===");

            Matrix2x2 m1 = new Matrix2x2(1, 2, 3, 4);
            Matrix2x2 m2 = new Matrix2x2(5, 6, 7, 8);

            Console.WriteLine("Матриця M1:");
            Console.WriteLine(m1);

            Console.WriteLine("\nМатриця M2:");
            Console.WriteLine(m2);

            Console.WriteLine($"\nЕлемент M1[0, 1]: {m1[0, 1]}");
            m1[0, 1] = 10;
            Console.WriteLine($"M1 після зміни M1[0, 1] = 10:\n{m1}");
            m1[0, 1] = 2;

            Matrix2x2 sum = m1 + m2;
            Console.WriteLine($"\nM1 + M2:\n{sum}");

            Matrix2x2 scaled = m1 * 3;
            Console.WriteLine($"\nM1 * 3:\n{scaled}");

            Matrix2x2 transposed = m1.Transpose();
            Console.WriteLine($"\nТранспонована M1:\n{transposed}");

            Matrix2x2 m3 = new Matrix2x2(1, 2, 3, 4);
            Console.WriteLine($"\nM1 == M3: {m1 == m3}");
            Console.WriteLine($"M1 != M2: {m1 != m2}");
        }
    }
}