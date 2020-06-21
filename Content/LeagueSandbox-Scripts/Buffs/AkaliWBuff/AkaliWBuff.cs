using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GameServerCore.Domain;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Other;
using LeagueSandbox.GameServer.GameObjects.Spells;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace AkaliWBuff
{
    class AkaliWBuff : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        IBuff curBuff;
        int radius = 300;
        Vector2 origin;
        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IObjAiBase unit, IBuff buff, ISpell ownerSpell)
        {
            curBuff = buff;
            origin = unit.GetPosition();
            unit.SetInvis(true);
        }

        public void OnDeactivate(IObjAiBase unit)
        {
            unit.SetInvis(false);
        }
        bool invisPenalty = false;
        GameScriptTimer pTimer = null;
        public void OnUpdate(double diff)
        {
            var unit = curBuff.OriginSpell.Owner;
            var curPos = curBuff.TargetUnit.GetPosition();
            var dist = Vector2.Distance(origin, curPos);
            if (dist >= radius)
            {
                unit.SetInvis(false);
                return;
            }
            if (unit.IsAttacking || unit.IsCastingSpell)
            {
                invisPenalty = true;
                unit.SetInvis(false);
                if (!(pTimer is null)) pTimer.EndTimerWithoutCallback();
                if (curBuff.Duration - curBuff.TimeElapsed > 2.0f)
                {
                    pTimer = CreateTimer(1.5f, () =>
                    {
                        invisPenalty = false;
                    });
                }
            }
            if (!invisPenalty) unit.SetInvis(true);
            
        }
                    
    }
}
