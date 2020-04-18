namespace Skype.Client
{
    public class Profile
    {
        public Profile(string userId, string displayName)
        {
            Id = userId;
            DisplayName = displayName;
        }

        public string Id { get; }
        public string DisplayName { get; set; }
    }
}