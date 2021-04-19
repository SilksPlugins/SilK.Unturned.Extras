using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Users;
using SDG.Unturned;

namespace SilK.Unturned.Extras.Movement
{
    public static class MovementManagerExtensions
    {
        public static UniTask ResetGravityMultiplier(this IMovementManager movementManager, Player player, string id) =>
            movementManager.SetGravityMultiplier(player, id, 1);

        public static UniTask ResetJumpMultiplier(this IMovementManager movementManager, Player player, string id) =>
            movementManager.SetJumpMultiplier(player, id, 1);

        public static UniTask ResetSpeedMultiplier(this IMovementManager movementManager, Player player, string id) =>
            movementManager.SetSpeedMultiplier(player, id, 1);

        public static UniTask ResetGravityMultiplier(this IMovementManager movementManager, UnturnedPlayer player, string id) =>
            movementManager.SetGravityMultiplier(player.Player, id, 1);

        public static UniTask ResetJumpMultiplier(this IMovementManager movementManager, UnturnedPlayer player, string id) =>
            movementManager.SetJumpMultiplier(player.Player, id, 1);

        public static UniTask ResetSpeedMultiplier(this IMovementManager movementManager, UnturnedPlayer player, string id) =>
            movementManager.SetSpeedMultiplier(player.Player, id, 1);

        public static UniTask ResetGravityMultiplier(this IMovementManager movementManager, UnturnedUser user, string id) =>
            movementManager.SetGravityMultiplier(user.Player.Player, id, 1);

        public static UniTask ResetJumpMultiplier(this IMovementManager movementManager, UnturnedUser user, string id) =>
            movementManager.SetJumpMultiplier(user.Player.Player, id, 1);

        public static UniTask ResetSpeedMultiplier(this IMovementManager movementManager, UnturnedUser user, string id) =>
            movementManager.SetSpeedMultiplier(user.Player.Player, id, 1);

        public static async UniTask ResetAllMultipliers(this IMovementManager movementManager, Player player, string id)
        {
            await UniTask.SwitchToMainThread();

            await movementManager.ResetGravityMultiplier(player, id);
            await movementManager.ResetJumpMultiplier(player, id);
            await movementManager.ResetSpeedMultiplier(player, id);
        }

        public static async UniTask ResetAllMultipliers(this IMovementManager movementManager, UnturnedPlayer player, string id)
        {
            await UniTask.SwitchToMainThread();

            await movementManager.ResetGravityMultiplier(player.Player, id);
            await movementManager.ResetJumpMultiplier(player.Player, id);
            await movementManager.ResetSpeedMultiplier(player.Player, id);
        }

        public static async UniTask ResetAllMultipliers(this IMovementManager movementManager, UnturnedUser user, string id)
        {
            await UniTask.SwitchToMainThread();

            await movementManager.ResetGravityMultiplier(user.Player.Player, id);
            await movementManager.ResetJumpMultiplier(user.Player.Player, id);
            await movementManager.ResetSpeedMultiplier(user.Player.Player, id);
        }

        public static UniTask<float> GetGravityMultiplier(
            this IMovementManager movementManager, UnturnedPlayer player) =>
            movementManager.GetGravityMultiplier(player.Player);

        public static UniTask<float> GetGravityMultiplier(this IMovementManager movementManager, UnturnedUser user) =>
            movementManager.GetGravityMultiplier(user.Player.Player);

        public static UniTask<float> GetJumpMultiplier(this IMovementManager movementManager, UnturnedPlayer player) =>
            movementManager.GetJumpMultiplier(player.Player);

        public static UniTask<float> GetJumpMultiplier(this IMovementManager movementManager, UnturnedUser user) =>
            movementManager.GetJumpMultiplier(user.Player.Player);

        public static UniTask<float> GetSpeedMultiplier(this IMovementManager movementManager, UnturnedPlayer player) =>
            movementManager.GetSpeedMultiplier(player.Player);

        public static UniTask<float> GetSpeedMultiplier(this IMovementManager movementManager, UnturnedUser user) =>
            movementManager.GetSpeedMultiplier(user.Player.Player);

        public static UniTask SetGravityMultiplier(this IMovementManager movementManager,
            UnturnedPlayer player, string id, float gravityMultiplier) =>
            movementManager.SetGravityMultiplier(player.Player, id, gravityMultiplier);

        public static UniTask SetGravityMultiplier(this IMovementManager movementManager,
            UnturnedUser user, string id, float gravityMultiplier) =>
            movementManager.SetGravityMultiplier(user.Player.Player, id, gravityMultiplier);

        public static UniTask SetJumpMultiplier(this IMovementManager movementManager,
            UnturnedPlayer player, string id, float jumpMultiplier) =>
            movementManager.SetJumpMultiplier(player.Player, id, jumpMultiplier);

        public static UniTask SetJumpMultiplier(this IMovementManager movementManager,
            UnturnedUser user, string id, float jumpMultiplier) =>
            movementManager.SetJumpMultiplier(user.Player.Player, id, jumpMultiplier);

        public static UniTask SetSpeedMultiplier(this IMovementManager movementManager,
            UnturnedPlayer player, string id, float speedMultiplier) =>
            movementManager.SetSpeedMultiplier(player.Player, id, speedMultiplier);

        public static UniTask SetSpeedMultiplier(this IMovementManager movementManager,
            UnturnedUser user, string id, float speedMultiplier) =>
            movementManager.SetSpeedMultiplier(user.Player.Player, id, speedMultiplier);
    }
}
