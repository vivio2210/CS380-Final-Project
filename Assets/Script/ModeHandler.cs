using System;
using Script;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Script.GameSetting;

public class ModeHandler : MonoBehaviour
{
    private static ModeHandler _instance;

    [SerializeField]private CooperativeCenter _cooperativeCenter;

    private void Start()
    {
        if (GameManager.Instance)
        {
            PlayerControlChange(GameManager.Instance.PlayerMode);
            EnemyBehaviorChange(GameManager.Instance.EnemyMode);
            PlayerDeadModeChange(GameManager.Instance.EnemyCaptureMode);
            EnemyVisionModeChange(GameManager.Instance.EnemyVisionMode);

            GameManager.Instance.OnPlayerModeChanged.AddListener(PlayerControlChange);
            GameManager.Instance.OnEnemyModeChanged.AddListener(EnemyBehaviorChange);
            GameManager.Instance.OnEnemyCaptureModeChanged.AddListener(PlayerDeadModeChange);
            GameManager.Instance.OnEnemyVisionModeChanged.AddListener(EnemyVisionModeChange);

        } 
    }

    public void EnemyVisionModeChange(GameSetting.EnemyVisionMode mode)
    {
        _cooperativeCenter.EnemyVisionModeChange(mode);
    }

    public void EnemyBehaviorChange(GameSetting.EnemyMode mode)
    {
        _cooperativeCenter.EnemyBehaviorChange(mode);
    }

    public void PlayerControlChange(GameSetting.PlayerMode mode)
    {
        _cooperativeCenter.PlayerControlChange(mode);
    }

    public void PlayerDeadModeChange(GameSetting.EnemyCaptureMode mode)
    {
        _cooperativeCenter.PlayerDeadModeChange(mode);
    }


}