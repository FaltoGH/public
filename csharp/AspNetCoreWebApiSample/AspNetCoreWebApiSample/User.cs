namespace AspNetCoreWebApiSample
{
    public class User
    {
        public string id;
        public string name;
        public int age;

        public User(string id, string name, int age)
        {
            this.id = id;
            this.name = name;
            this.age = age;
        }
    }
}
