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

    public void EnemyVisionModeChange()
    {
        if (_cooperativeCenter == null)
        {
            _cooperativeCenter = FindObjectOfType<CooperativeCenter>();
        }
        _cooperativeCenter.EnemyVisionModeChange();
    }
    public void EnemyBehaviorChange()
    {
        if (_cooperativeCenter == null)
        {
            _cooperativeCenter = FindObjectOfType<CooperativeCenter>();
        }
        _cooperativeCenter.EnemyBehaviorChange();
    }
    public void PlayerControlChange()
    {
        if (_cooperativeCenter == null)
        {
            _cooperativeCenter = FindObjectOfType<CooperativeCenter>();
        }
        _cooperativeCenter.PlayerControlChange();
    }

    public void PlayerDeadModeChange()
    {
        if (_cooperativeCenter == null)
        {
            _cooperativeCenter = FindObjectOfType<CooperativeCenter>();
        }
        _cooperativeCenter.PlayerDeadModeChange();
    }
}
