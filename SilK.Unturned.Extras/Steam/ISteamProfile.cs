namespace SilK.Unturned.Extras.Steam
{
    /// <summary>
    /// The profile data for a player's steam account.
    /// </summary>
    public interface ISteamProfile
    {
        ulong SteamId { get; }

        string ProfileName { get; }
        
        string AvatarIcon { get; set; }
        
        string AvatarMedium { get; set; }
        
        string AvatarFull { get; set; }
    }
}
