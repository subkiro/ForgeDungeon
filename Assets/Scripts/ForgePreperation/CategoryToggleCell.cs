using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryToggleCell : MonoBehaviour
{
    [SerializeField] Toggle m_Toggle;
    public Toggle Toggle=>m_Toggle;
   [SerializeField] Image m_Plus;
   [SerializeField] InventoryItemCell m_InventoryItemCell;
   [SerializeField] TMP_Text m_Title;

    public void Initialize(ToggleGroup group = null)
    {
        m_Toggle = GetComponent<Toggle>();
        if(group!=null) Toggle.group = group;
        CleanUp();
    }

    public void SetInvetoryItem(InventoryItemSO data)
    {
        m_Plus.enabled = false;
        m_InventoryItemCell.Initialize(data);
        
    }

    public void CleanUp()
    {
        m_Plus.enabled=true;
        m_InventoryItemCell.CleanUp();
        
    }
}
