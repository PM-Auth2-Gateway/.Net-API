namespace PMAuth.Models.OAuthUniversal
{
    public class UserProfile
    {
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        //public string Gender { get; set; }
        //public string BirthDate { get; set; }
        public string Photo { get; set; }
        public string Locale { get; set; }
        public bool? IsVerifiedEmail { get; set; }

        /*public override string ToString()
        {
            return $"Id: {Id}\n" +
                   $"First Name: {FirstName}\n" +
                   $"Last Name: {LastName}\n";
        }*/
    }
}