using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class TraitElement
{
    public AIStat LinkedStat;

    [Header("Scoring Scales")]
    [Range(0.5f, 1.5f)][FormerlySerializedAs("PositiveScale")] public float Scoring_PositiveScale = 1f;
    [Range(0.5f, 1.5f)][FormerlySerializedAs("NegativeScale")] public float Scoring_NegativeScale = 1f;

    [Header("Impact Scales")]
    [Range(0.5f, 1.5f)] public float Impact_PositiveScale = 1f;
    [Range(0.5f, 1.5f)] public float Impact_NegativeScale = 1f;

    [Header("Decay Rate")]
    [Range(0.5f, 1.5f)] public float DecayRateScale = 1f;

    public float Apply(AIStat targetStat, Trait.ETargetType targetType, float currentValue)
    {
        if (targetStat == LinkedStat)
        {
            if (targetType == Trait.ETargetType.DecayRate)
                currentValue *= DecayRateScale;
            else if (targetType == Trait.ETargetType.Impact)
            {
                if (currentValue > 0)
                    currentValue *= Impact_PositiveScale;
                else if (currentValue < 0)
                    currentValue *= Impact_NegativeScale;
            }
            else
            {
                if (currentValue > 0)
                    currentValue *= Scoring_PositiveScale;
                else if (currentValue < 0)
                    currentValue *= Scoring_NegativeScale;
            }
        }

        return currentValue;
    }
}

[CreateAssetMenu(menuName = "AI/Trait", fileName = "Trait")]
public class Trait : ScriptableObject
{
    public enum ETargetType
    {
        Score,
        Impact,
        DecayRate
    }

    public string DisplayName;
    public TraitElement[] Impacts;

    public float Apply(AIStat targetStat, Trait.ETargetType targetType, float currentValue)
    {
        foreach (var impact in Impacts)
        {
            currentValue = impact.Apply(targetStat, targetType, currentValue);
        }

        return currentValue;
    }
}
