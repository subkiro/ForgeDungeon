using System;
using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
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

    [SerializeField] Slider m_Slider_Base;
    [SerializeField] Slider m_Slider_0;
    [SerializeField] Slider m_Slider_1;
    [SerializeField] Slider m_Slider_2;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        m_Animator = m_DwarfCharacter.GetComponentInChildren<Animator>();
        m_ForgeButton.onClick.AddListener(()=>_=ForgePressed());
        PoolSetup();
    }
    async Awaitable ForgePressed()
    {
        m_ForgeButton.transform.DOKill(true);
        m_ForgeButton.transform.DOPunchScale(Vector2.one*-.1f,0.2f,vibrato:0).SetLink(this.gameObject);
        m_Animator.SetTrigger("2_Attack");
        m_Container.DOKill(true);
        await Awaitable.WaitForSecondsAsync(.3f);
        m_Container.DOPunchPosition(Vector2.down*50f*m_power,.3f);
        GameManager.Instance.SoundManager.PlayGivenSound("ForgeHit",pitch: UnityEngine.Random.Range(1,1.1f));
        BlinkForge();
        PlayVFX();
        _=SpawnText(m_Forge.transform.position,UnityEngine.Random.Range(50,1000),UnityEngine.Random.Range(1,1.5f));
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

async Awaitable SpawnText(Vector3 pos, float amount,float size)
        {
            pos = pos+Vector3.right* UnityEngine.Random.value*-2;
            var rot = new Vector3(0,0,UnityEngine.Random.Range(-30,30));

            var text = m_PoolText.Get();
            text.text = amount.ToString();
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
