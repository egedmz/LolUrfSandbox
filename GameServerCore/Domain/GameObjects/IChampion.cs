using System.Collections.Generic;
using GameServerCore.Enums;

namespace GameServerCore.Domain.GameObjects
{
    public interface IChampion : IObjAiBase
    {
        IShop Shop { get; }
        float RespawnTimer { get; }
        float ChampionGoldFromMinions { get; set; }
        IRuneCollection RuneList { get; }
        Dictionary<short, ISpell> Spells { get; }

        public ISpell lastDasher { get; set; }
        int Skin { get; }
        IChampionStats ChampStats { get; }
        byte SkillPoints { get; set; }

        // basic
        void UpdateSkin(int skinNo);
        void StopChampionMovement();
        bool CanMove();
        void UpdateMoveOrder(MoveOrder order);
        bool CanCast();
        void Recall();
        void Respawn();

        // spells
        void SwapSpells(byte slot1, byte slot2);
        void RemoveSpell(byte slot);
        ISpell SetSpell(string name, byte slot, bool enabled = false, IItem refItem = null);
        ISpell GetSpell(byte slot);
        ISpell LevelUpSpell(byte slot);

        void OnKill(IAttackableUnit killed);
    }
}
