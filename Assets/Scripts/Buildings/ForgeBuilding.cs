public class ForgeBuilding: BuildingBase
{

    void OnMouseDown()
    {
       var messagePrefab =  GameManager.Instance.AssetScriptableData.MessageForgeRoom;
       var message = PopUpManager.Instance.ShowSimple<MessageForgeRoom>(messagePrefab);
       message.SetData();
    }
}
