using System;
using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Linq;
public class MessageForgeRoom : PopUp
{

    [SerializeField] Button m_ForgeButton;
    [SerializeField] Button m_BackButton;

    [SerializeField] GameObject m_DwarfCharacter;
    [SerializeField] Image m_Forge;
    [SerializeField] ParticleSystem m_ForgeHitEffect;

    Animator m_Animator;

    [SerializeField] RectTransform m_Container;




    [SerializeField] float m_SliderStep = 10;
    int m_SliderCurrentIndex = 0;
    [SerializeField] Slider m_Slider_Base;
    [SerializeField] Slider m_Slider_Timer;

    [SerializeField] List<Slider> m_Sliders;
    [SerializeField] List<string> m_SliderTitles;
    [SerializeField]TMP_Text m_SliderTitleText;
    [SerializeField]TMP_Text m_SliderTimerText;


    [SerializeField] RectTransform m_ResourceContainer;
    [SerializeField] Image m_ResourceImage;


    [SerializeField] float m_power=.3f;
    [SerializeField] string m_SuccesHitID = "2_Attack";
    [SerializeField] float m_hitDuration = 1;
    [SerializeField] Vector2 m_HitRange;
    [SerializeField]float m_HitProgress;
    bool IsSucceshit =>m_HitProgress<=m_HitRange.y && m_HitProgress>m_HitRange.x;
    bool SuccesHit;
    float HitPercent;
    //Energy bar
    [SerializeField] Slider m_Slider_Energy;
    [SerializeField]TMP_Text m_Slider_EnergyText;



    [SerializeField] List<Stats> m_BaseStats;
    Dictionary<StatType,float> m_ActiveStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetData()
    {
        m_Animator = m_DwarfCharacter.GetComponentInChildren<Animator>();
        m_ForgeButton.onClick.AddListener(()=>_=ForgePressed());
        m_BackButton.onClick.AddListener(OnClose);
        PoolSetup();
        InitStas();
        InitSliders();
        InitTimer();
        InitEnergyBar();
        InitSequencer(m_hitDuration);

       
    }

    private void OnClose()
    {
         var param =new MessageChoice.ChoiceParameters();
         param.MiddleText = "Are you sure you want to exit the forge progress?";

         
         GameManager.Instance.UIManager.ShowChoice(param,OnChoicemade);
        
        void OnChoicemade(MessageChoice.ChoiceResult result)
        {
            switch (result.choice)
            {
                case TwoStateChoice.Yes:
                        OnCompleteBase?.Invoke();                
                break;

            }
        }
       
    }

    private void InitEnergyBar()
    {
        m_Slider_Energy.value = 1;
       UpdateEnergy();

    }
    void UpdateEnergy(float value = 0)
    {
      float energy = m_ActiveStats[StatType.ForgeEnergy]+value;
      m_ActiveStats[StatType.ForgeEnergy]= energy<0?0:energy;
      
      float energyMax = m_BaseStats.First(x=>x.Stat_Type== StatType.ForgeEnergy).Stat_Value;
      float percent = m_ActiveStats[StatType.ForgeEnergy]/energyMax;
      m_Slider_Energy.value = percent;
      m_Slider_EnergyText.text =   Mathf.RoundToInt(percent*100).ToString();
    }

    private void InitStas()
    {
        m_ActiveStats = new Dictionary<StatType, float>();
        m_BaseStats.ForEach(x=>m_ActiveStats.Add(x.Stat_Type,x.Stat_Value));
        
    }


    async Awaitable ForgePressed()
    {
        SuccesHit = IsSucceshit;
        HitPercent =SuccesHit?1: m_HitProgress>0.5f? Mathf.Abs(.5f-m_HitProgress):m_HitProgress;
        m_ForgeButton.transform.DOKill(true);
        m_ForgeButton.transform.DOPunchScale(Vector2.one*-.1f,0.2f,vibrato:0).SetLink(this.gameObject);     
         

    }

    async Awaitable Forge(bool succesHit,float hitPercent)
    {
       
       
        bool fail = hitPercent<0.1f;

        m_Container.DOKill(true);       
        GameManager.Instance.SoundManager.PlayGivenSound(succesHit?"ForgeHit":"ForgeMiss",pitch: UnityEngine.Random.Range(1,1.1f));
        GameManager.Instance.HapticManager.VibradePreset(succesHit?HapticManager.HapticType.HIT: HapticManager.HapticType.PEEK);

        m_Container.DOPunchPosition(Vector2.down*20f*m_power,.2f);
        UpdateEnergy(-m_ActiveStats[StatType.ForgePower]);
        if(succesHit)  BlinkForge();
         PlayVFX(succesHit);
         OnUpdateSlider();

        
        var succesHitMessage = Tools.ShortNumeric(UnityEngine.Random.Range(6000,12000)*hitPercent);
        var failHitMessage = "<color=orange>"+succesHitMessage;
        string message =fail? "<color=red>miss": succesHit? succesHitMessage:failHitMessage;
        float scale =succesHit? UnityEngine.Random.Range(1.5f,2f):1;
        _=SpawnText(m_Forge.transform.position,message,scale);

    }

    
    // Update is called once per frame
    void PlayVFX(bool isSucces)
    {
        
        m_ForgeHitEffect.Emit(isSucces ?UnityEngine.Random.Range(10,30): 5);
    }
    void BlinkForge()
    {
        
        if(m_Forge.TryGetComponent(out UIEffect effect))
        {
            DOVirtual.Float(0, 1, 0.01f, value =>
            {
                effect.colorIntensity = value;
            }).SetLoops(2,LoopType.Yoyo).SetLink(this.gameObject);
        }
        if(m_ResourceImage.TryGetComponent(out UIEffect effect2))
        {
            DOVirtual.Float(0, 1, 0.01f, value =>
            {
                effect2.colorIntensity = value;
            }).SetLoops(2,LoopType.Yoyo).SetLink(this.gameObject);
        }

        m_ResourceContainer.DOKill(true);
        m_ResourceContainer.DOPunchScale(Vector2.one*0.1f,.2f,vibrato:0);
    }

