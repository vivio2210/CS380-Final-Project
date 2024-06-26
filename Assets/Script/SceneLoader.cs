using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [ShowInInspector]
    private Dictionary<GameSetting.SceneEnum,bool> _openScenes = new Dictionary<GameSetting.SceneEnum, bool>();
   
    // Start is called before the first frame update
    void Awake()
    {  
        //add all enum to dictionary
        foreach (GameSetting.SceneEnum scene in Enum.GetValues(typeof(GameSetting.SceneEnum)))
        {
            _openScenes.Add(scene, false);
        }

        _openScenes[GameSetting.SceneEnum.MainMenu] = true;

    }

    public void Reload(GameSetting.SceneEnum scene)
    {
        UnloadScene(scene);
        LoadScene(scene);
    }

    public void ChangeScene(GameSetting.SceneEnum oldSceneEnum, GameSetting.SceneEnum newSceneEnum)
    {
        DataRecorder.Instance.EndSession();

        UnloadScene(oldSceneEnum);
        LoadScene(newSceneEnum);
    }
    
    [Button]
    public void LoadScene(GameSetting.SceneEnum scene)
    {
        if (!_openScenes[scene])
        {
            SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Additive);
            _openScenes[scene] = true;

        }
        
    }
    [Button]

    public void UnloadScene(GameSetting.SceneEnum scene)
    {
        if (_openScenes[scene])
        {
            SceneManager.UnloadSceneAsync((int)scene);
            _openScenes[scene] = false;

        }
    }
}
