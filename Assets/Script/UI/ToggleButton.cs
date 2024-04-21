
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public abstract class ToggleButton<T>: MonoBehaviour 
{
    
    [ShowInInspector]private int _currentIndex = 0;
    [ShowInInspector]protected Dictionary<T,string> EnumNames = new Dictionary<T, string>();
    
    public T CurrentEnum => EnumNames.ElementAt(_currentIndex).Key;
    public UnityEvent<T> OnValueChanged = new UnityEvent<T>();

    private void Awake()
    {
        foreach (T enumValue in System.Enum.GetValues(typeof(T)))
        {
            EnumNames.Add(enumValue, enumValue.ToString());
        }
        SetText(_currentIndex);
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    
    protected void RemoveOption(T option)
    {
        EnumNames.Remove(option);
        SetText(0);
    }
    
    private void SetText(int index)
    {
        GetComponentInChildren<TextMeshProUGUI>().text =
            EnumNames.ElementAt(index).Value;
    }

    public void OnClick()
    {
        _currentIndex++;
        if (_currentIndex >= EnumNames.Count)
        {
            _currentIndex = 0;
        }
        SetText(_currentIndex);
        OnValueChanged.Invoke(EnumNames.ElementAt(_currentIndex).Key);
    }

}
