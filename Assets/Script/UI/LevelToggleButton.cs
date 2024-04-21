using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Script;
using UnityEngine;

public class LevelToggleButton: ToggleButton<GameSetting.SceneEnum>
{
    private void Start()
    {
        RemoveOption(GameSetting.SceneEnum.MainMenu);
    }

}
