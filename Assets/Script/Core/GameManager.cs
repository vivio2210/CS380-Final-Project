using System.Collections;
using System.Linq;
using Script;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SceneLoader SceneLoader => GetComponent<SceneLoader>();
    
    [SerializeField] private UIManager _uiManager;


    private IEnumerator ReloadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DataRecorder.Instance.EndSession();
        SceneLoader.Reload(Scene);
    }

    public void Reload()
    {
        StartCoroutine(ReloadAfterDelay(2f));
    }

    private void Awake()
    {
        Instance = this;

        _uiManager._levelToggleButton.OnValueChanged.AddListener(scene =>
        {
            if (scene != this.Scene)
            {
                SceneLoader.ChangeScene(this.Scene , scene);
                this.Scene = scene;
                OnSceneChanged.Invoke(scene);
            }

        });
        
        _uiManager._playerToggleButton.OnValueChanged.AddListener(mode =>
        {
            this.PlayerMode = mode;
            OnPlayerModeChanged.Invoke(mode);
        });
        
        _uiManager._enemyModeToggleButton.OnValueChanged.AddListener(mode =>
        {
            this.EnemyMode = mode;
            OnEnemyModeChanged.Invoke(mode);
        });
        
        _uiManager._enemyCaptureModeToggleButton.OnValueChanged.AddListener(mode =>
        {
            
            this.EnemyCaptureMode = mode;
            OnEnemyCaptureModeChanged.Invoke(mode);
        });
        
        _uiManager._enemyVisionModeToggleButton.OnValueChanged.AddListener(mode =>
        {
            this.EnemyVisionMode = mode;
            OnEnemyVisionModeChanged.Invoke(mode);
        });
        
        _uiManager._enemyPathDebugButton.OnValueChanged.AddListener(mode =>
        {
            this.EnemyPathDebug = mode;
            OnEnemyPathDebugChanged.Invoke(mode);
        });
        
        _uiManager._floorDebugButton.OnValueChanged.AddListener(mode =>
        {
            this.FloorDebug = mode;
            OnFloorDebugChanged.Invoke(mode);
        });
        
        _uiManager._levelResetButton.OnValueChanged.AddListener(call =>
        {
            SceneLoader.Reload(Scene);
        });
    }

    private void Start()
    {
        SceneLoader.LoadScene(GameSetting.SceneEnum.PacMan);
    }


    public GameSetting.SceneEnum Scene = GameSetting.SceneEnum.PacMan;
    public GameSetting.PlayerMode PlayerMode;
    public GameSetting.EnemyMode EnemyMode;
    public GameSetting.EnemyCaptureMode EnemyCaptureMode;
    public GameSetting.EnemyVisionMode EnemyVisionMode;
    public GameSetting.EnemyPathDebug EnemyPathDebug;
    public GameSetting.FloorDebug FloorDebug;
    
    public UnityEvent<GameSetting.SceneEnum> OnSceneChanged = new UnityEvent<GameSetting.SceneEnum>();
    public UnityEvent<GameSetting.PlayerMode> OnPlayerModeChanged = new UnityEvent<GameSetting.PlayerMode>();
    public UnityEvent<GameSetting.EnemyMode> OnEnemyModeChanged = new UnityEvent<GameSetting.EnemyMode>();
    public UnityEvent<GameSetting.EnemyCaptureMode> OnEnemyCaptureModeChanged = new UnityEvent<GameSetting.EnemyCaptureMode>();
    public UnityEvent<GameSetting.EnemyVisionMode> OnEnemyVisionModeChanged = new UnityEvent<GameSetting.EnemyVisionMode>();
    public UnityEvent<GameSetting.EnemyPathDebug> OnEnemyPathDebugChanged = new UnityEvent<GameSetting.EnemyPathDebug>();
    public UnityEvent<GameSetting.FloorDebug> OnFloorDebugChanged = new UnityEvent<GameSetting.FloorDebug>();
    
}