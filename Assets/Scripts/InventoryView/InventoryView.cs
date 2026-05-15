using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [SerializeField] CanvasGroup m_Group;
    [SerializeField] CanvasGroup m_ContainerGroup;

    [SerializeField] bool m_Interactable=true;
    private Dictionary<InventoryItemSO,ItemCellView>m_elements = new();

    void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        GameManager.Instance.Player.PlayerInvetory.OnAdded+=OnAdded;
        GameManager.Instance.Player.PlayerInvetory.OnRemoved+=OnRemoved;

        LoadInventory();
    }

    private void OnRemoved(KeyValuePair<InventoryItemSO, int> keyValuePair, int amountRemoved)
    {
        InventoryItemSO item = keyValuePair.Key;
        int totalAmount = keyValuePair.Value;
        var cell = m_elements[item];

        Tools.Log($"Player Inventory: {cell.DataSO.DisplayName} Removed from Inventory",Color.red);

        GameManager.Instance.SoundManager.PlayGivenSound("WooblePop",pitch: 1.3f);

        if (totalAmount == 0)
        {
            m_elements.Remove(item);
            AnimateRemove(cell).OnComplete(() =>
            {
              Destroy(cell.gameObject);
            });

        }
        else
        {
            m_elements[item].Initialize(item,totalAmount);
        }
    }

    private void LoadInventory()
    {
        foreach(var item in GameManager.Instance.Player.PlayerInvetory.Elements)
        {
     
            OnAdded(item,0);
                   
        }
    }
    private void OnAdded(KeyValuePair<InventoryItemSO,int> keyValuePair,int amountAdded)
    {
        InventoryItemSO item = keyValuePair.Key;
        int totalAmount = keyValuePair.Value;

        ItemCellView cell = null;
        GameManager.Instance.SoundManager.PlayGivenSound("WooblePop",pitch: 1.3f);

        if (!m_elements.ContainsKey(item))
        {
           var prefab = GameManager.Instance.AssetScriptableData.ItemCell_View;
           cell = Instantiate(prefab,m_ContainerGroup.transform); 
           m_elements.Add(item,cell);
           AnimateAdd(cell);
        }
        else
        {
            cell = m_elements[item];
            AnimateUpdate(cell);
        }
        
        m_elements[item].Initialize(item,totalAmount);

        Tools.Log($"Player Inventory: {item.DisplayName} Added {amountAdded} to Inventory",Color.green);

        if(m_Interactable) {
            if(!m_elements[item].gameObject.TryGetComponent(out Button button))
            {
                button = m_elements[item].gameObject.AddComponent<Button>();
                button.onClick.AddListener(()=>OnSelect(m_elements[item]));
            }
            
        }

        

    }

    private void OnSelect(ItemCellView itemCell)
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
                //GameManager.Instance.Player.PlayerInvetory.Remove(itemCell.DataSO);
                break;
                
                
            }
        }
    }

    Tween AnimateAdd(ItemCellView item)
    {
        return item.transform.DOScale(1,.2f).From(0.5f).SetEase(Ease.OutBack).SetLink(item.gameObject);
    }
     Tween AnimateUpdate(ItemCellView item)
    {
        return item.transform.DOPunchScale(Vector2.one*.1f,.2f).SetEase(Ease.OutBack).SetLink(item.gameObject);
    }
    Tween AnimateRemove(ItemCellView item)
    {
        return item.transform.DOScale(0,.2f).SetEase(Ease.InBack).SetLink(item.gameObject);
    }
    
     private void OnDestroy()
    {
        GameManager.Instance.Player.PlayerInvetory.OnAdded-=OnAdded;
        GameManager.Instance.Player.PlayerInvetory.OnRemoved-=OnRemoved;

    }


}
