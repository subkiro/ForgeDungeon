using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class MessageForgePreperation : PopUp
{

    [SerializeField] CanvasGroup m_ContentGroup;
    [SerializeField] CanvasGroup m_InventoryItemGroup;
    List<InventoryItemCell> m_InvetoryItemCells;


    [SerializeField] ToggleGroup m_ToggleGroup;
    Dictionary<System.Type, CategoryToggleCell> m_CategoryToggle;


    [SerializeField] Button m_ButtonClear;

    [SerializeField] Button m_ButtonForge;
    public void SetData()
    {
        m_ButtonClear.onClick.AddListener(OnCleanUp);
        m_ButtonForge.onClick.AddListener(OnForge);
        InitItems();
        InitToggles();
        _ = ShowAnimation();

    }

    private void InitItems()
    {

        m_InvetoryItemCells = new List<InventoryItemCell>();
        var allItems = GameManager.Instance.AssetScriptableData.dataBaseSO.AllInventoryItems;
        foreach (var item in allItems)
        {
            var prefab = GameManager.Instance.AssetScriptableData.InventoryItemCell;
            InventoryItemCell cell = Instantiate(prefab, m_InventoryItemGroup.transform);
            cell.Initialize(item);

            cell.Button.onClick.AddListener(() => OnSelectItem(item));
            m_InvetoryItemCells.Add(cell);
        }
    }


    #region Toggle
    void InitToggles()
    {
        var toggles = m_ToggleGroup.gameObject.GetComponentsInChildren<CategoryToggleCell>().ToList();
        m_CategoryToggle = new Dictionary<System.Type, CategoryToggleCell>();

        m_CategoryToggle.Add(typeof(MaterialSO), toggles[0]);
        m_CategoryToggle.Add(typeof(RawResourceSO), toggles[1]);
        m_CategoryToggle.Add(typeof(EndProductSO), toggles[2]);

        m_CategoryToggle[typeof(MaterialSO)].Toggle.onValueChanged.AddListener(OnMaterialToggle);
        m_CategoryToggle[typeof(RawResourceSO)].Toggle.onValueChanged.AddListener(OnRawToggle);
        m_CategoryToggle[typeof(EndProductSO)].Toggle.onValueChanged.AddListener(OnEndToggle);

        m_CategoryToggle[typeof(MaterialSO)].Toggle.isOn = true;

    }

    public void OnMaterialToggle(bool isOn) => OnToggle(isOn, typeof(MaterialSO));
    public void OnRawToggle(bool isOn) => OnToggle(isOn, typeof(RawResourceSO));
    public void OnEndToggle(bool isOn) => OnToggle(isOn, typeof(EndProductSO));
    void OnToggle(bool isOn, System.Type type)
    {
        if (isOn)
            RefreshView(type);
    }


    void RefreshView(System.Type type)
    {
        foreach (var cell in m_InvetoryItemCells)
        {
            cell.gameObject.SetActive(type.IsAssignableFrom(cell.DataSO.GetType()));
        }
    }




    #endregion

    void OnSelectItem(InventoryItemSO selectedItem)
    {
        m_CategoryToggle[selectedItem.GetType()].SetInvetoryItem(selectedItem);
    }

    void OnCleanUp()
    {
        m_CategoryToggle.Values.ForEach(x => x.CleanUp());
    }


    void OnForge()
    {
        _ = OnClose();
        var messagePrefab = GameManager.Instance.AssetScriptableData.MessageForgeRoom;
        var message = PopUpManager.Instance.ShowSimple<MessageForgeRoom>(messagePrefab);
        message.SetData();

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
