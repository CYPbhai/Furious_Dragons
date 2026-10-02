using System;
using UnityEngine;

[CreateAssetMenu(fileName="StringChannelEventSO", menuName = "ScriptableObjects/Events/StringChannelEventSO")]
public class StringChannelEventSO : ScriptableObject
{
    public event Action<string> OnRised;

    public void Raise(string str)
    {
        OnRised?.Invoke(str);
    }
}
