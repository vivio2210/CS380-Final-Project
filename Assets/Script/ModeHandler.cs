using Script;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Script.GameSetting;

public class ModeHandler : MonoBehaviour
{
    private static ModeHandler _instance;

    private CooperativeCenter _cooperativeCenter;

    public static ModeHandler GetInstance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ModeHandler>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void EnemyVisionModeChange(GameSetting.EnemyVisionMode mode)
    {
        if (_cooperativeCenter == null)
        {
            _cooperativeCenter = FindObjectOfType<CooperativeCenter>();
        }
        _cooperativeCenter.EnemyVisionModeChange(mode);
    }
    public void EnemyBehaviorChange(GameSetting.EnemyMode mode)
    {
        if (_cooperativeCenter == null)
        {
            _cooperativeCenter = FindObjectOfType<CooperativeCenter>();
        }
        _cooperativeCenter.EnemyBehaviorChange(mode);
    }
    public void PlayerControlChange(GameSetting.PlayerMode mode)
    {
        if (_cooperativeCenter == null)
        {
            _cooperativeCenter = FindObjectOfType<CooperativeCenter>();
        }
        _cooperativeCenter.PlayerControlChange(mode);
    }

    public void PlayerDeadModeChange(GameSetting.EnemyCaptureMode mode)
    {
        if (_cooperativeCenter == null)
        {
            _cooperativeCenter = FindObjectOfType<CooperativeCenter>();
        }
        _cooperativeCenter.PlayerDeadModeChange(mode);
    }
}
