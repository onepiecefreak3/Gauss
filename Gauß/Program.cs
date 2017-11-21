using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Gauß
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() != 2)
            {
                Console.WriteLine("So wird das Programm verwendet:\nGauß.exe <lineCount> \"{...}\"\n\nSetze die Matrix in \"\". " +
                    "Trenne die Elemente mit einem Leerzeichen.");
                Environment.Exit(0);
            }

            int lineCount = Int32.Parse(args[0]);
            var matrix = Matrix.ReadMatrix(args[1]);

            var solved = Matrix.SolveSystem(matrix, lineCount);
        }
    }

    public class Helper
    {
        public static int GgT(int x, int y)
        {
            if (x < y)
            {
                var help = x;
                x = y;
                y = help;
            }

            while (x % y != 0)
            {
                var r = x % y;

                x = y;
                y = r;
            }

            return y;
        }

        public static int KgV(int x, int y)
        {
            return (x * y) / GgT(x, y);
        }
    }

    public class Matrix
    {
        public static int[] SolveSystem(int[] matrix, int lines)
        {
            var cols = matrix.Count() / lines;

            var solveMatrix = new int[matrix.Count()];
            Array.Copy(matrix, 0, solveMatrix, 0, matrix.Count());

            //Checke, dass keine 0'en auf Diagonale liegen
            for (int i = 0; i < (lines - 1); i++)
                SearchSwap(matrix, i);

            Console.WriteLine("\nKeine 0 auf der Diagonale:");
            PrintMatrix(matrix);

            //Erstelle Zeilenstufenform
            for (int i = 0; i < (cols - 2); i++)
            {
                var d = Get(matrix, i, i);

                var lineCount = i + 1;
                while (lineCount < lines)
                {
                    var e = Get(matrix, lineCount, i);

                    int kgv = Helper.KgV(e, d);
                    int factor = kgv / e;
                    MultLine(matrix, lineCount, factor);

                    factor = -kgv / d;
                    MultAddLine(matrix, i, lineCount, factor);

                    lineCount++;
                }
            }

            Console.WriteLine("\nZeilenstufenform:");
            PrintMatrix(matrix);

            //Löse nun für jede Variable die Zeilenstufenform auf
            int[] x = new int[cols - 1];
            var distToE = 1;
            for (int i = cols - 2; i >= 0; i--)
            {
                //Bekomme Ergebnis der line und modifiziere es für die finale x-Berechnung
                var e = Get(matrix, i, i + distToE);
                for (int j = i + distToE - 1; j > i; j--) e += -Get(matrix, i, j);

                //Errechne x und setze es in die selbe und darübergelegenen Lines ein
                var xn = e / Get(matrix, i, i);
                x[i] = xn;
                for (int j = i; j >= 0; j--) Set(matrix, j, i, Get(matrix, j, i) * xn);

                distToE++;
            }

            //Erstelle Zahlenvektor mit errechneten x-Werten von Originalmatrix
            Console.WriteLine("\nErgebnis:");
            for (int i = 0; i < cols - 1; i++) Console.WriteLine($"x{i + 1} = {x[i]}");

            for (int i = 0; i < cols - 1; i++)
                for (int j = 0; j < lines; j++)
                    Set(solveMatrix, j, i, Get(solveMatrix, j, i) * x[i]);
            Console.WriteLine("\nFinaler Zahlenvektor:");
            PrintMatrix(solveMatrix);

            return solveMatrix;
        }

        public static void Set(int[] matrix, int line, int column, int value)
        {
            matrix[line * 4 + column] = value;
        }

        public static int Get(int[] matrix, int line, int column)
        {
            return matrix[line * 4 + column];
        }

        public static int[] ReadMatrix(string input)
        {
            return input.Split(' ').Select(e => Int32.Parse(e)).ToArray();
        }

        public static void PrintMatrix(int[] matrix)
        {
            var count = 0;
            foreach (var element in matrix)
            {
                if (count != 0 & count % 4 == 0)
                    Console.WriteLine();
                Console.Write(element + " ");
                count++;
            }

            Console.WriteLine();
        }

        public static void MultLine(int[] matrix, int line, int factor)
        {
            for (int i = line * 4; i < line * 4 + 4; i++)
            {
                matrix[i] *= factor;
            }
        }

        public static void MultAddLine(int[] matrix, int line, int otherLine, int factor)
        {
            int[] tmp = new int[4];
            Array.Copy(matrix, line * 4, tmp, 0, 4);

            var count = 0;
            for (int i = otherLine * 4; i < otherLine * 4 + 4; i++)
            {
                matrix[i] += tmp[count++] * factor;
            }
        }

        public static void SwapLines(int[] matrix, int lineA, int lineB)
        {
            int[] tmp = new int[4];
            Array.Copy(matrix, lineA * 4, tmp, 0, 4);

            for (int i = 0; i < 4; i++)
            {
                matrix[lineA * 4 + i] = matrix[lineB * 4 + i];
                matrix[lineB * 4 + i] = tmp[i];
            }
        }

        public static void SearchSwap(int[] matrix, int line)
        {
            var nextLine = line + 1;
            while (Get(matrix, line, line) == 0)
            {
                if (Get(matrix, nextLine, line) != 0)
                {
                    SwapLines(matrix, nextLine, line);
                }
                nextLine++;
            }
        }
    }
}
