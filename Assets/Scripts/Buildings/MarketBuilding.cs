using UnityEngine;
using UnityEngine.EventSystems;

public class MarketBuilding : BuildingBase
{
   
    void OnMouseDown()
    {
       if(GameManager.Instance.InteractionState == InteractionState.UI) return;
       Show_MessageInventoryMarket();
    }

    public void Show_MessageInventoryMarket()
    {
       var messagePrefab =  GameManager.Instance.AssetScriptableData.MessageInventoryMarket;
       var message = PopUpManager.Instance.ShowSimple<MessageInventoryMarket>(messagePrefab);
       message.SetData();
    }
}
