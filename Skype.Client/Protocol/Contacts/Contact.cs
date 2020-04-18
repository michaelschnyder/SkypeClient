using System;

namespace Skype.Client.Protocol.Contacts
{
    public class Contact
    {
        public string PersonId { get; set; }
        public string Mri { get; set; }
        public string DisplayName { get; set; }
        public string DisplayNameSource { get; set; }
        public Profile Profile { get; set; }
        public bool Authorized { get; set; }
        public bool Blocked { get; set; }
        public bool Explicit { get; set; }
        public DateTime? CreationTime { get; set; }
        public RelationshipHistory RelationshipHistory { get; set; }
        public Agent Agent { get; set; }
    }
}