using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System;

namespace Spells
{
    public class YasuoQW : IGameScript
    {
        private Vector2 trueCoords;
        float MaxCastTime = 0.6f;
        float MaxCastTimeQE = 0.3f;
        float CurrentCastTime;
        bool casting = false;
        bool adder = false;
        IChampion _owner;
        ISpell _spell;
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
            casting = true;
            adder = false;
            _spell = spell;
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
                AddParticleTarget(owner, "Yasuo_Base_EQ_SwordGlow.troy", owner,1,"C_BUFFBONE_GLB_Weapon_1");

                int affCnt = 0;
                foreach (var affectEnemys in GetUnitsInRange(owner, 270f, true))
                {
                    if (affectEnemys is IAttackableUnit && affectEnemys.Team != owner.Team)
                    {
                        affectEnemys.TakeDamage(owner, spell.Level * 20f + owner.Stats.AttackDamage.Total, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
                        AddParticleTarget(owner, "Yasuo_Base_Q_hit_tar.troy", affectEnemys);
                    }
                    affCnt += 1;
                }
                if(affCnt > 0)adder = true;
            }
            else
            {
                (owner as IChampion).StopChampionMovement();
                FaceDirection(owner, to, true, 0f);
                CurrentCastTime = MaxCastTime;

                spell.SpellAnimation("SPELL1A", owner, 1.1f);
                spell.AddLaser("YasuoQ", trueCoords.X, trueCoords.Y);
                AddParticleTarget(owner, "Yasuo_Q_Hand.troy", owner);
                AddParticleTarget(owner, "Yasuo_Base_Q1_cast_sound.troy", owner);
                AddParticleTarget(owner, "Yasuo_Base_Q_strike_build_up_test.troy", owner, bone: "C_BUFFBONE_GLB_Weapon_1");
                AddParticleTarget(owner, "Yasuo_Base_Q_strike_test.troy", owner,bone: "C_BUFFBONE_GLB_Weapon_1");


            }
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, IProjectile projectile)
        {
            AddParticleTarget(owner, "Yasuo_Base_Q_hit_tar.troy", target);
            target.TakeDamage(owner, spell.Level * 20f + owner.Stats.AttackDamage.Total,DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            if (!HasBuff(owner, "YasuoQ01"))
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
                    if(adder) AddBuff("YasuoQ01", 6f, 1, _spell, _owner, _owner);
                }
            }
        }
    }
}
