using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageInventoryMarket : PopUp
{
    [SerializeField] Button m_CloseButton;


    [SerializeField] CanvasGroup m_PreviewGroup;
    [SerializeField] ItemCellView m_PreviewCellView;
    [SerializeField] Button m_PreviewBuyButton;
    [SerializeField] TMP_Text m_PreviewDescription;


    [SerializeField] CanvasGroup m_ContentGroup;
    [SerializeField] CanvasGroup m_InventoryItemGroup;
     ToggleGroup m_InventoryItemToggleGroup;
    List<InventoryItemCell_Toggle> m_InvetoryItemCellsToggles;




    [SerializeField] RectTransform m_ToggleGroupRect;
    List<Toggle> m_CategoryToggle;
    private  Dictionary<System.Type,Toggle>_activeFilters = new();

    public void SetData()
    {
        m_CloseButton.onClick.AddListener(()=>_=OnClose());
        InitPreview();
        InitItems();
        InitToggles();
        _ = ShowAnimation();

    }

    private void InitItems()
    {

        m_InventoryItemToggleGroup = m_InventoryItemGroup.gameObject.AddComponent<ToggleGroup>();
        m_InvetoryItemCellsToggles = new List<InventoryItemCell_Toggle>();
        var allItems = GameManager.Instance.AssetScriptableData.dataBaseSO.AllInventoryItems;
        foreach (var item in allItems)
        {
            var prefab = GameManager.Instance.AssetScriptableData.InventoryItemCell_Toggle;
            InventoryItemCell_Toggle cell = Instantiate(prefab, m_InventoryItemGroup.transform);
            cell.Toggle.group = m_InventoryItemToggleGroup;    
            cell.Toggle.isOn = false;
            cell.Toggle.onValueChanged.AddListener((x)=>OnSelectItem(item,x));
            cell.Initialize(item);
            m_InvetoryItemCellsToggles.Add(cell);
        }
    }
    void OnSelectItem(InventoryItemSO item, bool isOn)
    {
        if(isOn) UpdatePreview(item);
        //Show some actions
    }
    #region  Preview

    void InitPreview()
    {
        m_PreviewBuyButton.onClick.AddListener(OnBuyPressed);

    }

    private void OnBuyPressed()
    {
        if(m_PreviewCellView.DataSO==null) return;
        var itemToBuy = m_PreviewCellView.DataSO;


        Tools.Log($"Try to buy <color=orange>{itemToBuy.DisplayName}</color> and update the player inventory");
    }

    void UpdatePreview(InventoryItemSO item)
    {
        m_PreviewCellView.Initialize(item);
        string itemName = item.DisplayName;
        string rarity = $"<color=#{ColorUtility.ToHtmlStringRGB(item.RarityData.color)}> {item.RarityData.Rarity}</color>";
        string final = $"{itemName} {rarity}\n{ item.Descripton}";
        m_PreviewDescription.text = final.TrimStart();
        m_PreviewCellView.Icon.rectTransform.DOPunchScale(Vector3.one*.1f,.1f,vibrato:0).SetLink(this.gameObject);

    }
    #endregion
    
    #region Toggle
    void InitToggles()
    {
        m_CategoryToggle = m_ToggleGroupRect.GetComponentsInChildren<Toggle>().ToList();
        _activeFilters.Add(typeof(MaterialSO),m_CategoryToggle[0]);
        _activeFilters.Add(typeof(RawResourceSO),m_CategoryToggle[1]);
        _activeFilters.Add(typeof(EndProductSO),m_CategoryToggle[2]);

        _activeFilters[typeof(MaterialSO)].onValueChanged.AddListener(OnMaterialToggle);
        _activeFilters[typeof(RawResourceSO)].onValueChanged.AddListener(OnRawToggle);
        _activeFilters[typeof(EndProductSO)].onValueChanged.AddListener(OnEndToggle);

        m_CategoryToggle[0].isOn = true;
        m_CategoryToggle[1].isOn = true;
        m_CategoryToggle[2].isOn = true;

    }

    public void OnMaterialToggle(bool isOn) => OnToggle(isOn, typeof(MaterialSO));
    public void OnRawToggle(bool isOn) => OnToggle(isOn, typeof(RawResourceSO));
    public void OnEndToggle(bool isOn) => OnToggle(isOn, typeof(EndProductSO));
    void OnToggle(bool isOn, System.Type type)
    {
     
        RefreshView();
    }


    #endregion
    void RefreshView()
    {
        foreach (var cell in m_InvetoryItemCellsToggles)
        {
            var type =  cell.ItemCellView.DataSO.GetType();
            cell.gameObject.SetActive(_activeFilters[type].isOn);
        }
    }
    







    #region  Show/Hide Animations
    private async Awaitable ShowAnimation()
    {

        //Show Animation
        Sequence s = DOTween.Sequence();
        s.SetId(this);
        s.OnStart(() =>
        {
            GameManager.Instance.SoundManager.PlayGivenSound("Pop", volume: 0.2f);
        });
        ;
        s.Append(m_ContentGroup.DOFade(1, 0.2f).From(0));
        s.Join(m_ContentGroup.transform.RectTransform().DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.2f, vibrato: 8).SetEase(Ease.OutElastic));

        await s.ToAwaitable();
    }
    private async Awaitable OnClose()
    {
        m_ContentGroup.interactable = false;
        await HideAnimation().ToAwaitable();
        OnCompleteBase?.Invoke();
    }

    private Tween HideAnimation()
    {

        //Show Animation
        Sequence s = DOTween.Sequence();
        s.SetId(this);
        s.OnStart(() =>
        {
            GameManager.Instance.SoundManager.PlayGivenSound("Sweesh", volume: 0.1f);
        });
        ;
        s.Append(m_ContentGroup.DOFade(0, 0.1f));
        s.Join(m_ContentGroup.transform.RectTransform().DOScale(0.8f, 0.1f).SetEase(Ease.InBack));

        return s;
    }
    #endregion
}
