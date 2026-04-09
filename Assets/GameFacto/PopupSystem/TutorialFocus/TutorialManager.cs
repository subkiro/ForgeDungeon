using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{


    public GameObject FocusPrefab;
    public bool SaveToPrefs = true;
    [Header("Tutorial Speech")]




    [SerializeField] TutorialID[] ForceShowIDs;
    [SerializeField] TutorialID[] ForceAvoidIDs;




    [SerializeField] CanvasGroup m_CanvasGroup;

    [SerializeField] CanvasGroup m_FaderRectGroup;
    [SerializeField] float  m_FaderRectGroupDefaultAlpha;
    [SerializeField] RectTransform m_FocusRect;

    [SerializeField] CanvasGroup m_MessageGroup;
    [SerializeField] TMP_Text m_MessageText;
    private Vector3 m_MessageGroupInitPos;

    [SerializeField] Image m_Avatar;
    [SerializeField] GameObject m_BuffArea;
    [SerializeField] CanvasGroup m_HandGroup;
    private Animator m_HandAnimator;

    private UnityAction m_OnCompleteAction;



    // Functions that will run on each state
    #region Tutorial Functions
    // IN GAME TUTORIALS

    private void Awake()
    {

        InitItems();

    }


    private void InitItems() {
        DOTween.Kill(this);
        m_CanvasGroup.alpha = 0;
        m_MessageGroup.alpha = 0;
        m_HandGroup.alpha = 0;
        m_MessageGroupInitPos = m_MessageGroup.transform.position;
        m_FaderRectGroupDefaultAlpha = m_FaderRectGroup.alpha;
        m_HandGroup.transform.localScale = Vector3.one;
        m_HandAnimator = m_HandGroup.GetComponentInChildren<Animator>();
        m_BuffArea.SetActive(false);
    }

    public void CallTutorial(TutorialFeatureSO data, UnityAction onCompleteAction =null)
    {
        m_OnCompleteAction = onCompleteAction;


        switch (data.TutorialID)
        {
            case TutorialID.LEVEL1:
               // Tutorial_lEVEL1_01().Forget();
                break;
            case TutorialID.LEVEL2:
               // Tutorial_lEVEL1_02().Forget();
                break;

        }
    }


    #endregion


    // TUTORIAL READ/CHECK FUNCTION
    #region Tutorial Tools
    private bool TutorialStageSetComplete(string Prefix, string ID)
    {

        // PlayerPrefs.SetInt("TopBar_" + rect.name.ToString(), 1);
        bool isComplete = PlayerPrefs.GetString(Prefix + ID, "") != "" ? true : false;

        if (!isComplete && SaveToPrefs)
        {
            PlayerPrefs.SetString("Tutorial_" + Prefix + ID, "Completed");
        }

        return isComplete;

    }

    private bool TutorialStageCheckComplete(string Prefix, string ID)
    {
        return PlayerPrefs.GetString("Tutorial_" + Prefix + ID, "") != "" ? true : false; ;
    }

    #endregion

    public bool IsTutorialComplete(TutorialID id)
    {
        if (ForceShow(id)) return false;
        else if (ForceAvoid(id)) return true;


        return TutorialStageCheckComplete("Tutorial_", id.ToString());

    }

    public bool SetTutorialComplete(TutorialID id)
    {
        return TutorialStageSetComplete("Tutorial_", id.ToString());

    }

    private bool ForceShow(TutorialID id)
    {
        if (ForceShowIDs == null || ForceShowIDs.Length == 0) return false;

        for (int i = 0; i < ForceShowIDs.Length; i++)
        {
            if (ForceShowIDs[i] == id) return true;
        }

        return false;
    }
    private bool ForceAvoid(TutorialID id)
    {
        if (ForceAvoidIDs == null || ForceAvoidIDs.Length == 0) return false;

        for (int i = 0; i < ForceAvoidIDs.Length; i++)
        {
            if (ForceAvoidIDs[i] == id) return true;
        }

        return false;
    }


    
}
public enum TutorialID
{
    LEVEL1,
    LEVEL2,
    UPGRADE,
   
}