    Tween tween;
    void InitSequencer(float duration=1 )
    {
        
       
        float timeCount = .5f;
        tween = DOVirtual.Float(0, 1, duration, (progress) =>
        {
                        m_Animator.SetFloat("Progress",progress);
                        m_HitProgress = progress;
            if (m_HitProgress >= timeCount)
            {

                _=Forge(SuccesHit,HitPercent);
                SuccesHit=false;
                HitPercent = 0;
                timeCount = 2;
            }

        }).OnStepComplete(() =>
        {
            timeCount = .5f;

        }).SetLoops(-1,LoopType.Restart).SetEase(Ease.Linear).SetLink(this.gameObject).SetId(m_Animator);

    }
    void InitTimer(int seconds = 120)
    {
        m_Slider_Timer.value = 1;
        m_Slider_Timer.DOValue(0, seconds).From(1).SetEase(Ease.Linear).OnUpdate(() =>
        {
             m_SliderTimerText.text = ToMMSS(Mathf.RoundToInt(seconds*m_Slider_Timer.value));
        }).SetLink(this.gameObject);
        
        
        string ToMMSS(int seconds)
        {
            TimeSpan ts = TimeSpan.FromSeconds(seconds);
            return $"{(int)ts.TotalMinutes}:{ts.Seconds:D2}";
        }
    }
    void InitSliders(int sliderIndex = 0)
    {
        var devisionStep = 1/(sliderIndex+1f);
        m_Slider_Base.value =sliderIndex==0?0: 1-1f/(sliderIndex+1);
        m_SliderTitleText.text = m_SliderTitles[Mathf.Clamp(sliderIndex,0,m_SliderTitles.Count-1)];

        for (int i = 0; i < m_Sliders.Count; i++)
        {
            m_Sliders[i].value = i<=sliderIndex?(i+1)*devisionStep: 1 ;
        }

    }
    void OnUpdateSlider()
    {
        m_Slider_Base.value+=m_SliderStep/(m_SliderCurrentIndex+1);
        if (m_Slider_Base.value >= 1)
        {
            m_SliderCurrentIndex++;
            InitSliders(m_SliderCurrentIndex);

        }


    }
    #region  Pool

    ObjectPool<TMP_Text> m_PoolText;
     [SerializeField] TMP_Text m_TextPrefab;
     public void PoolSetup()
    {

        m_PoolText= new ObjectPool<TMP_Text>(Create, OnGet, OnRelease, OnDestory, false, 5, 5);
        TMP_Text Create()
        {
            var poolingObj = Instantiate(m_TextPrefab, this.m_Container);
            return poolingObj;


        }
        void OnGet(TMP_Text obj)
        {
            obj.gameObject.SetActive(true);
        }
        void OnRelease(TMP_Text obj)
        {

             obj.alpha = 0;
            obj.transform.localScale = Vector3.one;
            obj.transform.localEulerAngles = Vector3.zero;
            obj?.gameObject.SetActive(false);
           


        }
        void OnDestory(TMP_Text obj)
        {
            Destroy(obj.gameObject);

        }

        
    }

async Awaitable SpawnText(Vector3 pos, string message,float size)
        {
            pos = pos+Vector3.right* UnityEngine.Random.value*-2;
            var rot = new Vector3(0,0,UnityEngine.Random.Range(-20,20));

            var text = m_PoolText.Get();
            text.text = message;
            text.rectTransform.localEulerAngles = rot;
            text.rectTransform.position = pos;
            text.transform.localScale = Vector3.one*size;

            var s = DOTween.Sequence();
            s.Append(text.DOFade(1,.3f));
            s.Join(text.rectTransform.DOPunchScale(Vector3.one*.2f,.3f));
            s.Join(text.rectTransform.DOLocalMoveY(100,1f));
            s.Join(text.DOFade(0,.2f).SetDelay(.8f));
            await s.ToAwaitable(destroyCancellationToken);
            m_PoolText.Release(text);

        }
    #endregion
}
