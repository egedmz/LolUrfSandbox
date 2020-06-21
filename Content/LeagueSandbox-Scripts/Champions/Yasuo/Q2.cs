using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using GameServerCore.Domain;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace Spells
{
    public class YasuoQ2W : IGameScript
    {
        private Vector2 trueCoords;
        float MaxCastTime = 0.6f;
        float MaxCastTimeQE = 0.3f;
        float CurrentCastTime;
        bool casting = false;
        IChampion _owner;
        ISpell _spell;
        bool adder;
        public void OnActivate(IObjAiBase owner)
        {
            CurrentCastTime = MaxCastTime;
            adder = false;
            _owner = owner as IChampion;
            // here's nothing
        }

        public void OnDeactivate(IObjAiBase owner)
        {
            // here's empty
            owner.IsCastingSpell = false;
        }


        public void OnStartCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
        }

        public void OnFinishCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            adder = false;
            casting = true;

            _owner = owner as IChampion;

            CreateTimer(0.03f, () =>
            {
                owner.IsCastingSpell = true;
            });


            var current = new Vector2(owner.X, owner.Y);
            var to = Vector2.Normalize(new Vector2(spell.X, spell.Y) - current);
            var range = to * spell.SpellData.CastRangeDisplayOverride[0];
            trueCoords = current + range;

            if (HasBuff(owner, "YasuoE"))
            {
                CurrentCastTime = MaxCastTimeQE;
                spell.SpellAnimation("SPELL1_Dash", owner);
                AddParticleTarget(owner, "Yasuo_Base_EQ_cas.troy", owner);
                AddParticleTarget(owner, "Yasuo_Base_EQ_SwordGlow.troy", owner, 1, "C_BUFFBONE_GLB_Weapon_1");
                foreach (var affectEnemys in GetUnitsInRange(owner, 270f, true))
                {
                    if (affectEnemys is IAttackableUnit && affectEnemys.Team != owner.Team)
                    {
                        affectEnemys.TakeDamage(owner, spell.Level * 20f + owner.Stats.AttackDamage.Total, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
                        AddParticleTarget(owner, "Yasuo_Base_Q_hit_tar.troy", affectEnemys);
                    }
                }
                adder = true;
            }
            else
            {
                (owner as IChampion).StopChampionMovement();
                FaceDirection(owner, to, true, 0f);
                CurrentCastTime = MaxCastTime;

                spell.SpellAnimation("SPELL1B", owner, 1.1f);
                spell.AddLaser("YasuoQ", trueCoords.X, trueCoords.Y);
                AddParticleTarget(owner, "Yasuo_Q_Hand.troy", owner);
                AddParticleTarget(owner, "Yasuo_Base_Q1_cast_sound.troy", owner);
                AddParticleTarget(owner, "Yasuo_Base_Q_strike_build_up_test.troy", owner, bone: "C_BUFFBONE_GLB_Weapon_1");
                AddParticleTarget(owner, "Yasuo_Base_Q_strike_test.troy", owner, bone: "C_BUFFBONE_GLB_Weapon_1");


            }
        }


        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, IProjectile projectile)
        {
            AddParticleTarget(owner, "Yasuo_Base_Q_hit_tar.troy", target);
            target.TakeDamage(owner, spell.Level * 20f + owner.Stats.AttackDamage.Total,DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            if (!HasBuff(owner, "YasuoQ02"))
            {
                adder = true;
            }
        }

        public void OnUpdate(double diff)
        {
            if (casting)
            {
                CurrentCastTime -= (float)diff / 1000.0f;
                if (CurrentCastTime <= 0)
                {
                    _owner.IsCastingSpell = false;
                    casting = false;
                    adder = false;
                    if (adder)
                    {
                        AddBuff("YasuoQ02", 6f, 1, _spell, _owner, _owner);
                        RemoveBuff(_owner, "YasuoQ01");
                    }
                }
            }
        }
    }
}
