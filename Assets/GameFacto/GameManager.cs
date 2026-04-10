
using Sirenix.OdinInspector;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    //COSTS

    public static UnityAction OnInitializationComplete;
    public static GameManager Instance;

    //public static GameManager Instance;

    [GUIColor("orange")] public bool DebugLevel;

    [SerializeField] public GameConstants GameConstants;
    //
    // public bool ColorSorting;
    public bool SkipAuth = true;
    public bool InitOnAwake = true;

    public bool[] loadIsComplete = { false };

    //public TutorialManager TutorialManager;

    [SerializeField] private float TimeScale;
    [ShowInInspector] internal readonly float SpeedUp = 2;

    [Space]

    public ScriptableAssetsSO AssetScriptableData;
    private SettingsManager m_SettingsManager;
    public SettingsManager SettingsManager => m_SettingsManager;
    private HapticManager m_HapticManager;
    public HapticManager HapticManager => m_HapticManager;
    

    private SoundManager m_SoundManager;
    public SoundManager SoundManager => m_SoundManager;

    private TutorialManager m_TutorialManager;
    public TutorialManager TutorialManager => m_TutorialManager;
    private Player m_Player;
    public Player Player => m_Player;
    private UIManager m_UIManager;
    public UIManager UIManager => m_UIManager;

    public CameraManager CameraManager {set;get;}




    public static UnityAction<InteractionState> OnInteractionChanged;
    private InteractionState m_CurrentInteractionState;
    public bool UseAnalytics;

    public InteractionState InteractionState
    {
        set
        {
            bool sameState = (m_CurrentInteractionState == value);
            m_CurrentInteractionState = value;
            if (!sameState) OnInteractionChanged?.Invoke(m_CurrentInteractionState);
        }
        get
        {
            return m_CurrentInteractionState;
        }
    }

    private bool m_FullInit;
    private bool m_StartDone;
    private UnityAction m_InitAction;
    public static CancellationToken CancellationToken;
    public bool LeveEnded;
    public int MovesCounter = 0;
    public bool HasRevived = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }


#if UNITY_EDITOR 
        UseAnalytics = false;
#else
        UseAnalytics = true;
#endif

        Application.runInBackground = true;
        CancellationToken = this.destroyCancellationToken;
        m_SoundManager = this.GetComponentInChildren<SoundManager>(true);
        m_SettingsManager = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();
        m_HapticManager = GameObject.Find("HapticManager").GetComponent<HapticManager>();
        m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        m_TutorialManager = this.GetComponentInChildren<TutorialManager>(true);
        m_Player = this.GetComponentInChildren<Player>(true);
        CameraManager = FindAnyObjectByType<CameraManager>();

        if (InitOnAwake)
        {
            if (SkipAuth)
            {
                Init();
            }
            else
            {
                CheckIfAllComplete(Init);

            }
        }
    }
    bool CheckIfAllComplete(UnityAction action = null)
    {
        foreach (var item in loadIsComplete)
        {
            if (item == false) return false;
        }


        action?.Invoke();
        return true;
    }
    private void Start()
    {
        if (m_FullInit) m_InitAction?.Invoke();
        m_StartDone = true;
    }

    public void Init()
    {
        InteractionState = InteractionState.INGAME;

        m_InitAction = () =>
        {
            CameraManager.Initialize();
            UIManager.Initialize();
            Player.InitialisePlayer();
            SettingsManager.Initialize();
            HapticManager.Initialize();
            SoundManager.Initialize();

            OnInitializationComplete?.Invoke();

            //Start Game
           Tools.Log("Game Started");

        };



        if (m_StartDone)
        {
            m_InitAction?.Invoke();
        }

        m_FullInit = true;

    }






    


}


public enum InteractionState
{
    UI,
    INGAME
}


