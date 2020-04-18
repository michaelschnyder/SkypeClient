namespace Skype.Client
{
    public class StatusChangedEventArgs
    {
        public AppStatus Old { get; set; }

        public AppStatus New { get; set; }
    }
}