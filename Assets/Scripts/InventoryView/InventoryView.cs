using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [SerializeField] CanvasGroup m_Group;
    [SerializeField] CanvasGroup m_ContainerGroup;

   [SerializeField] bool m_Interactable=true;


    void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        GameManager.Instance.Player.PlayerInvetory.OnAdded+=OnAdded;
        LoadInventory();
    }
    private void LoadInventory()
    {
        foreach(var item in GameManager.Instance.Player.PlayerInvetory.Elements)
        {
            OnAdded(item.Key,item.Value);

        }
    }
    private void OnAdded(InventoryItemSO item,int amount)
    {
        Tools.Log($"Player Inventory: {item.DisplayName} Added to Inventory",Color.green);
        var prefab = GameManager.Instance.AssetScriptableData.InventoryItemCell;
        var cell = Instantiate(prefab,m_ContainerGroup.transform);
        cell.Initialize(item);
        if(m_Interactable) cell.Button.onClick.AddListener(()=>OnSelect(cell));

    }

    private void OnSelect(InventoryItemCell itemCell)
    {
        var prefab = GameManager.Instance.AssetScriptableData.MessageItemInfo;
        var cell = PopUpManager.Instance.ShowSimple<MessageItemInfo>(prefab);
        cell.SetData(itemCell.DataSO,OnUserChoice);

        void OnUserChoice(TwoStateChoice choice)
        {
            //choice yes = sold
            //choice no = just closed

            switch (choice)
            {
                case TwoStateChoice.Yes:
                GameManager.Instance.Player.PlayerInvetory.Remove(itemCell.DataSO);
                OnRemoved(itemCell);
                break;
                
                
            }
        }
    }

    private void OnRemoved(InventoryItemCell cell)
    {
        Tools.Log($"Player Inventory: {cell.DataSO.DisplayName} Removed from Inventory",Color.red);

        GameManager.Instance.Player.PlayerInvetory.Remove(cell.DataSO);
        Destroy(cell.gameObject);

    }

    
     private void OnDestroy()
    {
        GameManager.Instance.Player.PlayerInvetory.OnAdded-=OnAdded;
    }


}
