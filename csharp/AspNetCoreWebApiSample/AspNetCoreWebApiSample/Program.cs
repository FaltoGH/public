using AspNetCoreWebApiSample.Data.SQLite;
using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AspNetCoreWebApiSample
{
    public static class Program
    {
        private static readonly JwtProvider jwtProvider = new JwtProvider();

        static IResult GetResult(JObject jo)
        {
            return Results.Content(jo.ToString(Newtonsoft.Json.Formatting.None), "application/json");
        }

        static IResult GetResultB(object o)
        {
            return GetResult(JObject.FromObject(o));
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("begin Main");
            Console.WriteLine("cd: " + Directory.GetCurrentDirectory());

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            WebApplication app = builder.Build();
            app.UseStaticFiles();
            string[] summaries =
            [
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            ];

            app.MapGet("/wf", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            });


            app.MapGet("/grant", () =>
            {
                Dictionary<string, object> payload = new Dictionary<string, object>
                {
                    { "int1", 3 },
                    { "string1", "value5" },
                    // https://www.rfc-editor.org/info/rfc7519/#section-4.1
                    { "exp", DateTimeOffset.UtcNow.AddSeconds(20).ToUnixTimeSeconds() },
                    { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
                };
                string token = jwtProvider.Encode(payload);
                return token;
            });

            app.MapGet("/verify", (string token) =>
            {
                string json = jwtProvider.Decode(token);
                return json;
            });

            app.MapGet("/getecho", (string x, string y = "default_y", byte z = 100) =>
            {
                JObject jo = new JObject();
                jo["x"] = x;
                jo["y"] = y;
                jo["z"] = z;
                return GetResult(jo);
            });

            app.MapGet("/zip", () =>
            {
                return GzipHelper.Zip("Hello, World!");
            });

            using (var conn = new SQLiteConnectionB("mydatabase.db"))
            {
                conn.CommandText = "DROP TABLE IF EXISTS user;";
                conn.ExecuteNonQuery();

                conn.CommandText = "CREATE TABLE IF NOT EXISTS user(id TEXT PRIMARY KEY, name TEXT,age INT);";
                conn.ExecuteNonQuery();

                conn.CommandText = "DELETE FROM user;";
                conn.ExecuteNonQuery();

                conn.CommandText = "INSERT OR IGNORE INTO user(id,name,age) VALUES('chul','Kim Chulsoo',23),('yung','Kim Yunghee',24);";
                conn.ExecuteNonQuery();

                object _lock = new object();

                app.MapGet("/select", (string id) =>
                {
                    lock (_lock)
                    {
                        // warning: At the line below, SQL injection is possible.
                        conn.CommandText = "SELECT id,name,age FROM user WHERE id = '" + id + "';";

                        using (SQLiteDataReader rdr = conn.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                string rid = rdr.GetString(0);
                                string name = rdr.GetString(1);
                                int age = rdr.GetInt32(2);
                                var user = new User(rid, name, age);
                                return GetResultB(user);
                            }
                        }

                        return Results.InternalServerError();
                    }
                });

                app.Run();
            }

            Console.WriteLine("end Main");
        }
    }
}
