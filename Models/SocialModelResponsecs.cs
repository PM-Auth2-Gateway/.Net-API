namespace PMAuth.Models
{
    public class SocialModelResponsecs
    {
        public int Id { get; set; }
        public string name { get; set; }
        public bool IsSetting { get; set; }

        public SocialModelResponsecs(int id, string name, bool isSetting)
        {
            Id = id;
            this.name = name;
            IsSetting = isSetting;
        }
    }
}
