using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellPanel : MonoBehaviour
{
    [SerializeField] Button m_Button_Sell;
    public Button ButtonSell=>m_Button_Sell;
    [SerializeField] Button m_Button_Plus;
    [SerializeField] Button m_Button_Minus;
    [SerializeField] TMP_Text m_Amount;
    [SerializeField] TMP_Text m_EarnAmount;


    InventoryItemSO m_item;
    int m_selectedAmount=1;

    void Awake()
    {
        m_Button_Plus.onClick.AddListener(OnPlus);
        m_Button_Minus.onClick.AddListener(OnMinus);
        m_Button_Sell.onClick.AddListener(OnSell);
    }

   
    public void SetData(InventoryItemSO item)
    {
        m_item = item;
        OnUpdateItem(item);
    }
    void OnPlus()
    {
        var totalAmount = GameManager.Instance.Player.PlayerInvetory.Elements[m_item];
        m_selectedAmount = m_selectedAmount + 1 <= totalAmount ? m_selectedAmount + 1 : totalAmount;
        OnUpdateItem(m_item);
    }
    void OnMinus()
    {
        m_selectedAmount = m_selectedAmount - 1 < 1 ? 1 : m_selectedAmount - 1;
        OnUpdateItem(m_item);

    }
    private void OnSell()
    {
        GameManager.Instance.Player.Coins.Value+= ( int) m_item.Cost.Stat_Value;
        GameManager.Instance.Player.PlayerInvetory.Remove(m_item,m_selectedAmount);

    }


    void OnUpdateItem(InventoryItemSO item)
    {
        var totalAmount = GameManager.Instance.Player.PlayerInvetory.Elements[item];
        m_Amount.text = $"{m_selectedAmount}/{totalAmount}";
        m_EarnAmount.text = $"Earn {item.Cost.Stat_Value*m_selectedAmount}<sprite name=coin>";
    }
}
