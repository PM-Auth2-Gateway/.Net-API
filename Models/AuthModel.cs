namespace PMAuth.Models
{
    public class AuthModel
    {
        public AuthModel(string name, string token, string refreshtoken)
        {
            Name = name;
            Token = token;
            Refreshtoken = refreshtoken;
        }

        public  string Name { get; set; }
        public string Token { get; set; }
        public string Refreshtoken { get; set; }
    }
}
