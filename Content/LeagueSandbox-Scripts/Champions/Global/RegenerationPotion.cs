using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.Items;

namespace Spells
{
    public class RegenerationPotion : IGameScript
    {
        public void OnStartCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
        }

        public void OnFinishCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            AddBuff("HealthPotion", 15.0f, 1, spell, owner, owner);
            var p = AddParticleTarget(owner, "potion_manaheal.troy", owner, 1);
            CreateTimer(15.0f, () =>
            {
                RemoveParticle(p);
            });
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, IProjectile projectile)
        {
        }

        public void OnUpdate(double diff)
        {
        }

        public void OnActivate(IObjAiBase owner)
        {
        }

        public void OnDeactivate(IObjAiBase owner)
        {
        }
    }
}
