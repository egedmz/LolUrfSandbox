using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.Spells;
using System;
using System.Collections.Generic;

namespace LeagueSandbox.GameServer.GameObjects.AttackableUnits.Buildings.AnimatedBuildings
{
    public class Nexus : ObjAnimatedBuilding, INexus
    {
        public string mInhiName = null;
        public List<string> mTowerNames = null;
        public int exInhis = 0;
        public int exNexusTowers = 0;
        public Nexus(
            Game game,
            string model,
            TeamId team,
            int collisionRadius = 40,
            float x = 0,
            float y = 0,
            int visionRadius = 0,
            uint netId = 0,
            List<string> towersNames = null,
            string inhiName = null
        ) : base(game, model, new Stats.Stats(), collisionRadius, x, y, visionRadius, netId)
        {
            Stats.CurrentHealth = 5500;
            Stats.HealthPoints.BaseValue = 5500;

            SetTeam(team);
            Stats.IsTargetableToTeam = SpellFlags.NonTargetableAll;
            mInhiName = inhiName;
            mTowerNames = towersNames;
        }

        public override void Die(IAttackableUnit killer)
        {
            var cameraPosition = _game.Map.MapProperties.GetEndGameCameraPosition(Team);
            _game.Stop();
            _game.PacketNotifier.NotifyGameEnd(cameraPosition, this, _game.PlayerManager.GetPlayers());
            _game.SetGameToExit();
        }

        public override void SetToRemove()
        {

        }

        public override float GetMoveSpeed()
        {
            return 0;
        }

        public void ReEnable(string tName)
        {
            if(mInhiName.Equals(tName))
            {
                exInhis -= 1;
                if (exInhis == 0)
                {
                    Stats.IsTargetableToTeam = SpellFlags.NonTargetableAll;
                }
            }
        }

        public void CheckTargetable(string tName)
        {
            foreach (var tower in mTowerNames)
            {
                if (tower.Equals(tName))
                {
                    exNexusTowers += 1;
                    mTowerNames.Remove(tName);
                    if (exInhis > 0 && exNexusTowers == 2)
                    {
                        Stats.IsTargetableToTeam = SpellFlags.TargetableToAll;
                    }
                    return;
                }
            }
            if(mInhiName.Equals(tName))
            {
                exInhis += 1;
                if (exInhis > 0 && exNexusTowers == 2)
                {
                    Stats.IsTargetableToTeam = SpellFlags.TargetableToAll;
                }
            }
        }
    }
}
