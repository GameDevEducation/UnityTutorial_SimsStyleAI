using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseAI))]
public class VisionSensor : MonoBehaviour
{
    [SerializeField] LayerMask DetectionMask = ~0;

    BaseAI LinkedAI;
    LocalDetectableTargetManager TargetManager;

    // Start is called before the first frame update
    void Start()
    {
        LinkedAI = GetComponent<BaseAI>();
        TargetManager = GetComponent<LocalDetectableTargetManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // check all candidates
        List<DetectableTarget> targets = TargetManager != null ? TargetManager.AllTargets : DetectableTargetManager.Instance.AllTargets;

        for (int index = 0; index < targets.Count; ++index)
        {
            var candidateTarget = targets[index];

            // skip if the candidate is ourselves
            if (candidateTarget.gameObject == gameObject)
                continue;

            var vectorToTarget = candidateTarget.transform.position - LinkedAI.EyeLocation;

            // if out of range - cannot see
            if (vectorToTarget.sqrMagnitude > (LinkedAI.VisionConeRange * LinkedAI.VisionConeRange))
                continue;

            vectorToTarget.Normalize();

            // if out of vision cone - cannot see
            if (Vector3.Dot(vectorToTarget, LinkedAI.EyeDirection) < LinkedAI.CosVisionConeAngle)
                continue;

            // raycast to target passes?
            RaycastHit hitResult;
            if (Physics.Raycast(LinkedAI.EyeLocation, vectorToTarget, out hitResult, 
                                LinkedAI.VisionConeRange, DetectionMask, QueryTriggerInteraction.Collide))
            {
                if (hitResult.collider.GetComponentInParent<DetectableTarget>() == candidateTarget)
                    LinkedAI.ReportCanSee(candidateTarget);
            }
        }
    }
}
