namespace GameServerCore.Domain.GameObjects
{
    public interface INexus : IObjAnimatedBuilding
    {
        public void ReEnable(string tName);
        public void CheckTargetable(string tName);
    }
}
