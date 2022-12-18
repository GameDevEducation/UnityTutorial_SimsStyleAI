using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleInteraction : BaseInteraction
{
    protected class PerformerInfo
    {
        public float ElapsedTime;
        public UnityAction<BaseInteraction> OnCompleted;
    }

    [SerializeField] protected int MaxSimultaneousUsers = 1;

    protected Dictionary<CommonAIBase, PerformerInfo> CurrentPerformers = new Dictionary<CommonAIBase, PerformerInfo> ();
    public int NumCurrentUsers => CurrentPerformers.Count;

    protected List<CommonAIBase> PerformersToCleanup = new List<CommonAIBase>();

    public override bool CanPerform()
    {
        return NumCurrentUsers < MaxSimultaneousUsers;
    }

    public override bool LockInteraction(CommonAIBase performer)
    {
        if (NumCurrentUsers >= MaxSimultaneousUsers)
        {
            Debug.LogError($"{performer.name} trying to lock {_DisplayName} which is already at max users");
            return false;
        }

        if (CurrentPerformers.ContainsKey(performer))
        {
            Debug.LogError($"{performer.name} tried to lock {_DisplayName} multiple times.");
            return false;
        }

        CurrentPerformers[performer] = null;

        return true;
    }

    public override bool Perform(CommonAIBase performer, UnityAction<BaseInteraction> onCompleted)
    {
        if (!CurrentPerformers.ContainsKey(performer))
        {
            Debug.LogError($"{performer.name} is trying to perform an interaction {_DisplayName} that they have not locked");
            return false;
        }

        // check the interaction type
        if (InteractionType == EInteractionType.Instantaneous)
        {
            if (StatChanges.Length > 0)
                ApplyInteractionEffects(performer, 1f, true);

            OnInteractionCompleted(performer, onCompleted);
        }
        else if (InteractionType == EInteractionType.OverTime || InteractionType == EInteractionType.AfterTime)
        {
            CurrentPerformers[performer] = new PerformerInfo() { ElapsedTime = 0, OnCompleted = onCompleted };
        }

        return true;
    }

    protected void OnInteractionCompleted(CommonAIBase performer, UnityAction<BaseInteraction> onCompleted)
    {
        onCompleted.Invoke(this);

        if (!PerformersToCleanup.Contains(performer))
        {
            PerformersToCleanup.Add(performer);
            Debug.LogWarning($"{performer.name} did not unlock interaction in their OnCompleted handler for {_DisplayName}");
        }
    }

    public override bool UnlockInteraction(CommonAIBase performer)
    {
        if (CurrentPerformers.ContainsKey(performer))
        {
            PerformersToCleanup.Add(performer);
            return true;
        }

        Debug.LogError($"{performer.name} is trying to unlock an interaction {_DisplayName} they have not locked");

        return false;
    }

    protected virtual void Update()
    {
        // update any current performers
        foreach(var kvp in CurrentPerformers)
        {
            CommonAIBase performer = kvp.Key;
            PerformerInfo performerInfo = kvp.Value;

            if (performerInfo == null)
                continue;

            float previousElapsedTime = performerInfo.ElapsedTime;
            performerInfo.ElapsedTime = Mathf.Min(performerInfo.ElapsedTime + Time.deltaTime, _Duration);
            bool isFinalTick = performerInfo.ElapsedTime >= _Duration;

            bool continueInteraction = false;
            if (StatChanges.Length > 0 && 
                ((InteractionType == EInteractionType.OverTime) ||
                 (InteractionType == EInteractionType.AfterTime && isFinalTick)))
            {
                continueInteraction = ApplyInteractionEffects(performer, (performerInfo.ElapsedTime - previousElapsedTime) / _Duration, isFinalTick);
            }

            // interaction complete?
            if (!continueInteraction || isFinalTick)
                OnInteractionCompleted(performer, performerInfo.OnCompleted);
        }

        // cleanup any performers that are finished
        foreach(var performer in PerformersToCleanup)
            CurrentPerformers.Remove(performer);
        PerformersToCleanup.Clear();
    }
}
