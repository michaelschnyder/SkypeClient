namespace Skype.Client.Protocol.Contacts
{
    public class ContactsFrame
    {
        public Contact[] Contacts { get; set; }
        public object[] BlockList { get; set; }
        public object[] Groups { get; set; }
        public string Scope { get; set; }
    }
}
