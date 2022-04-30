using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseNavigation))]
public class SimpleAI : CommonAIBase
{
    [SerializeField] protected float PickInteractionInterval = 2f;

    protected float TimeUntilNextInteractionPicked = -1f;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (CurrentInteraction == null)
        {
            TimeUntilNextInteractionPicked -= Time.deltaTime;

            // time to pick an interaction
            if (TimeUntilNextInteractionPicked <= 0)
            {
                TimeUntilNextInteractionPicked = PickInteractionInterval;
                PickRandomInteraction();
            }
        }
    }

    void PickRandomInteraction()
    {
        // pick a random object
        int objectIndex = Random.Range(0, SmartObjectManager.Instance.RegisteredObjects.Count);
        var selectedObject = SmartObjectManager.Instance.RegisteredObjects[objectIndex];

        // pick a random interaction
        int interactionIndex = Random.Range(0, selectedObject.Interactions.Count);
        var selectedInteraction = selectedObject.Interactions[interactionIndex];

        // can perform the interaction?
        if (selectedInteraction.CanPerform())
        {
            CurrentInteraction = selectedInteraction;
            CurrentInteraction.LockInteraction(this);
            StartedPerforming = false;

            // move to the target
            if (!Navigation.SetDestination(selectedObject.InteractionPoint))
            {
                Debug.LogError($"Could not move to {selectedObject.name}");
                CurrentInteraction = null;
            }
            else
                Debug.Log($"Going to {CurrentInteraction.DisplayName} at {selectedObject.DisplayName}");
        }
    }
}
