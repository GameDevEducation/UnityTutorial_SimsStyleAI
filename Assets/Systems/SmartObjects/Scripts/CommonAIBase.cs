using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIStatConfiguration
{
    [field: SerializeField] public AIStat LinkedStat { get; private set; }

    [field: SerializeField] public bool OverrideDefaults { get; private set; } = false;
    [field: SerializeField, Range(0f, 1f)] public float Override_InitialValue { get; protected set; } = 0.5f;
    [field: SerializeField, Range(0f, 1f)] public float Override_DecayRate { get; protected set; } = 0.005f;
}

[RequireComponent(typeof(BaseNavigation))]
public class CommonAIBase : MonoBehaviour
{
    [Header("General")]
    [SerializeField] int HouseholdID = 1;
    [field: SerializeField] AIStatConfiguration[] Stats;
    [SerializeField] protected FeedbackUIPanel LinkedUI;

    [Header("Traits")]
    [SerializeField] protected List<Trait> Traits;

    protected BaseNavigation Navigation;
    protected bool StartedPerforming = false;

    public Blackboard IndividualBlackboard { get; protected set; }
    public Blackboard HouseholdBlackboard { get; protected set; }

    protected Dictionary<AIStat, float> DecayRates = new Dictionary<AIStat, float>();
    protected Dictionary<AIStat, AIStatPanel> StatUIPanels = new Dictionary<AIStat, AIStatPanel>();

    protected BaseInteraction CurrentInteraction
    {
        get 
        {
            BaseInteraction interaction = null;
            IndividualBlackboard.TryGetGeneric(EBlackboardKey.Character_FocusObject, out interaction, null);
            return interaction; 
        }
        set 
        {
            BaseInteraction previousInteraction = null;
            IndividualBlackboard.TryGetGeneric(EBlackboardKey.Character_FocusObject, out previousInteraction, null);

            IndividualBlackboard.SetGeneric(EBlackboardKey.Character_FocusObject, value);

            List<GameObject> objectsInUse = null;
            HouseholdBlackboard.TryGetGeneric(EBlackboardKey.Household_ObjectsInUse, out objectsInUse, null);

            // are we starting to use something?
            if (value != null)
            {
                // need to create list?
                if (objectsInUse == null)
                    objectsInUse = new List<GameObject>();

                // not already in list? add and update the blackboard
                if (!objectsInUse.Contains(value.gameObject))
                {
                    objectsInUse.Add(value.gameObject);
                    HouseholdBlackboard.SetGeneric(EBlackboardKey.Household_ObjectsInUse, objectsInUse);
                }
            } // we've stopped using something
            else if (objectsInUse != null)
            {
                // attempt to remove and update the blackboard if changed
                if (objectsInUse.Remove(previousInteraction.gameObject))
                    HouseholdBlackboard.SetGeneric(EBlackboardKey.Household_ObjectsInUse, objectsInUse);
            }
        }
    }

    protected virtual void Awake()
    {
        Navigation = GetComponent<BaseNavigation>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        HouseholdBlackboard = BlackboardManager.Instance.GetSharedBlackboard(HouseholdID);
        IndividualBlackboard = BlackboardManager.Instance.GetIndividualBlackboard(this);

        // setup the stats
        foreach (var statConfig in Stats)
        {
            var linkedStat = statConfig.LinkedStat;
            float initialValue = statConfig.OverrideDefaults ? statConfig.Override_InitialValue : linkedStat.InitialValue;
            float decayRate = statConfig.OverrideDefaults ? statConfig.Override_DecayRate : linkedStat.DecayRate;

            DecayRates[linkedStat] = decayRate;
            IndividualBlackboard.SetStat(linkedStat, initialValue);

            if (linkedStat.IsVisible)
                StatUIPanels[linkedStat] = LinkedUI.AddStat(linkedStat, initialValue);
        }
    }

    protected float ApplyTraitsTo(AIStat targetStat, Trait.ETargetType targetType, float currentValue)
    {
        foreach(var trait in Traits)
        {
            currentValue = trait.Apply(targetStat, targetType, currentValue);
        }

        return currentValue;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (CurrentInteraction != null)
        {
            if (Navigation.IsAtDestination && !StartedPerforming)
            {
                StartedPerforming = true;
                CurrentInteraction.Perform(this, OnInteractionFinished);
            }
        }

        // apply the decay rate
        foreach(var statConfig in Stats)
        {
            UpdateIndividualStat(statConfig.LinkedStat, -DecayRates[statConfig.LinkedStat] * Time.deltaTime, Trait.ETargetType.DecayRate);
        }
    }

    protected virtual void OnInteractionFinished(BaseInteraction interaction)
    {
        interaction.UnlockInteraction(this);
        CurrentInteraction = null;
        Debug.Log($"Finished {interaction.DisplayName}");
    }

    public void UpdateIndividualStat(AIStat linkedStat, float amount, Trait.ETargetType targetType)
    {
        float adjustedAmount = ApplyTraitsTo(linkedStat, targetType, amount);
        float newValue = Mathf.Clamp01(GetStatValue(linkedStat) + adjustedAmount);

        IndividualBlackboard.SetStat(linkedStat, newValue);

        if (linkedStat.IsVisible)
            StatUIPanels[linkedStat].OnStatChanged(newValue);
    }

    public float GetStatValue(AIStat linkedStat)
    {
        return IndividualBlackboard.GetStat(linkedStat);
    }
}
