using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "AI/Memory", fileName = "Memory")]
public class MemoryFragment : ScriptableObject
{
    public string Name;
    public string Description;
    public float Duration = 0f;
    public InteractionStatChange[] StatChanges;
    public MemoryFragment[] MemoriesCountered;

    public int Occurrences { get; private set; } = 0;
    public float DurationRemaining { get; private set; } = 0;

    public bool IsSimilarTo(MemoryFragment other)
    {
        return Name == other.Name && Description == other.Description;
    }

    public bool IsCancelledBy(MemoryFragment other)
    {
        foreach(var fragment in MemoriesCountered)
        {
            if (fragment.IsSimilarTo(other))
                return true;
        }

        return false;
    }

    public void Reinforce(MemoryFragment other)
    {
        DurationRemaining = Mathf.Max(DurationRemaining, other.DurationRemaining);
        Occurrences++;
    }

    public MemoryFragment Duplicate()
    {
        var newMemory = ScriptableObject.Instantiate(this);
        newMemory.Occurrences = 1;
        newMemory.DurationRemaining = Duration;

        return newMemory;
    }

    public bool Tick(float deltaTime)
    {
        DurationRemaining -= deltaTime;

        return DurationRemaining > 0;
    }
}
