namespace SilK.Unturned.Extras.Steam
{
    public interface ISteamProfile
    {
        ulong SteamId { get; }

        string ProfileName { get; }
        
        string AvatarIcon { get; set; }
        
        string AvatarMedium { get; set; }
        
        string AvatarFull { get; set; }
    }
}
