using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using SDG.Unturned;

namespace SilK.Unturned.Extras.Movement
{
    [Service]
    public interface IMovementManager
    {
        UniTask<float> GetGravityMultiplier(Player player);
        UniTask<float> GetJumpMultiplier(Player player);
        UniTask<float> GetSpeedMultiplier(Player player);

        UniTask SetGravityMultiplier(Player player, string id, float gravityMultiplier);
        UniTask SetJumpMultiplier(Player player, string id, float jumpMultiplier);
        UniTask SetSpeedMultiplier(Player player, string id, float speedMultiplier);
    }
}
