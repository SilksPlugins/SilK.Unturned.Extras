using OpenMod.API.Ioc;
using Steamworks;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.Steam
{
    /// <summary>
    /// A service for fetching steam profile information.
    /// </summary>
    [Service]
    public interface ISteamProfileFetcher
    {
        /// <summary>
        /// Gets the steam profile data for a player.
        /// </summary>
        /// <param name="steamId">The steam id of the player.</param>
        /// <param name="forceRefresh">Force a refresh of the cache.</param>
        /// <returns>The steam profile data for the player.</returns>
        Task<ISteamProfile?> GetSteamProfile(CSteamID steamId, bool forceRefresh = false);
    }
}
