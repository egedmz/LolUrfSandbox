using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using GameServerCore.Domain;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace Spells
{
    public class YasuoQ3W : IGameScript
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
            casting = false;
            adder = false;
            _owner = owner as IChampion;
            //empty
        }

        public void OnDeactivate(IObjAiBase owner)
        {
            owner.IsCastingSpell = false;
            //empty
        }

        public void OnStartCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
        }

        public void OnFinishCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            adder = false;
            casting = true;

            _owner = owner as IChampion;
            var current = new Vector2(owner.X, owner.Y);
            var to = Vector2.Normalize(new Vector2(spell.X, spell.Y) - current);
            var range = to * spell.SpellData.CastRangeDisplayOverride[0];
            trueCoords = current + range;

            CreateTimer(0.03f, () =>
            {
                owner.IsCastingSpell = true;
            });
            if (HasBuff(owner, "YasuoE"))
            {
                CurrentCastTime = MaxCastTimeQE;
                spell.SpellAnimation("SPELL1_Dash", owner);
                AddParticleTarget(owner, "Yasuo_Base_EQ3_cas.troy", owner);
                AddParticleTarget(owner, "Yasuo_Base_EQ_SwordGlow.troy", owner, bone: "C_BUFFBONE_GLB_Weapon_1");
                foreach (var affectEnemys in GetUnitsInRange(owner, 270f, true))
                {
                    if (affectEnemys is IAttackableUnit && affectEnemys.Team != owner.Team)
                    {
                        affectEnemys.TakeDamage(owner, spell.Level * 20f + owner.Stats.AttackDamage.Total, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
                        AddParticleTarget(owner, "Yasuo_Base_Q_WindStrike.troy", affectEnemys);
                        AddParticleTarget(owner, "Yasuo_Base_Q_windstrike_02.troy", affectEnemys);
                        AddParticleTarget(owner, "Yasuo_Base_Q_hit_tar.troy", affectEnemys);
                        //DashToLocation((ObjAiBase)affectEnemys, affectEnemys.X + 10f, affectEnemys.Y + 10f, 13f, true, "RUN", 16.5f, travelTime: 1.15f);
                    }
                }
            }
            else
            {
                (owner as IChampion).StopChampionMovement();
                FaceDirection(owner, to, true, 0f);
                CurrentCastTime = MaxCastTime;

                spell.AddProjectile("YasuoQ3Mis", owner.X, owner.Y, trueCoords.X, trueCoords.Y);
                spell.SpellAnimation("SPELL1C", owner);
                
                AddParticleTarget(owner, "Yasuo_Base_Q3_Hand.troy", owner);
                AddParticleTarget(owner, "Yasuo_Base_Q3_cast_sound.troy", owner);
            }
            if (HasBuff(owner, "YasuoQ02"))
            {
                adder = true;
            }
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, IProjectile projectile)
        {
            AddParticleTarget(owner, "Yasuo_Base_Q_WindStrike.troy", target);
            AddParticleTarget(owner, "Yasuo_Base_Q_windstrike_02.troy", target);
            AddParticleTarget(owner, "Yasuo_Base_Q_hit_tar.troy", target);
            target.TakeDamage(owner, spell.Level * 20f + owner.Stats.AttackDamage.Total,DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            DashToLocation((ObjAiBase)target, target.X+10f, target.Y+10f, 13f, true, "RUN", 16.5f, travelTime:1.15f);
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
                    if (adder)
                    {
                        RemoveBuff(_owner, "YasuoQ02");
                    }
                }
            }
        }
    }
}
