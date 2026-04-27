using UnityEngine;
using UnityEngine.UI;

public class ItemCellView : MonoBehaviour
{
    [SerializeField] Image m_BG_Rarity;
    [SerializeField] Image m_MainIcon;
    public Image Icon=>m_MainIcon;
    [SerializeField] CanvasGroup m_Group;

    public InventoryItemSO DataSO { set; get; }
    public void Initialize(InventoryItemSO itemData)
    {

        DataSO = itemData;
        m_MainIcon.enabled = true;
        m_BG_Rarity.enabled = true;


        m_MainIcon.sprite = itemData.Icon;
        m_BG_Rarity.color = itemData.RarityData.color;


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
        m_Group.alpha = alpha;
    }

}


