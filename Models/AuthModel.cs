namespace PMAuth.Models
{
    public class AuthModel
    {
        public AuthModel(string name, string token)
        {
            Name = name;
            Token = token;
        }

        public  string Name { get; set; }
        public string Token { get; set; }
    }
}
