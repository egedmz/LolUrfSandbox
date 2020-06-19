using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using System;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.Spells;
using System.Collections.Generic;

namespace Spells
{
    public class AkaliSmokeBomb : IGameScript
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
            (spell as Spell)._game.PacketNotifier.NotifySetAnimation(owner, new List<string> { "RUN" , "Run_sneak" });
            var smokeBomb = AddParticle(owner, "akali_smoke_bomb_tar.troy", owner.X, owner.Y);

            var enemyTeam = owner.Team == TeamId.TEAM_BLUE ? TeamId.TEAM_BLUE : TeamId.TEAM_PURPLE;
            var smokeBombBorder = AddParticle(owner, "akali_smoke_bomb_tar_team_green.troy", owner.X, owner.Y, forTeam:owner.Team);
            var smokeBombBorderEnemy = AddParticle(owner, "akali_smoke_bomb_tar_team_red.troy", owner.X, owner.Y, forTeam: enemyTeam);
            AddParticle(owner, "akali_invis_cas.troy", owner.X, owner.Y);
            AddBuff("AkaliWBuff", 8.0f, 1, spell, owner, owner);
            CreateTimer(8.0f, () =>
            {
                RemoveParticle(smokeBomb);
                RemoveParticle(smokeBombBorder);
                RemoveParticle(smokeBombBorderEnemy);
            });
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, IProjectile projectile)
        {
        }

        public void OnUpdate(double diff)
        {
        }
    }
}
