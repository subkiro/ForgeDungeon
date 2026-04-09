using System;
using UnityEngine;
using UnityEngine.Pool;


public class VFXManager : SingletonObj<VFXManager>
{

    [SerializeField] private ParticleSystem m_MergeVFX;
    public override void Awake()
    {
        base.Awake();

    }
    private void Start()
    {

        BuffPoolSetup();

    }
  



    #region Pool Buff
    [Header("Pop Buff")]
    [SerializeField] private ParticlePoolData m_BuffPoolData;


    public void BuffPoolSetup()
    {

        m_BuffPoolData.pool = new ObjectPool<ParticleSystem>(Create, OnGet, OnRelease, OnDestory, false, m_BuffPoolData.pool_DefaultAmount, m_BuffPoolData.pool_MaxAmoun);
        ParticleSystem Create()
        {
            var poolingObj = Instantiate(m_BuffPoolData.prefab, this.transform);
            m_BuffPoolData.defaultScale = poolingObj.transform.localScale;
            return poolingObj.GetComponent<ParticleSystem>();


        }
        void OnGet(ParticleSystem obj)
        {
            obj.gameObject.SetActive(true);
        }
        void OnRelease(ParticleSystem obj)
        {
            obj?.gameObject.SetActive(false);
            obj.transform.localScale = m_BuffPoolData.defaultScale;


        }
        void OnDestory(ParticleSystem obj)
        {
            Destroy(obj.gameObject);

        }
    }


    public async Awaitable ShowBuff(Vector3 startPos, float scaleMax = 1)
    {
        if (m_BuffPoolData.pool.CountActive >= m_BuffPoolData.pool_MaxAmoun) return;


        ParticleSystem poolObj = m_BuffPoolData.pool.Get();
        //   Debug.Log(scale);
        poolObj.transform.position = startPos;
        poolObj.transform.localScale = m_BuffPoolData .defaultScale* scaleMax;

        poolObj.Play();

        await Awaitable.WaitForSecondsAsync((int)poolObj.main.duration * 1000);
        m_BuffPoolData.pool?.Release(poolObj);
    }
}


    [Serializable]
    public struct PoolHiddenData
    {
        public Vector3 Offcet;
        public Vector3 Scale;
    }
[Serializable]
public struct ParticlePoolData
{
    public ParticleSystem prefab;
    public int pool_DefaultAmount;
    public int pool_MaxAmoun;
    [HideInInspector]
    public ObjectPool<ParticleSystem> pool;
    [HideInInspector]
    public Vector3 defaultScale;
}

#endregion

