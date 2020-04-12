using System;
using System.Xml.Serialization;

namespace Skype.Client.Protocol.Events.Resource.Content
{
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "partlist")]
    public class ParticipantList
    {
        [XmlElement("part")]
        public Participant[] Participants { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("alt")]
        public string Alt { get; set; }

        [XmlAttribute("callId")]
        public string CallId { get; set; }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class Participant
    {

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlAttribute("identity")]
        public string Identity { get; set; }
    }
}