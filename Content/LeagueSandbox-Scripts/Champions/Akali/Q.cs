using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
namespace Spells
{
    public class AkaliMota : IGameScript
    {
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
            var to = Vector2.Normalize(new Vector2(target.X, target.Y) - current);
            var range = to * 1150;
            var trueCoords = current + range;
            spell.AddProjectileTarget("AkaliMota", target);
            //spell.AddProjectile("AkaliMota", owner.X, owner.Y, trueCoords.X, trueCoords.Y, gudumlu: true);
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, IProjectile projectile)
        {
            var ap = owner.Stats.AbilityPower.Total * 0.4f;
            var damage = 15 + spell.Level * 20 + ap;
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            var p=AddParticleTarget(owner, "akali_markOftheAssasin_marker_tar_02.troy", target, 1, "");
            projectile.SetToRemove();
        }

        public void OnUpdate(double diff)
        {
        }
    }
}
