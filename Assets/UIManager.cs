using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private LevelToggleButton _levelToggleButton;
    [SerializeField] private PlayerToggleButton _playerToggleButton;
    [SerializeField] private EnemyModeToggleButton _enemyModeToggleButton;
    [SerializeField] private EnemyCaptureModeToggleButton _enemyCaptureModeToggleButton;
    [SerializeField] private EnemyVisionModeToggleButton _enemyVisionModeToggleButton;

    private void Start()
    {
        _levelToggleButton.OnValueChanged.
            AddListener(GameManager.Instance.SceneLoader.ChangeScene);
    }
}
