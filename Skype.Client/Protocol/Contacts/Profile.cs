namespace Skype.Client.Protocol.Contacts
{
    public class Profile
    {
        public string AvatarUrl { get; set; }
        public string Birthday { get; set; }
        public string Gender { get; set; }
        public Location[] Locations { get; set; }
        public Name Name { get; set; }
        public string Language { get; set; }
        public string SkypeHandle { get; set; }
        public string About { get; set; }
        public string Website { get; set; }
    }
}