using GameServerCore.Domain;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YasuoE
{
    internal class YasuoE : IBuffGameScript
    {
        public BuffType BuffType => BuffType.INTERNAL;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => true;

        public IStatsModifier StatsModifier { get; private set; }

        private readonly IAttackableUnit target = Spells.YasuoDashWrapper._target;

        public void OnActivate(IObjAiBase unit, IBuff buff, ISpell ownerSpell)
        {
            var champ = unit as IChampion;
            string dashBase = "Yasuo_Base_E_Dash.troy";
            string dashHit = "Yasuo_Base_E_dash_hit.troy";
            //Skine göre kostüm
            if(champ.Skin == 2)
            {
                dashBase = "Yasuo_Skin1_E_Dash.troy";
                dashHit = "Yasuo_Skin1_E_dash_hit.troy";
            }

            CreateTimer(0.03f, () =>
            {
                unit.IsCastingSpell = true;
            });
                
            CreateTimer(2*buff.Duration / 3, () =>
              {
                  unit.IsCastingSpell = false;
              });
            
            AddParticleTarget(unit, dashBase, unit);
            AddParticleTarget(unit, dashHit, target);
            var to = Vector2.Normalize(target.GetPosition() - unit.GetPosition());
            ownerSpell.DashToLocation(unit, target.X + to.X * 300f, target.Y + to.Y * 300f, 1100f, false, "SPELL3");
            
        }

        public void OnDeactivate(IObjAiBase unit)
        {
            CancelDash(unit);   
        }

        public void OnUpdate(double diff)
        {
            //empty
        }
    }
}
