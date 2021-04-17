using OpenMod.API.Ioc;
using Steamworks;
using System.Threading.Tasks;

namespace SilK.Unturned.Extras.Steam
{
    [Service]
    public interface ISteamProfileFetcher
    {
        Task<ISteamProfile?> GetSteamProfile(CSteamID steamId);
    }
}
