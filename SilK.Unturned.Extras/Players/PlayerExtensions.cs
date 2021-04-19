using OpenMod.Unturned.Players;
using OpenMod.Unturned.Users;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;

namespace SilK.Unturned.Extras.Players
{
    public static class PlayerExtensions
    {
        public static CSteamID GetSteamId(this Player player) => player.channel.owner.playerID.steamID;

        public static ITransportConnection GetTransportConnection(this Player player) =>
            player.channel.GetOwnerTransportConnection();

        public static ITransportConnection GetTransportConnection(this UnturnedPlayer player) =>
            GetTransportConnection(player.Player);

        public static ITransportConnection GetTransportConnection(this UnturnedUser user) =>
            GetTransportConnection(user.Player.Player);
    }
}
