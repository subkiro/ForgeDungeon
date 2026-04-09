using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MessageTeachingFocus : PopUp
{

    public RectTransform MainInfoUI;
    public CanvasGroup MainInfoGroup;
    public TMP_Text MessageTextUI;
    public RectTransform MessageRect;
    public Button PositiveButton, OverlayButton;
    public RectTransform focusRect, focusRectContainer;
    private UnityAction OnComplete;

    [Space]
    public CanvasGroup OverlayButtonGroup;
    public CanvasGroup FocusrRectContainerGroup;
    public CanvasGroup FocusCutoutGroup;
    public CanvasGroup MessageGroup;
    public CanvasGroup RayCastBlockerGroup;
    public CanvasGroup Glow;
    public RectTransform CutOutMask;
    public RectTransform FingerPointer;
    private Vector3 FingerPointDefaultScale;

    [Space]
    public TutorialSpeech tutorialSpeechScript;
    private Camera mainCamera;
    private float m_pixelMultiplier_Default;





    private void Awake()
    {
        tutorialData = new TutorialData { completedState = CompleteState.NormalCompleted, ReadyToFollow = TutorialFocusData.FollowTypes.None };
        MessageGroup.alpha = 0;
        FocusCutoutGroup.alpha = 0;
        OverlayButtonGroup.alpha = 0;
        RayCastBlockerGroup.alpha = 0;
        Glow.alpha = 0;
        FingerPointDefaultScale = FingerPointer.localScale;
        Image CoutOutImage = FocusCutoutGroup.GetComponent<Image>();

        m_pixelMultiplier_Default = CoutOutImage.pixelsPerUnitMultiplier;
        CutOutMask.gameObject.SetActive(false);


    }
    private void Start()
    {
        mainCamera = Camera.main;

        //TAP = LocaleManager.Instance.GetLocaString("GENERAL_TAP");
        // TAP_HERE = LocaleManager.Instance.GetLocaString("GENERAL_TAP_HERE");
        // CONTINUE = LocaleManager.Instance.GetLocaString("GENERAL_TAP_TO_CONTINUE");


    }
    public override void Show(UnityAction OnComplete)
    {
        this.OnComplete = OnComplete;

    }




    /// <summary>
    /// example of executing simple chain showing a popUp: 
    /// message.AddFocusData(rect1).AddTapAction(() => { ToolsGamesNGames.Log("Action Complete"); }).AddSpeechData(new TutorialSpeech.SpeechData()).ShowPopUp();
    /// MUST END WITH SHOWPOPUP
    /// 
    ///example of executing simple chain showing a popUp IN QUEUE: 
    /// message.AddFocusData(rect1).AddTapAction(() => { ToolsGamesNGames.Log("Action Complete"); }).AddSpeechData(new TutorialSpeech.SpeechData()).ShowPopUp(isQueue: true).ShowPopUpQueue();
    /// </summary>
    /// 


    #region CHANING PUBLIC FUNCTIONS

    private TutorialData tutorialData, ActiveTutorialData;



    public MessageTeachingFocus AddSpeechData(string Speechtext, TextAnchor aligment = TextAnchor.UpperLeft, int spriteIdex = 0, int prefered_width = 700, float anchorMinX = 0, float anchorMaxX = 1, float anchorMinY = 0.1f, float anchorMaxY = 0.9f)
    {
        tutorialData.speechData = new TutorialSpeechData(spriteIdex, Speechtext, aligment, prefered_width, anchorMin: new Vector2(anchorMinX, anchorMinY), anchorMax: new Vector2(anchorMaxX, anchorMaxY));
        return this;
    }



    public MessageTeachingFocus AddFocusData(Transform targetTrans, string messageText = "", int width = 0, int height = 0, float margins = 100, Extensions.AnchorPresets aligment = Extensions.AnchorPresets.TopCenter, TutorialFocusData.FollowTypes FollowTarget = TutorialFocusData.FollowTypes.None, Extensions.AnchorPresets fingerAligment = Extensions.AnchorPresets.None, bool Breath = false, TutorialFocusData.HiglightType highlightType = TutorialFocusData.HiglightType.Fadeded,float scaleMultiplier = 1)
    {
        tutorialData.focusData = new TutorialFocusData(
            targetTrans: targetTrans,
            customWidth: width,
            customHeight: height,
            margins: margins,
            messageText: messageText,
            textAligment: aligment,
            followType: FollowTarget,
            fingerAligment: fingerAligment,
            Breath: Breath,
            highlightType: highlightType,
            scaleMultiplier: scaleMultiplier
            );
        return this;
    }




    public MessageTeachingFocus AddPrefabData(GameObject prefab, bool snapToFocus = true)
    {
        tutorialData.prefabData = new TutorialPrefabData(prefab, snapToFocus);
        return this;
    }
    public MessageTeachingFocus AddOnStart(UnityAction startAction)
    {
        tutorialData.OnStart = startAction;
        return this;
    }
    public MessageTeachingFocus AddOnFinish(UnityAction<CompleteState> finishAction)
    {
        tutorialData.OnFinish = finishAction;
        return this;
    }

    public MessageTeachingFocus AddDelay(float Delay)
    {
        tutorialData.Delay = Delay;
        return this;
    }
    public MessageTeachingFocus AddTransparency(float alphaValue = 0.8f, bool blockRaycast = true, bool isInteractable = true, bool CloseOnAnyClick = false)
    {

        tutorialData.tutorialTransparencyData = new TutorialTransparencyData(alphaValue, blockRaycast, isInteractable, CloseOnAnyClick);


        return this;
    }
    public MessageTeachingFocus AddEventListener(string eventName)
    {
        tutorialData.Event = eventName;
        return this;
    }
    public MessageTeachingFocus ShowPopUp(bool IsQueue = false)
    {

        if (IsQueue) //If the show pop up is added to queue, then when you finish importing popups, you need to call "StartShowingFromQueue"
        {
            SetDataAddToQueue(tutorialData);
        }
        else
        {
            SetData(tutorialData);

        }

        return this;
    }

    private void SetData(TutorialData tutData)
    {


        ActiveTutorialData = tutData;
        DOVirtual.DelayedCall(ActiveTutorialData.Delay, Setup);

    }

    private void Setup()
    {
        TransparencySetup();
        EventSetup();
        FocusSetup();
        TutorialSpeechSetup(ActiveTutorialData.speechData);
        PrefabDataSetup(ActiveTutorialData.prefabData);
        ActiveTutorialData.OnStart?.Invoke();
    }


    public void ShowPopUpQueue()
    {

        DequeueNext();

    }
    #endregion

    #region PRIVATE SETUP FUNCITIONS

    private GameObject temp_Prefab;
    private void PrefabDataSetup(TutorialPrefabData prefabData)
    {
        if (prefabData == null) return;



        temp_Prefab = Instantiate(prefabData.prefab, this.transform);
        RectTransform prefabRect = temp_Prefab.transform.RectTransform();

        if (ActiveTutorialData.focusData != null)
        {

            if (prefabData.snapToFocus)
            {
                RectTransform target = (RectTransform)ActiveTutorialData.focusData.targetTrans;

                Vector2 OriginalSize = new Vector2(target.rect.width, target.rect.height);
                prefabRect.pivot = target.pivot;

                prefabRect.transform.DOMove(target.position, 0);
                prefabRect.DOSizeDelta(OriginalSize, 0);
            }
            else
            {
                prefabRect.transform.DOLocalMove(Vector3.zero, 0);
            }

        }
        else
        {
            prefabRect.transform.DOLocalMove(Vector3.zero, 0);
        }


    }
    private void EventSetup()
    {
        if (ActiveTutorialData.Event.IsNullOrEmpty())
        {
            return;
        }
        else
        {
            EventManager.StartListening(ActiveTutorialData.Event, OnCompleteAction);
        }
    }
    private void TutorialSpeechSetup(TutorialSpeechData data)
    {
        if (data.IsNull())
        {
            tutorialSpeechScript.MainGroup.gameObject.SetActive(false);

        }
        else
        {
            tutorialSpeechScript.MainGroup.gameObject.SetActive(true);
            tutorialSpeechScript.SetData(data);
        }
    }
    private void FocusSetup()
    {
        if (ActiveTutorialData.focusData.IsNull())
        {
            FocusrRectContainerGroup.alpha = 0;

            return;
        }
        else
        {
            FocusrRectContainerGroup.alpha = 1;
            PositiveButton.interactable = false;
            Glow.alpha = 0;
            MessageTextUI.text = ActiveTutorialData.focusData.MessageText;
            SetUpFocusTextAligment(ActiveTutorialData.focusData);
            SetUpFingerAligment(ActiveTutorialData.focusData.fingerAligment);


            AnimateFocus(ActiveTutorialData.focusData);
        }
    }



    private void TransparencySetup()
    {


        TutorialTransparencyData data = ActiveTutorialData.tutorialTransparencyData;

        //temporary disable this functionality



        if (ActiveTutorialData.tutorialTransparencyData == null)
        {
            data = new TutorialTransparencyData(0.8f);
        }

        bool temp_onAnyClick = data.CloseOnAnyClick;
        data.CloseOnAnyClick = false;

        CanvasGroup group;
        if (ActiveTutorialData.focusData.IsNull())
        {
            PositiveButton.gameObject.SetActive(false);
            OverlayButton.onClick.RemoveAllListeners();
            OverlayButton.gameObject.SetActive(true);
            OverlayButton.onClick.AddListener(() => OnCompleteAction());
            group = OverlayButtonGroup;
            group.blocksRaycasts = true;
            group.interactable = false;

        }
        else
        {
            OverlayButton.gameObject.SetActive(false);
            PositiveButton.gameObject.SetActive(true);
            PositiveButton.onClick.RemoveAllListeners();
            PositiveButton.onClick.AddListener(() => OnCompleteAction());
            group = FocusCutoutGroup;
            group.blocksRaycasts = true;
            group.interactable = false;



        }



        

        Tools.Log($"Group name {group.name} Alpha :  {data.alpha}");
        group.DOFade(data.alpha, 0.4f).SetId(this).OnStart(() => { CutOutMask.gameObject.SetActive(true); }).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
        {

            group.blocksRaycasts = data.BlockRaycast;
            group.interactable = data.Interactable;
            RayCastBlockerGroup.blocksRaycasts = data.BlockRaycast;
            data.CloseOnAnyClick = temp_onAnyClick;

        });




    }



    private void ResetData()
    {
        tutorialData = new TutorialData { completedState = CompleteState.NormalCompleted, ReadyToFollow = TutorialFocusData.FollowTypes.None };
    }
    public void SetDataAddToQueue(TutorialData tutData)
    {

        EnqueueAction(() => SetData(tutData));
        ResetData();



    }
    private void SetUpFingerAligment(Extensions.AnchorPresets fingerAligment)
    {
        FingerPointer.gameObject.SetActive(false);


        if (fingerAligment == Extensions.AnchorPresets.None)
        {
            return;
        }
        else
        {


            FingerPointer.gameObject.SetActive(true);

        }


        switch (fingerAligment)
        {
            case Extensions.AnchorPresets.None:
                FingerPointer.transform.localScale = new Vector3(1, 1, 1);
                return;
            case Extensions.AnchorPresets.MiddleCenter:
            case Extensions.AnchorPresets.TopLeft:
            case Extensions.AnchorPresets.MiddleLeft:
            case Extensions.AnchorPresets.BottomLeft:
                FingerPointer.SetAnchor(fingerAligment);
                FingerPointer.transform.localScale = new Vector3(1, 1, 1);
                break;
            case Extensions.AnchorPresets.TopRight:
            case Extensions.AnchorPresets.MiddleRight:
            case Extensions.AnchorPresets.BottomRight:
                FingerPointer.SetAnchor(fingerAligment);
                FingerPointer.transform.localScale = new Vector3(1, 1, 1);
                break;
            case Extensions.AnchorPresets.TopCenter:
            case Extensions.AnchorPresets.BottomCenter:
                FingerPointer.SetAnchor(fingerAligment);
                FingerPointer.transform.localScale = new Vector3(-1, 1, 1);
                break;

        }


        switch (fingerAligment)
        {
            case Extensions.AnchorPresets.None:
                FingerPointer.transform.localEulerAngles = new Vector3(0, 0, 0);
                return;
            case Extensions.AnchorPresets.TopLeft:
                FingerPointer.transform.localEulerAngles = new Vector3(0, 0, -25);
                break;
            case Extensions.AnchorPresets.MiddleRight:
            case Extensions.AnchorPresets.MiddleLeft:
                FingerPointer.transform.localEulerAngles = new Vector3(0, 0, 0);
                break;
            case Extensions.AnchorPresets.BottomLeft:
                FingerPointer.transform.localEulerAngles = new Vector3(0, 0, 25);
                break;
            case Extensions.AnchorPresets.TopRight:
                FingerPointer.transform.localEulerAngles = new Vector3(0, 0, 25);
                break;
            case Extensions.AnchorPresets.BottomRight:
                FingerPointer.transform.localEulerAngles = new Vector3(0, 0, 40);
                break;
            case Extensions.AnchorPresets.MiddleCenter:
                FingerPointer.transform.localEulerAngles = new Vector3(0, 0, 25);
                break;
            case Extensions.AnchorPresets.TopCenter:
                FingerPointer.transform.localEulerAngles = new Vector3(0, 0, 90);
                break;
            case Extensions.AnchorPresets.BottomCenter:
                FingerPointer.transform.localEulerAngles = new Vector3(0, 0, -90);
                break;
        }
    }
    private void SetUpFocusTextAligment(TutorialFocusData m_data)
    {

        MessageRect.SetAnchor(m_data.childAligment);
        switch (m_data.childAligment)
        {
            case Extensions.AnchorPresets.TopLeft:
                MessageRect.SetPivot(Extensions.PivotPresets.BottomRight);
                break;
            case Extensions.AnchorPresets.TopCenter:
                MessageRect.SetPivot(Extensions.PivotPresets.BottomCenter);
                break;
            case Extensions.AnchorPresets.TopRight:
                MessageRect.SetPivot(Extensions.PivotPresets.BottomLeft);
                break;
            case Extensions.AnchorPresets.MiddleLeft:
                MessageRect.SetPivot(Extensions.PivotPresets.MiddleRight);
                break;
            case Extensions.AnchorPresets.MiddleCenter:
                MessageRect.SetPivot(Extensions.PivotPresets.MiddleCenter);
                break;
            case Extensions.AnchorPresets.MiddleRight:
                MessageRect.SetPivot(Extensions.PivotPresets.MiddleLeft);
                break;
            case Extensions.AnchorPresets.BottomLeft:
                MessageRect.SetPivot(Extensions.PivotPresets.TopRight);
                break;
            case Extensions.AnchorPresets.BottomCenter:
                MessageRect.SetPivot(Extensions.PivotPresets.TopCenter);
                break;
            case Extensions.AnchorPresets.BottomRight:
                MessageRect.SetPivot(Extensions.PivotPresets.TopLeft);
                break;

        }
        //switch (m_data.childAligment)
        //{


        //    case TextAnchor.LowerCenter:
        //        MessageRect.anchorMin = new Vector2(0.5f, 0);
        //        MessageRect.anchorMax = new Vector2(0.5f, 0);
        //        MessageRect.pivot = new Vector2(0.5f, 1f);
        //        break;
        //    case TextAnchor.UpperCenter:
        //        MessageRect.anchorMin = new Vector2(0.5f, 1);
        //        MessageRect.anchorMax = new Vector2(0.5f, 1);
        //        MessageRect.pivot = new Vector2(0.5f, 0f);

        //        break;
        //    case TextAnchor.MiddleLeft:

        //        MessageRect.anchorMin = new Vector2(0, 0.5f);
        //        MessageRect.anchorMax = new Vector2(0, 0.5f);
        //        MessageRect.pivot = new Vector2(1, 0.5f);

        //        break;
        //    case TextAnchor.MiddleRight:
        //        MessageRect.anchorMin = new Vector2(1, 0.5f);
        //        MessageRect.anchorMax = new Vector2(1, 0.5f);
        //        MessageRect.pivot = new Vector2(0, 0.5f);

        //        break;

        //}







    }



    private void ShowInternalMessage(string Message)
    {


        if (string.IsNullOrEmpty(Message))
        {
            MessageGroup.alpha = 0;
            return;
        }

        Sequence s = DOTween.Sequence();
        s.SetId(this);
        s.SetUpdate(isIndependentUpdate: true);
        s.Append(MessageRect.transform.DOScale(Vector3.one, 0.3f).From(new Vector3(0.8f, 0.3f, 1)).SetEase(Ease.OutElastic));
        s.Join(MessageGroup.DOFade(1, 0.2f).From(0));



    }
    private void OnCompleteAction()
    {

        CompleteState sate = ActiveTutorialData.completedState;

        if (temp_Prefab != null) Destroy(temp_Prefab);


        if (!ActiveTutorialData.Event.IsNull())
        {
            EventManager.StopListening(ActiveTutorialData.Event, OnCompleteAction);

        }




        if (m_MainQueue.Count > 0)
        {
            ActiveTutorialData.OnFinish?.Invoke(ActiveTutorialData.completedState);
            DequeueNext();
        }
        else
        {
            ActiveTutorialData.OnFinish?.Invoke(ActiveTutorialData.completedState);
            this.OnComplete?.Invoke();
        }



    }

    private void LateUpdate()
    {

        if (ActiveTutorialData.focusData != null)
        {



            if (ActiveTutorialData.focusData.targetTrans.IsRect())
            {

                switch (ActiveTutorialData.ReadyToFollow)
                {
                    case TutorialFocusData.FollowTypes.None:
                        break;
                    case TutorialFocusData.FollowTypes.Position:
                        if (focusRectContainer!=null)
                            focusRectContainer.position = ActiveTutorialData.focusData.targetTrans.position;
                        break;
                    case TutorialFocusData.FollowTypes.Pivot:
                        focusRectContainer.pivot = ((RectTransform)ActiveTutorialData.focusData.targetTrans).pivot;
                        break;

                }
            }
            else
            {

                switch (ActiveTutorialData.ReadyToFollow)
                {
                    case TutorialFocusData.FollowTypes.Position:
                        if (focusRectContainer != null)
                        {
                            var pos = mainCamera.WorldToScreenPoint(ActiveTutorialData.focusData.targetTrans.position);
                            focusRectContainer.position = Vector3.Lerp(focusRectContainer.position,pos,Time.deltaTime*20);
                        }
                        break;
                }



            }
        }




        //if (Input.GetMouseButton(0))
        //{
        //    if (!ActiveTutorialData.tutorialTransparencyData.IsNull())
        //    {
        //        if (ActiveTutorialData.tutorialTransparencyData.CloseOnAnyClick)
        //        {
        //            OnCompleteAction();
        //        }
        //    }
        //}



    }


    private void AnimateFocus(TutorialFocusData focusData)
    {

        Vector2 OriginalSize;
        Vector2 MarginedSize;
        Vector3 Offset;

        if (focusData.targetTrans.IsRect())
        {

            OriginalSize = new Vector2(((RectTransform)focusData.targetTrans).rect.width, ((RectTransform)focusData.targetTrans).rect.height);
            MarginedSize = new Vector2(((RectTransform)focusData.targetTrans).rect.width + focusData.Margins, ((RectTransform)focusData.targetTrans).rect.height + focusData.Margins);
            Offset = focusRect.pivot - ((RectTransform)focusData.targetTrans).pivot;
            focusRectContainer.pivot = ((RectTransform)focusData.targetTrans).pivot;


            //focusRectContainer.CopyPivot(((RectTransform)focusData.targetTrans));
            //focusRectContainer.CopyAnchorPos(((RectTransform)focusData.targetTrans));
            //focusRectContainer.CopyAnchorMinMax(((RectTransform)focusData.targetTrans));


            Tools.Log("focusRectContainer pos : " + focusRectContainer.position);
            Tools.Log("focusRectContainer piv: " + focusRectContainer.pivot);
        }
        else
        {

            Tools.Log("target POSITION : " + focusData.targetTrans.position);

            OriginalSize = new Vector2(focusData.CustomWidth, focusData.CustomHeight);
            MarginedSize = new Vector2(focusData.CustomWidth + focusData.Margins, focusData.CustomHeight + focusData.Margins);
            Offset = Vector3.zero;
            focusRectContainer.pivot = new Vector2(0.5f, 0.5f);
            focusRectContainer.transform.position = GameManager.Instance.CameraManager.MainCamera.WorldToScreenPoint(focusData.targetTrans.position);

        }
        FingerPointer.localScale = FingerPointDefaultScale * focusData.ScaleMultiplier;

        bool isFollowing = (ActiveTutorialData.ReadyToFollow != TutorialFocusData.FollowTypes.None);
        Image CoutOutImage = FocusCutoutGroup.GetComponent<Image>();

        Sequence s = DOTween.Sequence();
        s.SetId(this);
        s.SetUpdate(isIndependentUpdate: true);

        s.Join(DOVirtual.Float(CoutOutImage.pixelsPerUnitMultiplier, focusData.highlightType == TutorialFocusData.HiglightType.WhiteBright ? 10 : m_pixelMultiplier_Default, 0.3f, (value) => { CoutOutImage.pixelsPerUnitMultiplier = value; }).SetUpdate(isIndependentUpdate: true));
        if(focusData.targetTrans.IsRect()) s.Join(focusRectContainer.DOMove(focusData.targetTrans.position, isFollowing ? 0 : 0));
        s.Join(focusRectContainer.DOSizeDelta(OriginalSize, 0));
        s.Join(focusRect.DOSizeDelta(MarginedSize, 0.4f).From(OriginalSize).OnComplete(() =>
        {

            PositiveButton.interactable = true;
            ShowInternalMessage(MessageTextUI.text);
            ActiveTutorialData.ReadyToFollow = focusData.FollowType;

            FingerPointer.DOScale(FingerPointer.localScale, 0.3f).From(FingerPointer.localScale * 1.2f).SetEase(Ease.OutBack).SetDelay(0.3f).SetUpdate(isIndependentUpdate: true).SetLink(FingerPointer.gameObject);

        }));

        s.OnComplete(() =>
        {
            Glow.DOFade(focusData.highlightType == TutorialFocusData.HiglightType.WhiteBright ? 1 : 0, 0.3f).SetUpdate(isIndependentUpdate: true).SetId(this);
            if (focusData.Breath) focusRect.DOScale(1.05f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetUpdate(isIndependentUpdate: true).SetId(this);
        });
    }


    #endregion

    #region QUEUE MANAGMENT
    private Queue<UnityAction> m_MainQueue = new Queue<UnityAction>();



    public void EnqueueAction(UnityAction action)
    {
        m_MainQueue.Enqueue(action);
    }

    public void DequeueNext()
    {
        UnityAction queuedAction = m_MainQueue.Count > 0 ? m_MainQueue.Dequeue() : null;
        queuedAction?.Invoke();
    }
    #endregion

    #region DATA CLASSES

    public struct TutorialData
    {
        public TutorialSpeechData speechData;
        public TutorialFocusData focusData;
        public TutorialPrefabData prefabData;
        public TutorialTransparencyData tutorialTransparencyData;
        public string Event;
        public UnityAction OnStart;
        public UnityAction<CompleteState> OnFinish;

        public CompleteState completedState;
        public float Delay;

        //Temp Data that need to be set to defaults after Action Complete.
        public TutorialFocusData.FollowTypes ReadyToFollow;
    }
    public class TutorialFocusData
    {
        public Transform targetTrans;

        public bool Breath;
        public float Margins;
        public HiglightType highlightType;
        public string MessageText;
        public Extensions.AnchorPresets childAligment;
        public FollowTypes FollowType = FollowTypes.None;

        public bool isCustom = false;
        public int CustomWidth;
        public int CustomHeight;
        public float ScaleMultiplier = 1;   
        public Extensions.AnchorPresets fingerAligment = Extensions.AnchorPresets.None;
        public TutorialFocusData(Transform targetTrans, float margins, string messageText, Extensions.AnchorPresets textAligment, FollowTypes followType, int customWidth, int customHeight, Extensions.AnchorPresets fingerAligment, bool Breath, HiglightType highlightType, float scaleMultiplier)
        {
            this.targetTrans = targetTrans;
            Margins = margins;
            MessageText = messageText;
            childAligment = textAligment;
            FollowType = followType;
            CustomWidth = customWidth;
            CustomHeight = customHeight;
            this.fingerAligment = fingerAligment;
            this.Breath = Breath;
            this.highlightType = highlightType;
            ScaleMultiplier = scaleMultiplier;
        }



        public enum FollowTypes
        {
            None,
            Position,
            Pivot
        }

        public enum HiglightType
        {
            None,
            WhiteBright,
            Fadeded
        }

    }
    public class TutorialPrefabData
    {
        public GameObject prefab;
        public bool snapToFocus;

        public TutorialPrefabData(GameObject prefab, bool snapToFocus)
        {
            this.prefab = prefab;
            this.snapToFocus = snapToFocus;
        }
    }
    public class TutorialTransparencyData
    {
        public float alpha;
        public bool BlockRaycast;
        public bool Interactable;
        public bool CloseOnAnyClick;
        public TutorialTransparencyData(float alpha = 1, bool blockRaycast = true, bool interactable = true, bool CloseOnAnyClick = false)
        {
            this.alpha = alpha;
            BlockRaycast = blockRaycast;
            Interactable = interactable;
            this.CloseOnAnyClick = CloseOnAnyClick;
        }
    }
    public class TutorialSpeechData
    {
        public int spriteIdex;
        public string Speechtext;
        public TextAnchor childAligment;
        public int Prefered_Width;
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
        public TutorialSpeechData()
        {
            this.spriteIdex = 0;
            Speechtext = "Defaulte Text";
            this.childAligment = TextAnchor.UpperLeft;
            Prefered_Width = 1000;
            AnchorMin = Vector2.zero;
            AnchorMax = Vector2.one;
        }

        public TutorialSpeechData(int spriteIdex, string speechtext, TextAnchor aligment, int prefered_width, Vector2 anchorMin, Vector2 anchorMax)
        {
            this.spriteIdex = spriteIdex;
            Speechtext = speechtext;
            this.childAligment = aligment;
            this.Prefered_Width = prefered_width;
            AnchorMin = anchorMin;
            AnchorMax = anchorMax;
        }
    }

    #endregion

    #region ENUMS
    public enum CompleteState
    {
        NormalCompleted,
        Skipped

    }
    #endregion  


}
