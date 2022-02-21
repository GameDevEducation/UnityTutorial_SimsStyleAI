using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public abstract class BaseAI : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI FeedbackDisplay;

    [SerializeField] protected float _VisionConeAngle = 60f;
    [SerializeField] protected float _VisionConeRange = 30f;
    [SerializeField] protected Color _VisionConeColour = new Color(1f, 0f, 0f, 0.25f);

    [SerializeField] protected float _HearingRange = 20f;
    [SerializeField] protected Color _HearingRangeColour = new Color(1f, 1f, 0f, 0.25f);

    [SerializeField] protected float _ProximityDetectionRange = 3f;
    [SerializeField] protected Color _ProximityRangeColour = new Color(1f, 1f, 1f, 0.25f);

    public Vector3 EyeLocation => transform.position;
    public Vector3 EyeDirection => transform.forward;

    public float VisionConeAngle => _VisionConeAngle;
    public float VisionConeRange => _VisionConeRange;
    public Color VisionConeColour => _VisionConeColour;

    public float HearingRange => _HearingRange;
    public Color HearingRangeColour => _HearingRangeColour;

    public float ProximityDetectionRange => _ProximityDetectionRange;
    public Color ProximityDetectionColour => _ProximityRangeColour;

    public float CosVisionConeAngle { get; private set; } = 0f;

    protected AwarenessSystem Awareness;

    void Awake()
    {
        CosVisionConeAngle = Mathf.Cos(VisionConeAngle * Mathf.Deg2Rad);
        Awareness = GetComponent<AwarenessSystem>();
    }

    public void ReportCanSee(DetectableTarget seen)
    {
        Awareness.ReportCanSee(seen);
    }

    public void ReportCanHear(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        Awareness.ReportCanHear(source, location, category, intensity);
    }

    public void ReportInProximity(DetectableTarget target)
    {
        Awareness.ReportInProximity(target);
    }

    public virtual void OnSuspicious()
    {
    }

    public virtual void OnDetected(GameObject target)
    {
    }

    public virtual void OnFullyDetected(GameObject target)
    {
    }

    public virtual void OnLostDetect(GameObject target)
    {
    }

    public virtual void OnLostSuspicion()
    {
    }

    public virtual void OnFullyLost()
    {
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BaseAI))]
public class BaseAIEditor : Editor
{
    public void OnSceneGUI()
    {
        var ai = target as BaseAI;

        // draw the detectopm range
        Handles.color = ai.ProximityDetectionColour;
        Handles.DrawSolidDisc(ai.transform.position, Vector3.up, ai.ProximityDetectionRange);

        // draw the hearing range
        Handles.color = ai.HearingRangeColour;
        Handles.DrawSolidDisc(ai.transform.position, Vector3.up, ai.HearingRange);

        // work out the start point of the vision cone
        Vector3 startPoint = Mathf.Cos(-ai.VisionConeAngle * Mathf.Deg2Rad) * ai.transform.forward +
                             Mathf.Sin(-ai.VisionConeAngle * Mathf.Deg2Rad) * ai.transform.right;

        // draw the vision cone
        Handles.color = ai.VisionConeColour;
        Handles.DrawSolidArc(ai.transform.position, Vector3.up, startPoint, ai.VisionConeAngle * 2f, ai.VisionConeRange);
    }
}
#endif // UNITY_EDITOR