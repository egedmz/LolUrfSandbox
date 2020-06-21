﻿using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using GameServerCore.Packets.Interfaces;
using System.Collections.Generic;

namespace GameServerCore.Domain
{
    public interface ISpell: IUpdate
    {
        IObjAiBase Owner { get; }
        byte Level { get; }
        byte Slot { get; }
        float CastTime { get; }
        string SpellName { get; }
        SpellState State { get; }
        float CurrentCooldown { get; }
        float CurrentCastTime { get; }
        float CurrentChannelDuration { get; }
        Dictionary<uint, IProjectile> Projectiles { get; }
        uint SpellNetId { get; }
        IAttackableUnit Target { get; }
        float X { get; }
        float Y { get; }
        float X2 { get; }
        float Y2 { get; }
        ISpellData SpellData { get; }
        bool Cast(float x, float y, float x2, float y2, IAttackableUnit u);
        int GetId();
        float GetCooldown();
        void LowerCooldown(float lowerValue);
        void Deactivate();
        void ApplyEffects(IAttackableUnit u, IProjectile p);
        void LevelUp();
        void SetLevel(byte toLevel);
        void AddProjectile(string nameMissile, float fromX, float fromY, float toX, float toY, bool isServerOnly = false);
        void AddProjectileTarget(string nameMissile, ITarget target, bool isServerOnly = false);
        void AddLaser(string effectName, float toX, float toY, bool affectAsCastIsOver = true);
        void AddCone(string effectName, float toX, float toY, float angleDeg, bool affectAsCastIsOver = true);

        public void DashToLocation(IObjAiBase unit, float x, float y,
                                 float dashSpeed,
                                 bool keepFacingLastDirection,
                                 string animation = null,
                                 float leapHeight = 0.0f,
                                 float followTargetMaxDistance = 0.0f,
                                 float backDistance = 0.0f,
                                 float travelTime = 0.0f);
        void SpellAnimation(string animName, IAttackableUnit target, float speedScale = 1.0f);


    }
}
