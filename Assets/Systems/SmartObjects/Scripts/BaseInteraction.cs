using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EInteractionType
{
    Instantaneous = 0,
    OverTime = 1
}

public abstract class BaseInteraction : MonoBehaviour
{
    [SerializeField] protected string _DisplayName;
    [SerializeField] protected EInteractionType _InteractionType = EInteractionType.Instantaneous;
    [SerializeField] protected float _Duration = 0f;

    public string DisplayName => _DisplayName;
    public EInteractionType InteractionType => _InteractionType;
    public float Duration => _Duration;

    public abstract bool CanPerform();
    public abstract void LockInteraction();
    public abstract void Perform(MonoBehaviour performer, UnityAction<BaseInteraction> onCompleted);
    public abstract void UnlockInteraction();
}
