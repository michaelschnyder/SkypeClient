namespace Skype.Client.Protocol.People
{
    public class ProfileItem
    {
        public int Status { get; set; }
        public string Type { get; set; }
        public bool Authorized { get; set; }
        public Profile Profile { get; set; }
    }
}