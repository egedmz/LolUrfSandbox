using System;
using System.Collections.Generic;
using System.Timers;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.Spells;

namespace LeagueSandbox.GameServer.GameObjects.AttackableUnits.Buildings.AnimatedBuildings
{
    public class Inhibitor : ObjAnimatedBuilding, IInhibitor
    {
        private Timer _respawnTimer;
        public InhibitorState InhibitorState { get; private set; }
        private const double RESPAWN_TIMER = 5 * 60 * 1000;
        private const double RESPAWN_ANNOUNCE = 1 * 60 * 1000;
        private const float GOLD_WORTH = 50.0f;
        private DateTime _timerStartTime;

        public List<string> conds;
        public bool RespawnAnnounced { get; private set; } = true;

        // TODO assists
        public Inhibitor(
            Game game,
            string model,
            TeamId team,
            int collisionRadius = 40,
            float x = 0,
            float y = 0,
            int visionRadius = 0,
            uint netId = 0,
            List<string> conditions = null
        ) : base(game, model, new Stats.Stats(), collisionRadius, x, y, visionRadius, netId)
        {
            Stats.CurrentHealth = 4000;
            Stats.HealthPoints.BaseValue = 4000;
            InhibitorState = InhibitorState.ALIVE;
            SetTeam(team);
            conds = conditions;
            if (conds != null)
            {
                Stats.IsTargetableToTeam = SpellFlags.NonTargetableAll;
            }
        }

        public override void OnAdded()
        {
            base.OnAdded();
            _game.ObjectManager.AddInhibitor(this);
        }

        public override void Die(IAttackableUnit killer)
        {
            var objects = _game.ObjectManager.GetObjects().Values;
            foreach (var obj in objects)
            {
                var u = obj as IObjAiBase;
                if (u != null && u.TargetUnit == this)
                {
                    u.SetTargetUnit(null);
                    u.AutoAttackTarget = null;
                    u.IsAttacking = false;
                    _game.PacketNotifier.NotifySetTarget(u, null);
                    u.HasMadeInitialAttack = false;
                }
            }

            _respawnTimer?.Stop();
            _respawnTimer = new Timer(RESPAWN_TIMER) {AutoReset = false};

            _respawnTimer.Elapsed += (a, b) =>
            {
                Stats.CurrentHealth = Stats.HealthPoints.Total;
                SetState(InhibitorState.ALIVE);
                IsDead = false;
                foreach (var obj in _game.ObjectManager.GetObjects())
                {
                    if (obj.Value is ILaneTurret turret) turret.ReEnable(this.Model);
                    if (obj.Value is INexus nexus) nexus.ReEnable(this.Model);
                }
            };
            _respawnTimer.Start();
            _timerStartTime = DateTime.Now;

            if (killer is IChampion c)
            {
                c.Stats.Gold += GOLD_WORTH;
                _game.PacketNotifier.NotifyAddGold(c, this, GOLD_WORTH);
            }

            SetState(InhibitorState.DEAD, killer);
            RespawnAnnounced = false;

            foreach (var obj in _game.ObjectManager.GetObjects())
            {
                if (obj.Value is ILaneTurret turret) turret.CheckTargetable(this.Model);
                if (obj.Value is INexus nexus) nexus.CheckTargetable(this.Model);
            }
            base.Die(killer);
        }
        public void CheckTargetable(string tName)
        {
            if (conds is null) return;
            if (Stats.IsTargetableToTeam == SpellFlags.TargetableToAll) return;
            foreach (var cond in conds)
            {
                if (cond.Equals(tName))
                {
                    Stats.IsTargetableToTeam = SpellFlags.TargetableToAll;
                }
            }
        }
        public void SetState(InhibitorState state, IGameObject killer = null)
        {
            if (_respawnTimer != null && state == InhibitorState.ALIVE)
            {
                _respawnTimer.Stop();
            }

            InhibitorState = state;
            _game.PacketNotifier.NotifyInhibitorState(this, killer);
        }

        public double GetRespawnTimer()
        {
            var diff = DateTime.Now - _timerStartTime;
            return RESPAWN_TIMER - diff.TotalMilliseconds;
        }

        public override void Update(float diff)
        {
            if (!RespawnAnnounced && InhibitorState == InhibitorState.DEAD && GetRespawnTimer() <= RESPAWN_ANNOUNCE)
            {
                _game.PacketNotifier.NotifyInhibitorSpawningSoon(this);
                RespawnAnnounced = true;
            }

            base.Update(diff);
        }

        public override void SetToRemove()
        {

        }

        public override float GetMoveSpeed()
        {
            return 0;
        }

    }
}
