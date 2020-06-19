using System.Numerics;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Packets.PacketDefinitions.Responses;
using System;

namespace Spells
{
    public class AkaliShadowDance : IGameScript
    {
        IAttackableUnit lastTarget = null;
        public void OnActivate(IObjAiBase owner)
        {
        }

        public void OnDeactivate(IObjAiBase owner)
        {
        }

        public void OnStartCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
        }

        public void OnFinishCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            var current = new Vector2(owner.X, owner.Y);
            var dist = Vector2.Distance(current, new Vector2(target.X, target.Y));
            var offset = 100;
            var to = Vector2.Normalize(new Vector2(target.X, target.Y) - current);

            var trueCoords = current + (to * (dist + offset));

            //TODO: Dash to the correct location (in front of the enemy IChampion) instead of far behind or inside them
            spell.DashToLocation(owner, trueCoords.X, trueCoords.Y, 2200, false, "Attack1");
            lastTarget = target;
            AddParticleTarget(owner, "akali_shadowDance_tar.troy", target, 1, "");
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, IProjectile projectile)
        {
            var bonusAd = owner.Stats.AttackDamage.Total - owner.Stats.AttackDamage.BaseValue;
            var ap = owner.Stats.AbilityPower.Total * 0.5f;
            var damage = 150 + spell.Level * 100 + bonusAd + ap;
            lastTarget.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL,
                DamageSource.DAMAGE_SOURCE_SPELL, false);
        }

        public void OnUpdate(double diff)
        {
        }
    }
}
