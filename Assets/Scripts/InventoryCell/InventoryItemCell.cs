using UnityEngine;
using UnityEngine.UI;

public class InventoryItemCell : MonoBehaviour
{
    [SerializeField] Image m_BG_Rarity;
    [SerializeField] Image m_MainIcon;
    
    [SerializeField] CanvasGroup m_Group;
    [SerializeField] Button m_Button;
    public Button Button=>m_Button;
    
    public InventoryItemSO DataSO{set;get;} 
    public void Initialize(InventoryItemSO itemData)
    {

        DataSO = itemData;        
        m_MainIcon.enabled = true;
        m_MainIcon.sprite = itemData.Icon;
        Visiblity(1);
    }

    public void CleanUp()
    {
        m_BG_Rarity.sprite = default;
        m_MainIcon.sprite = default;

        m_BG_Rarity.enabled = false;
        m_MainIcon.enabled = false;

        DataSO = null;
        Visiblity(0.5f);
    }

    public void Visiblity(float alpha)
    {
                    m_Group.alpha =alpha;
    }

}


