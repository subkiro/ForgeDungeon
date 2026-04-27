public class ForgeBuilding: BuildingBase
{

    void OnMouseDown()
    {
       Show_MessageForgePreperation();
    }

    public void Show_MessageForgePreperation()
    {
       var messagePrefab =  GameManager.Instance.AssetScriptableData.MessageForgePreperation;
       var message = PopUpManager.Instance.ShowSimple<MessageForgePreperation>(messagePrefab);
       message.SetData();
    }


}
