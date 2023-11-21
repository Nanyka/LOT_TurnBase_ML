namespace JumpeeIsland
{
    public class PlaceCollectableButton : BuildingBuyButton
    {
        public override void ClickYes()
        {
            SavingSystemManager.Instance.OnSpawnCollectable(m_BuidlingItem.inventoryName, _buildingPosition, 0);
        }
    }
}