using GameServerCore.Enums;

namespace GameServerCore.Domain.GameObjects
{
    public interface ILaneTurret : IBaseTurret
    {
        TurretType Type { get; }

        public void CheckTargetable(string tName);

        public void ReEnable(string tName);
    }
}
