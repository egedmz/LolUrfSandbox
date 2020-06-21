using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using GameServerCore.Domain;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace Spells
{
    public class YasuoDashWrapper : IGameScript
    {
        public static IObjAiBase _target = null;
        public static IChampion _owner = null;
        public void OnActivate(IObjAiBase owner)
        {
            //here's nothing yet
        }

        public void OnDeactivate(IObjAiBase owner)
        {
            //here's empty
        }

        public void OnStartCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            //here's empty, maybe will add some functions?
        }

        public void OnFinishCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            _target = (IObjAiBase)target;
            _owner = (IChampion)owner;

            if (!_target.HasBuff("YasuoEBlock"))
            {
                var damage = 50f + spell.Level * 20f + _owner.Stats.AbilityPower.Total * 0.6f;
                target.TakeDamage(_owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                AddBuff("YasuoE", 0.4f, 1, spell, _owner, _owner);            
                AddBuff("YasuoEBlock", 11f - spell.Level * 1f, 1, spell, _target, _owner);
            }
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, IProjectile projectile)
        {
            //here's empty because no needed to add things here
        }
               
        public void OnUpdate(double diff)
        {
            //here's empty because it's not working
        }
    }
}
