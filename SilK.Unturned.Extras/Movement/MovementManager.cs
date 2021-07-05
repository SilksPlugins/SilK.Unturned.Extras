using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using SDG.Unturned;
using SilK.Unturned.Extras.Players;
using Steamworks;
using System;
using System.Collections.Generic;

namespace SilK.Unturned.Extras.Movement
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    internal class MovementManager : IMovementManager
    {
        public class Multipliers
        {
            public float Gravity { get; set; } = 1;

            public float Jump { get; set; } = 1;

            public float Speed { get; set; } = 1;
        }

        private readonly Dictionary<CSteamID, Dictionary<string, Multipliers>> _multipliers;

        public MovementManager()
        {
            _multipliers = new();
        }

        private Multipliers GetMultipliers(Player player, string id)
        {
            if (_multipliers.TryGetValue(player.GetSteamId(), out var multipliersDict))
            {
                if (multipliersDict.TryGetValue(id, out var multipliers))
                    return multipliers;

                multipliers = new Multipliers();

                multipliersDict.Add(id, multipliers);

                return multipliers;
            }
            else
            {
                var multipliers = new Multipliers();

                _multipliers.Add(player.GetSteamId(), new Dictionary<string, Multipliers>
                {
                    {id, multipliers}
                });

                return multipliers;
            }
        }

        public async UniTask<float> GetGravityMultiplier(Player player)
        {
            await UniTask.SwitchToMainThread();

            return player.movement.pluginGravityMultiplier;
        }

        public async UniTask<float> GetJumpMultiplier(Player player)
        {
            await UniTask.SwitchToMainThread();

            return player.movement.pluginJumpMultiplier;
        }

        public async UniTask<float> GetSpeedMultiplier(Player player)
        {
            await UniTask.SwitchToMainThread();

            return player.movement.pluginSpeedMultiplier;
        }

        private float GetTotalPluginMultiplier(Player player, Func<Multipliers, float> valueChooser)
        {
            var product = 1f;

            if (!_multipliers.TryGetValue(player.GetSteamId(), out var multipliersDict)) return product;

            foreach (var multipliers in multipliersDict.Values)
            {
                product *= valueChooser.Invoke(multipliers);

                // No value will change this
                if (product == 0) break;
            }

            return product;
        }

        public async UniTask SetGravityMultiplier(Player player, string id, float gravityMultiplier)
        {
            await UniTask.SwitchToMainThread();

            var multipliers = GetMultipliers(player, id);

            multipliers.Gravity = gravityMultiplier;

            var gravity = GetTotalPluginMultiplier(player, x => x.Gravity);

            player.movement.sendPluginGravityMultiplier(gravity);
        }

        public async UniTask SetJumpMultiplier(Player player, string id, float jumpMultiplier)
        {
            await UniTask.SwitchToMainThread();

            var multipliers = GetMultipliers(player, id);

            multipliers.Jump = jumpMultiplier;

            var jump = GetTotalPluginMultiplier(player, x => x.Jump);

            player.movement.sendPluginJumpMultiplier(jump);
        }

        public async UniTask SetSpeedMultiplier(Player player, string id, float speedMultiplier)
        {
            await UniTask.SwitchToMainThread();

            var multipliers = GetMultipliers(player, id);

            multipliers.Speed = speedMultiplier;

            var speed = GetTotalPluginMultiplier(player, x => x.Speed);

            player.movement.sendPluginSpeedMultiplier(speed);
        }
    }
}
