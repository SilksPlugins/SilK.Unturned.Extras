using System.Xml.Serialization;

namespace SilK.Unturned.Extras.Steam
{
    [XmlRoot(ElementName = "profile")]
    public class SteamProfile : ISteamProfile
    {
        [XmlElement(ElementName = "steamID64")]
        public ulong SteamId { get; set; }

        [XmlElement(ElementName = "steamID")]
        public string ProfileName { get; set; } = null!;

        [XmlElement(ElementName = "onlineState")]
        public string OnlineState { get; set; } = null!;

        [XmlElement(ElementName = "stateMessage")]
        public string StateMessage { get; set; } = null!;

        [XmlElement(ElementName = "privacyState")]
        public string PrivacyState { get; set; } = null!;

        [XmlElement(ElementName = "visibilityState")]
        public int VisibilityState { get; set; }

        [XmlElement(ElementName = "avatarIcon")]
        public string AvatarIcon { get; set; } = null!;

        [XmlElement(ElementName = "avatarMedium")]
        public string AvatarMedium { get; set; } = null!;

        [XmlElement(ElementName = "avatarFull")]
        public string AvatarFull { get; set; } = null!;

        [XmlElement(ElementName = "vacBanned")]
        public string VacBanned { get; set; } = null!;

        [XmlElement(ElementName = "tradeBanState")]
        public string TradeBanState { get; set; } = null!;

        [XmlElement(ElementName = "isLimitedAccount")]
        public string IsLimitedAccount { get; set; } = null!;
    }
}
