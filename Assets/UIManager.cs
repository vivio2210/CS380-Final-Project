using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private LevelToggleButton _levelToggleButton;

    private void Start()
    {
        _levelToggleButton.OnValueChanged.
            AddListener(GameManager.Instance.SceneLoader.ChangeScene);
    }
}
