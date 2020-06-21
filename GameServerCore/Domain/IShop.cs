namespace GameServerCore.Domain
{
    public interface IShop
    {
        public bool HandleItemSellRequest(byte slotId);
        public bool HandleItemBuyRequest(int itemId);
        public void RemoveItem(IItem item, byte slotId, byte newStacks);
    }
}