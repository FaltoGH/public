using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SQLiteBenchmark
{
    internal static class Program
    {
        private const int PRINT_INTERVAL_MS = 500;
        private const int SIZE = 100;

        private static void Benchmark1001()
        {
            Console.WriteLine("Benchmark1001");
            Stopwatch sw = Stopwatch.StartNew();
            Random dice = new Random(1);
            File.Delete("tmp.db");
            SQLiteConnection2 conn = new SQLiteConnection2("tmp.db");
            Stopwatch printCooldown = Stopwatch.StartNew();
            for (int i = 0; i < SIZE; i++)
            {
                conn.CommandText = $"CREATE TABLE tabl{i}(name PRIMARY KEY, age, height, weight)";
                conn.ExecuteNonQuery();
                conn.ExecuteAtomicTransaction(() =>
                {
                    for (int j = 0; j < SIZE; j++)
                    {
                        if (printCooldown.ElapsedMilliseconds > PRINT_INTERVAL_MS)
                        {
                            Console.WriteLine($"i={i}; j={j}");
                            printCooldown.Restart();
                        }
                        conn.CommandText = $"INSERT INTO tabl{i} VALUES ({j},{dice.Next()},{dice.Next()},{dice.Next()})";
                        conn.ExecuteNonQuery();
                    }
                });
            }
            conn.Dispose();
            Console.WriteLine($"write: Elapsed ms: {sw.ElapsedMilliseconds}\r\nFile size: {new FileInfo("tmp.db").Length}");
        }

        private static void Benchmark1002()
        {
            Console.WriteLine("Benchmark1002");
            Stopwatch sw = Stopwatch.StartNew();
            Random dice = new Random(1);
            File.Delete("tmp.db");
            SQLiteConnection2 conn = new SQLiteConnection2("tmp.db");
            Stopwatch printCooldown = Stopwatch.StartNew();
            conn.CommandText = $"CREATE TABLE tabl(tid, name, age, height, weight, PRIMARY KEY(tid,name))";
            conn.ExecuteNonQuery();
            for (int i = 0; i < SIZE; i++)
            {
                conn.ExecuteAtomicTransaction(() =>
                {
                    for (int j = 0; j < SIZE; j++)
                    {
                        if (printCooldown.ElapsedMilliseconds > PRINT_INTERVAL_MS)
                        {
                            Console.WriteLine($"i={i}; j={j}");
                            printCooldown.Restart();
                        }
                        conn.CommandText = $"INSERT INTO tabl VALUES ({i},{j},{dice.Next()},{dice.Next()},{dice.Next()})";
                        conn.ExecuteNonQuery();
                    }
                });
            }
            conn.Dispose();
            Console.WriteLine($"write: Elapsed ms: {sw.ElapsedMilliseconds}\r\nFile size: {new FileInfo("tmp.db").Length}");
        }

        private static void Benchmark1003()
        {
            Console.WriteLine("Benchmark1003");
            Stopwatch sw = Stopwatch.StartNew();
            Random dice = new Random(1);
            File.Delete("tmp.db");
            SQLiteConnection2 conn = new SQLiteConnection2("tmp.db");
            Stopwatch printCooldown = Stopwatch.StartNew();
            conn.CommandText = $"CREATE TABLE tabl(tid, name, age, height, weight, PRIMARY KEY(tid,name))";
            conn.ExecuteNonQuery();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < SIZE; i++)
            {
                List<(int, int, int, int, int)> lis = new List<(int, int, int, int, int)>();
                for (int j = 0; j < SIZE; j++)
                {
                    if (printCooldown.ElapsedMilliseconds > PRINT_INTERVAL_MS)
                    {
                        Console.WriteLine($"i={i}; j={j}");
                        printCooldown.Restart();
                    }
                    lis.Add((i, j, dice.Next(), dice.Next(), dice.Next()));
                }
                sb.Clear();
                sb.Append("INSERT INTO tabl VALUES ");
                for(int j = 0; j < SIZE; j++)
                {
                    sb.AppendFormat("({0},{1},{2},{3},{4})", lis[j].Item1, lis[j].Item2, lis[j].Item3, lis[j].Item4, lis[j].Item5);
                    if(j + 1 < SIZE)
                    {
                        sb.Append(",");
                    }
                }
                conn.CommandText = sb.ToString();
                conn.ExecuteNonQuery();
            }
            conn.Dispose();
            Console.WriteLine($"write: Elapsed ms: {sw.ElapsedMilliseconds}\r\nFile size: {new FileInfo("tmp.db").Length}");
        }

        private static void Benchmark1004()
        {
            Console.WriteLine("Benchmark1004");
            Stopwatch sw = Stopwatch.StartNew();
            Random dice = new Random(1);
            File.Delete("tmp.db");
            SQLiteConnection2 conn = new SQLiteConnection2("tmp.db");
            Stopwatch printCooldown = Stopwatch.StartNew();
            conn.CommandText = $"CREATE TABLE tabl(tid, name, age, height, weight, PRIMARY KEY(tid,name))";
            conn.ExecuteNonQuery();
            for (int i = 0; i < SIZE; i++)
            {
                List<(int, int, int, int, int)> lis = new List<(int, int, int, int, int)>();
                for (int j = 0; j < SIZE; j++)
                {
                    if (printCooldown.ElapsedMilliseconds > PRINT_INTERVAL_MS)
                    {
                        Console.WriteLine($"i={i}; j={j}");
                        printCooldown.Restart();
                    }
                    lis.Add((i, j, dice.Next(), dice.Next(), dice.Next()));
                }
                conn.CommandText = "INSERT INTO tabl VALUES " + string.Join(",", lis.Select(x=>$"({x.Item1},{x.Item2},{x.Item3},{x.Item4},{x.Item5})"));
                conn.ExecuteNonQuery();
            }
            conn.Dispose();
            Console.WriteLine($"write: Elapsed ms: {sw.ElapsedMilliseconds}\r\nFile size: {new FileInfo("tmp.db").Length}");
        }


        private static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Press key...");
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                    Console.WriteLine();
                    switch(consoleKeyInfo.Key)
                    {
                        case ConsoleKey.D1:
                            Benchmark1001();
                            break;
                        case ConsoleKey.D2:
                            Benchmark1002();
                            break;
                        case ConsoleKey.D3:
                            Benchmark1003();
                            break;
                        case ConsoleKey.D4:
                            Benchmark1004();
                            break;

                        case ConsoleKey.Q:
                            Console.WriteLine("Quitting...");
                            return;
                        default:
                            Console.WriteLine("Did nothing.");
                            break;
                    }
                    Console.WriteLine("\r\n");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
