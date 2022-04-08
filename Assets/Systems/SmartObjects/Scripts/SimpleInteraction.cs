using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleInteraction : BaseInteraction
{
    protected class PerformerInfo
    {
        public CommonAIBase PerformingAI;
        public float ElapsedTime;
        public UnityAction<BaseInteraction> OnCompleted;
    }

    [SerializeField] protected int MaxSimultaneousUsers = 1;

    protected int NumCurrentUsers = 0;
    protected List<PerformerInfo> CurrentPerformers = new List<PerformerInfo> ();
    
    public override bool CanPerform()
    {
        return NumCurrentUsers < MaxSimultaneousUsers;
    }

    public override void LockInteraction()
    {
        ++NumCurrentUsers;

        if (NumCurrentUsers > MaxSimultaneousUsers)
            Debug.LogError($"Too many users have locked this interaction {_DisplayName}");
    }

    public override void Perform(CommonAIBase performer, UnityAction<BaseInteraction> onCompleted)
    {
        if (NumCurrentUsers <= 0)
        {
            Debug.LogError($"Trying to perform an interaction when there are no users {_DisplayName}");
            return;
        }

        // check the interaction type
        if (InteractionType == EInteractionType.Instantaneous)
        {
            if (StatChanges.Length > 0)
                ApplyStatChanges(performer, 1f);

            onCompleted.Invoke(this);
        }
        else if (InteractionType == EInteractionType.OverTime)
        {
            CurrentPerformers.Add(new PerformerInfo() { PerformingAI = performer,
                                                        ElapsedTime = 0, 
                                                        OnCompleted = onCompleted });
        }
    }

    public override void UnlockInteraction()
    {
        if (NumCurrentUsers <= 0)
            Debug.LogError($"Trying to unlock an already unlocked interaction {_DisplayName}");

        --NumCurrentUsers;
    }

    protected virtual void Update()
    {
        // update any current performers
        for(int index = CurrentPerformers.Count - 1; index >= 0; index--)
        {
            PerformerInfo performer = CurrentPerformers[index];

            float previousElapsedTime = performer.ElapsedTime;
            performer.ElapsedTime = Mathf.Min(performer.ElapsedTime + Time.deltaTime, _Duration);

            if (StatChanges.Length > 0)
                ApplyStatChanges(performer.PerformingAI, 
                                 (performer.ElapsedTime - previousElapsedTime) / _Duration);

            // interaction complete?
            if (performer.ElapsedTime >= _Duration)
            {
                performer.OnCompleted.Invoke(this);
                CurrentPerformers.RemoveAt(index);
            }
        }
    }
}
