using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Stat", fileName = "AIStat")]
public class AIStat : ScriptableObject
{
    [field: SerializeField] public string DisplayName { get; protected set; }
    [field: SerializeField] public bool IsVisible { get; protected set; } = true;
    [field: SerializeField, Range(0f, 1f)] public float InitialValue { get; protected set; } = 0.5f;
    [field: SerializeField, Range(0f, 1f)] public float DecayRate { get; protected set; } = 0.005f;
}
