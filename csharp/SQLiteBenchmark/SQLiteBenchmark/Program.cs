namespace SQLiteBenchmark
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            File.Delete("tmp.db");
            var conn = new SQLiteConnection2("tmp.db");
            
        }
    }
}
