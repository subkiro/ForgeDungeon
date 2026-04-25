using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ForgePreperation : MonoBehaviour
{
    [SerializeField] List<InventoryItemSO> m_Items;

    [SerializeField] CanvasGroup m_mainGroup;

    [SerializeField] CanvasGroup m_InventoryItemGroup;
    List<InventoryItemCell> m_InvetoryItemCells;


    [SerializeField] ToggleGroup m_ToggleGroup;
     List<CategoryToggleCell> m_CategoryToggle;
    void Awake()
    {
        m_InvetoryItemCells = m_InventoryItemGroup.GetComponentsInChildren<InventoryItemCell>().ToList();
       InitToggles();
    }

    void InitToggles()
    {
        m_CategoryToggle = GetComponentsInChildren<CategoryToggleCell>().ToList();
        m_CategoryToggle.ForEach(x=>x.Initialize(m_ToggleGroup));
        
        m_CategoryToggle[0].Toggle.onValueChanged.AddListener((value)=>OnToggle_0(m_CategoryToggle[0],value));
        m_CategoryToggle[1].Toggle.onValueChanged.AddListener((value)=>OnToggle_1(m_CategoryToggle[1],value));
        m_CategoryToggle[2].Toggle.onValueChanged.AddListener((value)=>OnToggle_2(m_CategoryToggle[2],value));


    }

    void OnToggle_0(CategoryToggleCell toggleCell, bool isOn)
    {
        if(isOn)ShowItems<MaterialSO>(toggleCell);
        
    }
    void OnToggle_1(CategoryToggleCell toggleCell,bool isOn)
    {
        if(isOn)ShowItems<RawResourceSO>(toggleCell);

    }
    void OnToggle_2(CategoryToggleCell toggleCell,bool isOn)
    {
        if(isOn)ShowItems<EndProductSO>(toggleCell);

    }


    void ShowItems<T>(CategoryToggleCell toggleCell)
    {
        var items = m_Items.Where(x=>x is T).ToList();
        m_InvetoryItemCells.ForEach(x=>x.CleanUp());
        for (int i = 0; i < m_InvetoryItemCells.Count && i<items.Count; i++)
        {
            var cell = m_InvetoryItemCells[i];
            var data = items[i];
            cell.Initialize(data);
            cell.Button.onClick.RemoveAllListeners();
            cell.Button.onClick.AddListener(()=>onSelectItem(toggleCell,cell.DataSO));
        }
    }


    void onSelectItem( CategoryToggleCell toggleCell, InventoryItemSO selectedItem)
    {
        toggleCell.SetInvetoryItem(selectedItem);
    }
}
