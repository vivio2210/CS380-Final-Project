using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] public LevelToggleButton _levelToggleButton;
    [SerializeField] public PlayerToggleButton _playerToggleButton;
    [SerializeField] public EnemyModeToggleButton _enemyModeToggleButton;
    [SerializeField] public EnemyCaptureModeToggleButton _enemyCaptureModeToggleButton;
    [SerializeField] public EnemyVisionModeToggleButton _enemyVisionModeToggleButton;

    private void Start()
    {
        
    }
}
