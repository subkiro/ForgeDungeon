using System;
using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
public class MessageForgeRoom : MonoBehaviour
{

    [SerializeField] Button m_ForgeButton;
    [SerializeField] GameObject m_DwarfCharacter;
    [SerializeField] Image m_Forge;
    [SerializeField] ParticleSystem m_ForgeHitEffect;

    Animator m_Animator;

    [SerializeField] RectTransform m_Container;
    [SerializeField] float m_delay=.3f;
    [SerializeField] float m_power=.3f;
    [SerializeField] string m_SuccesHitID = "2_Attack";
    [SerializeField] string m_FailHitID;


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


    [SerializeField]TMP_Text m_Slider_SequencerText;

    [SerializeField] float m_SequenceDuration = .5f;
    [SerializeField] float m_SequenceSuccesChance = .5f;
    [SerializeField] Slider m_Slider_Sequencer;
    [SerializeField]Image m_SequencerMissImage;
    [SerializeField]Image m_SequencerHandleImage;
    bool IsSucceshit=>m_Slider_Sequencer.value>m_SequencerMissImage.fillAmount;


    //Energy bar
    [SerializeField] Slider m_Slider_Energy;
    [SerializeField]TMP_Text m_Slider_EnergyText;



    [SerializeField] List<Stats> m_BaseStats;
    Dictionary<StatType,float> m_ActiveStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        m_Animator = m_DwarfCharacter.GetComponentInChildren<Animator>();
        m_ForgeButton.onClick.AddListener(()=>_=ForgePressed());

        PoolSetup();
        InitStas();
        InitSliders();
        InitTimer();
        InitEnergyBar();
        InitSequencer(m_SequenceDuration,m_SequenceSuccesChance);
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
        bool succesHit = IsSucceshit;

        m_ForgeButton.transform.DOKill(true);
        m_ForgeButton.transform.DOPunchScale(Vector2.one*-.1f,0.2f,vibrato:0).SetLink(this.gameObject);     
        m_Animator.SetTrigger(m_SuccesHitID);
        m_Container.DOKill(true);

        UpdateEnergy(-m_ActiveStats[StatType.ForgePower]);

        await Awaitable.WaitForSecondsAsync(.3f);
        GameManager.Instance.SoundManager.PlayGivenSound(succesHit?"ForgeHit":"ForgeMiss",pitch: UnityEngine.Random.Range(1,1.1f));
        GameManager.Instance.HapticManager.VibradePreset(succesHit?HapticManager.HapticType.HIT: HapticManager.HapticType.PEEK);
        if(succesHit) m_Container.DOPunchPosition(Vector2.down*50f*m_power,.3f);
        if(succesHit)  BlinkForge();
        if(succesHit)  PlayVFX();
        if(succesHit)  OnUpdateSlider();

        string message = succesHit? Tools.ShortNumeric(UnityEngine.Random.Range(50,12000)):"<color=red>miss";
        float scale = UnityEngine.Random.Range(1,1.5f);
        _=SpawnText(m_Forge.transform.position,message,scale);

    }

    
    // Update is called once per frame
    void PlayVFX()
    {
        
        m_ForgeHitEffect.Emit(UnityEngine.Random.Range(10,30));
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

    void InitSequencer(float duration ,float succesChance = .5f)
    {
        m_Slider_SequencerText.text = $"{Mathf.RoundToInt(succesChance*100)}%";
        m_SequencerMissImage.fillAmount = 1-succesChance;
        m_Slider_Sequencer.DOKill();
        m_Slider_Sequencer.DOValue(1,duration).From(0).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.Linear).SetLink(this.gameObject);

    }
    void InitTimer(int seconds = 120)
    {
        m_Slider_Timer.value = 1;
        m_Slider_Timer.DOValue(0, seconds).From(1).SetEase(Ease.Linear).OnUpdate(() =>
        {
             m_SliderTimerText.text = ToMMSS(Mathf.RoundToInt(seconds*m_Slider_Timer.value));
        });
        
        
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
            var rot = new Vector3(0,0,UnityEngine.Random.Range(-30,30));

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
            await s.ToAwaitable();
            m_PoolText.Release(text);

        }
    #endregion
}
