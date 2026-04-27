using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MessageForgePreperation : PopUp
{

    [SerializeField] CanvasGroup m_ContentGroup;
    [SerializeField] CanvasGroup m_InventoryItemGroup;
    List<InventoryItemCell> m_InvetoryItemCells;


    [SerializeField] ToggleGroup m_ToggleGroup;
    List<CategoryToggleCell> m_CategoryToggle;


    [SerializeField] Button m_ButtonClear;

    [SerializeField] Button m_ButtonForge;
    public void SetData()
    {
        m_InvetoryItemCells = m_InventoryItemGroup.GetComponentsInChildren<InventoryItemCell>().ToList();
        m_ButtonClear.onClick.AddListener(OnCleanUp);
        m_ButtonForge.onClick.AddListener(OnForge);
        InitToggles();
        _ = ShowAnimation();

    }
    #region Toggle
    void InitToggles()
    {
        m_CategoryToggle = GetComponentsInChildren<CategoryToggleCell>().ToList();
        m_CategoryToggle.ForEach(x => x.Initialize(m_ToggleGroup));

        m_CategoryToggle[0].Toggle.onValueChanged.AddListener((value) => OnToggle_0(m_CategoryToggle[0], value));
        m_CategoryToggle[1].Toggle.onValueChanged.AddListener((value) => OnToggle_1(m_CategoryToggle[1], value));
        m_CategoryToggle[2].Toggle.onValueChanged.AddListener((value) => OnToggle_2(m_CategoryToggle[2], value));

        m_CategoryToggle[0].Toggle.isOn=true;

    }

    void OnToggle_0(CategoryToggleCell toggleCell, bool isOn)
    {
        if (isOn) ShowItems<MaterialSO>(toggleCell);

    }
    void OnToggle_1(CategoryToggleCell toggleCell, bool isOn)
    {
        if (isOn) ShowItems<RawResourceSO>(toggleCell);

    }
    void OnToggle_2(CategoryToggleCell toggleCell, bool isOn)
    {
        if (isOn) ShowItems<EndProductSO>(toggleCell);

    }

    #endregion
    void ShowItems<T>(CategoryToggleCell toggleCell) where T : InventoryItemSO
    {
        var items = GameManager.Instance.AssetScriptableData.dataBaseSO.InventoryItems<T>();
        m_InvetoryItemCells.ForEach(x => x.CleanUp());
        for (int i = 0; i < m_InvetoryItemCells.Count && i < items.Count; i++)
        {
            var cell = m_InvetoryItemCells[i];
            var data = items[i];
            cell.Initialize(data);
            cell.Button.onClick.RemoveAllListeners();
            cell.Button.onClick.AddListener(() => OnSelectItem(toggleCell, cell.DataSO));
        }
    }
    void OnSelectItem(CategoryToggleCell toggleCell, InventoryItemSO selectedItem)
    {
        toggleCell.SetInvetoryItem(selectedItem);
    }

    void OnCleanUp()
    {
        m_CategoryToggle.ForEach(x => x.CleanUp());
    }


    void OnForge()
    {
       _= OnClose();
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
